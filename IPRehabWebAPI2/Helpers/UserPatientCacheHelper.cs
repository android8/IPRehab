using IPRehabModel;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PatientModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Helpers
{
    public class UserPatientCacheHelper : IUserPatientCacheHelper
    {
        protected readonly MasterreportsContext _context;
        protected readonly IConfiguration _configuration;
        private readonly IOptions<CustomAppSettingsModel> _appSettings;

        /// <summary>
        /// constructor injection of MasterreportsContext in order to execute _context.SqlQueryAsync()
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appSettingsOptions"></param>
        /// <param name="configuration"></param>
        public UserPatientCacheHelper(IConfiguration configuration, IOptions<CustomAppSettingsModel> appSettingsOptions, MasterreportsContext context)
        {
            _context = context;
            _configuration = configuration;
            //https://www.bing.com/ck/a?!&&p=9f40ffcae2103a86JmltdHM9MTY2MjQyMjQwMCZpZ3VpZD0yMGE2ODFkNi05OTNiLTZiN2YtMThjYS05M2MxOThiZjZhNTEmaW5zaWQ9NTQ0MA&ptn=3&hsh=3&fclid=20a681d6-993b-6b7f-18ca-93c198bf6a51&u=a1aHR0cHM6Ly93d3cuYy1zaGFycGNvcm5lci5jb20vYXJ0aWNsZS9yZWFkaW5nLXZhbHVlcy1mcm9tLWFwcHNldHRpbmdzLWpzb24taW4tYXNwLW5ldC1jb3JlLyM6fjp0ZXh0PVRoZXJlJTIwYXJlJTIwdHdvJTIwbWV0aG9kcyUyMHRvJTIwcmV0cmlldmUlMjBvdXIlMjB2YWx1ZXMlMkMsYXJlJTIwZ2V0dGluZyUyMGFub3RoZXIlMjBzZWN0aW9uJTIwdGhhdCUyMGNvbnRhaW5zJTIwdGhlJTIwdmFsdWUu&ntb=1
            _appSettings = appSettingsOptions;
        }

        /// <summary>
        /// Do not use generic repository, instead use MastReportsContext to execute stored procedure to get user access levels
        /// </summary>
        /// <param name="networkID"></param>
        /// <returns></returns>
        public async Task<List<MastUserDTO>> GetUserAccessLevels(string networkID)
        {
            string userName = CleanUserName(networkID); //use network ID without domain
            List<MastUserDTO> userAccessLevels = new();

            SqlParameter[] paramNetworkID = new SqlParameter[]
            {
        new SqlParameter(){
          ParameterName = "@UserName",
          SqlDbType = System.Data.SqlDbType.VarChar,
          Direction = System.Data.ParameterDirection.Input,
          Value = userName
        }
            };

            //use dbContext extension method
            var userPermission = await _context.SqlQueryAsync<uspVSSCMain_SelectAccessInformationFromNSSDResult>(
              $"execute [Apps].[uspVSSCMain_SelectAccessInformationFromNSSD] @UserName", paramNetworkID);
            var distinctFacilities = userPermission
              .Where(x => !string.IsNullOrEmpty(x.Facility)).Distinct()
              .Select(x => HydrateDTO.HydrateUser(x)).ToList();

            return distinctFacilities;
        }

        /// <summary>
        /// get patients from Health Factor using generic IFODPatientRepository
        /// </summary>
        /// <param name="_patientRepository"></param>
        /// <param name="networkName">optional</param>
        /// <param name="criteria">optional</param>
        /// <param name="orderBy">optional</param>
        /// <param name="pageNumber">optional</param>
        /// <param name="pageSize">optional</param>
        /// <param name="patientID">optional, used by individual patient search only</param>
        /// <returns></returns>
        public async Task<List<PatientDTO>> GetPatients(IFSODPatientRepository _patientRepository, string networkName, string criteria, string orderBy, int pageNumber, int pageSize, string patientID)
        {
            List<PatientDTO> patients = null;
            //get user access level from external stored proc
            var distinctUserFacilities = await DistinctUserFacilities(networkName);

            if (distinctUserFacilities != null && distinctUserFacilities.Any())
            {
                List<string> userFacilitySta3 = distinctUserFacilities.Select(x => x.Facility).Distinct().ToList();

                string cacheKey = criteria;
                if (string.IsNullOrEmpty(criteria))
                    cacheKey = "No Criteria";

                int totalViewablePatientCount = 0;
                string searchCriteriaType = string.Empty;
                int numericCriteria = -1;

                if (string.IsNullOrEmpty(criteria))
                    searchCriteriaType = "none";
                else if (int.TryParse(criteria, out numericCriteria))
                    searchCriteriaType = "numeric";
                else
                    searchCriteriaType = "non-numeric";

                bool patientsFound = false;
                List<int> targetQuarters = GetQuarterOfInterest();
                IEnumerable<FSODPatient> viewablePatients = null;
                foreach (int thisPeriod in targetQuarters)
                {
                    if (patientsFound) break; //break out foreach()
                    {
                        switch (searchCriteriaType)
                        {
                            case "none":
                                {
                                    var rawPatients = await _patientRepository
                                      .FindByCondition(p => thisPeriod == p.FiscalPeriodInt).OrderBy(x => x.Name).ToListAsync();

                                    if (rawPatients.Any())
                                    {
                                        //applying facility filter cannot be done in previous SQL server side query
                                        //it must be filtered in IIS memory after the last ToListAsync()
                                        viewablePatients = rawPatients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));

                                        totalViewablePatientCount = viewablePatients.Count();

                                        if (pageNumber <= 0)
                                        {
                                            viewablePatients = viewablePatients.Take(pageSize);
                                        }
                                        else
                                        {
                                            viewablePatients = viewablePatients.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                                        }

                                        patients = viewablePatients.Select(p => HydrateDTO.HydratePatient(p)).ToList();

                                        patientsFound = true;
                                    }
                                }
                                break; //break case
                            case "numeric":
                                {
                                    var rawPatients = await _patientRepository.FindByCondition(p =>
                                                      (thisPeriod == p.FiscalPeriodInt) &&
                                                      (
                                                        p.ADMParent_Key == numericCriteria ||
                                                        p.Sta6aKey == numericCriteria ||
                                                        p.bedsecn == numericCriteria ||
                                                        p.FiscalPeriodInt == numericCriteria ||
                                                        p.PTFSSN.Contains(criteria) ||
                                                        p.Facility.Contains(criteria) ||
                                                        p.VISN.Contains(criteria)
                                                      )
                                                    ).OrderBy(x => x.Name).ToListAsync();

                                    if (rawPatients.Any())
                                    {
                                        //facility filter cannot be done in previous SQL server side LINQ. must be filtered in IIS memory 
                                        viewablePatients = rawPatients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));

                                        totalViewablePatientCount = viewablePatients.Count();
                                        patients = viewablePatients.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                                          .Select(p => HydrateDTO.HydratePatient(p)).ToList();

                                        patientsFound = true;
                                    }
                                }
                                break; //break case
                            case "non-numeric":
                                {
                                    var rawPatients = await _patientRepository.FindByCondition(p =>
                                                      (thisPeriod == p.FiscalPeriodInt) &&
                                                      (
                                                        p.Name.Contains(criteria) || p.PTFSSN.Contains(criteria) || p.Facility.Contains(criteria) ||
                                                        p.VISN.Contains(criteria) || p.District.Contains(criteria) || p.FiscalPeriod.Contains(criteria)
                                                      )
                                                    ).OrderBy(x => x.Name).ToListAsync();

                                    if (rawPatients.Any())
                                    {
                                        //facility filter cannot be done in previous SQL server side query. must be filtered by IIS memory 
                                        viewablePatients = rawPatients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));


                                        totalViewablePatientCount = viewablePatients.Count();

                                        patients = viewablePatients.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                                          .Select(p => HydrateDTO.HydratePatient(p)).ToList();

                                        patientsFound = true;
                                    }
                                }
                                break;
                        }

                        if (!string.IsNullOrEmpty(patientID))
                        viewablePatients = viewablePatients.Where(x => x.PTFSSN == patientID);
                   }
                }
                //PatientSearchResultDTO meta = new() { Patients = patients.ToList(), TotalCount = totalViewablePatientCount };
                //return (meta);
            }
            return patients;
        }

        /// <summary>
        /// get patients from Treating Specialty using ITreatingSpecialtyPatientRepository
        /// </summary>
        /// <param name="_treatingSpecialtyPatientRepository"></param>
        /// <param name="networkName"></param>
        /// <param name="criteria"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="patientID"></param>
        /// <returns></returns>
        public async Task<List<PatientDTOTreatingSpecialty>> GetPatients(ITreatingSpecialtyPatientRepository _treatingSpecialtyPatientRepository, string networkName, string criteria, string orderBy, int pageNumber, int pageSize, string patientID)
        {
            List<PatientDTOTreatingSpecialty> patients = null;
            var distinctUserFacilities = await GetUserAccessLevels(networkName);

            if (distinctUserFacilities != null && distinctUserFacilities.Any())
            {
                List<string> userFacilitySta3 = distinctUserFacilities.Select(x => x.Facility).Distinct().ToList();

                string cacheKey = criteria;
                if (string.IsNullOrEmpty(criteria))
                    cacheKey = "No Criteria";

                int facilityPatientsCount = 0;
                string searchCriteriaType = string.Empty;
                int numericCriteria = -1;

                if (string.IsNullOrEmpty(criteria))
                    searchCriteriaType = "none";
                else if (int.TryParse(criteria, out numericCriteria))
                    searchCriteriaType = "numeric";
                else
                    searchCriteriaType = "non-numeric";

                string testSite = _appSettings.Value.TestSite;
                string[] bedSection = _appSettings.Value.BedSection;
                string[] c_los = _appSettings.Value.C_LOS;
                string[] staType = _appSettings.Value.STAType;
                if (!string.IsNullOrEmpty(testSite))
                {
                    //use default test site patients
                }
                else
                {
                    //cannot filter the p.bsta6 at server side, so use ToListAsync() to client list
                    var facilityPatients = await _treatingSpecialtyPatientRepository.FindAll().ToListAsync();

                    facilityPatients = facilityPatients.FindAll(p => userFacilitySta3.Any(f => f.Contains(p.bsta6a) || p.bsta6a.Contains(f)));
                    //var facilityPatients = viewablePatients.FindAll(p => p.bsta6a.Contains("501"));

                    if (!string.IsNullOrEmpty(patientID))
                    {
                        facilityPatients = facilityPatients.FindAll(p => p.scrssn.Value.ToString() == patientID || p.PatientICN == patientID);
                    }

                    if (facilityPatients.Any())
                    {
                        List<vTreatingSpecialtyRecent3Yrs> filteredPatients = new List<vTreatingSpecialtyRecent3Yrs>();

                        switch (searchCriteriaType)
                        {
                            case "none":
                                //get single patient if patientID is not blank
                                facilityPatientsCount = facilityPatients.Count();
                                filteredPatients = facilityPatients;
                                break; //break case
                            case "numeric":
                                filteredPatients = facilityPatients.FindAll(p =>
                                                    p.scrssn == numericCriteria ||
                                                    p.bedsecn == numericCriteria ||
                                                    p.bsta6a.Contains(numericCriteria.ToString())
                                                );

                                if (filteredPatients.Any())
                                {
                                    facilityPatientsCount = facilityPatients.Count();
                                }
                                break; //break case
                            case "non-numeric":
                                filteredPatients = facilityPatients.FindAll(p =>
                                                    p.Last_Name.Contains(criteria) ||
                                                    p.First_Name.Contains(criteria) ||
                                                    p.scrssn.ToString().Contains(criteria) ||
                                                    p.PatientICN.Contains(criteria)
                                                );

                                if (filteredPatients.Any())
                                {
                                    facilityPatientsCount = filteredPatients.Count();
                                }
                                break;
                        }

                        filteredPatients = filteredPatients.OrderBy(o => o.PatientName).ToList();
                        patients = filteredPatients.Select(p => HydrateDTO.HydrateTreatingSpecialtyPatient(p)).ToList();

                        if (pageNumber <= 0)
                            patients = patients.Take(pageSize).ToList();
                        else
                            patients = patients.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }
                }

                //PatientSearchResultDTO meta = new() { Patients = patients.ToList(), TotalCount = totalViewablePatientCount };
                //return (meta);
            }
            return patients;
        }

        /// <summary>
        /// get patient by Episode ID from Health Factor
        /// </summary>
        /// <param name="_episodeRepository"></param>
        /// <param name="_patientRepository"></param>
        /// <param name="episodeID"></param>
        /// <returns></returns>
        public async Task<PatientDTO> GetPatientByEpisode(IEpisodeOfCareRepository _episodeRepository, IFSODPatientRepository _patientRepository, int episodeID)
        {
            List<PatientDTO> patients = null;
            var thisEpisode = _episodeRepository.FindByCondition(x => x.EpisodeOfCareID == episodeID).FirstOrDefault();
            if (thisEpisode != null)
            {
                foreach (int thisPeriod in GetQuarterOfInterest())
                {
                    patients = await _patientRepository.FindByCondition(p => thisPeriod == p.FiscalPeriodInt && p.PTFSSN == thisEpisode.PatientICNFK)
                              .Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

                    if (patients.Any())
                    {
                        break;
                    }
                }

                //patient not in the last 4 consecutive quarters, so just search by Patient ICN
                patients = await _patientRepository.FindByCondition(p => p.PTFSSN == thisEpisode.PatientICNFK).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
            }

            return patients.FirstOrDefault();
        }

        /// <summary>
        /// get patient by Episode ID from Treating Specialty
        /// </summary>
        /// <param name="_episodeRepository"></param>
        /// <param name="_patientRepository"></param>
        /// <param name="episodeID"></param>
        /// <returns></returns>
        public async Task<PatientDTOTreatingSpecialty> GetPatientByEpisode(IEpisodeOfCareRepository _episodeRepository, ITreatingSpecialtyPatientRepository _patientRepository, int episodeID)
        {
            List<PatientDTOTreatingSpecialty> patients = null;
            var thisEpisode = _episodeRepository.FindByCondition(x => x.EpisodeOfCareID == episodeID).FirstOrDefault();
            if (thisEpisode != null)
            {
                foreach (int thisPeriod in GetQuarterOfInterest())
                {
                    patients = await _patientRepository.FindByCondition(p => p.PatientICN == thisEpisode.PatientICNFK)
                              .Select(p => HydrateDTO.HydrateTreatingSpecialtyPatient(p)).ToListAsync();

                    if (patients.Any())
                    {
                        break;
                    }
                }

                //patient not in the last 4 consecutive quarters, so just search by Patient ICN
                patients = await _patientRepository.FindByCondition(p => p.PatientICN == thisEpisode.PatientICNFK).Select(p => HydrateDTO.HydrateTreatingSpecialtyPatient(p)).ToListAsync();
            }

            return patients.FirstOrDefault();
        }

        private async Task<List<MastUserDTO>> DistinctUserFacilities(string networkName)
        {
            var distinctUserFacilities = await GetUserAccessLevels(networkName);

            if (!distinctUserFacilities.Any())
            {
                return null;
            }
            else
            {
                return distinctUserFacilities;
            }
        }

        private static List<int> GetQuarterOfInterest()
        {
            DateTime today = DateTime.Today;

            int currentYear = today.Year;
            int lastYear = currentYear - 1;
            int nextYear = currentYear + 1;

            int[] quarters = new int[] { 2, 2, 2, 3, 3, 3, 4, 4, 4, 1, 1, 1 };
            int currentQTableNumber = currentYear, minus1QTableNumber = currentYear, minus2QTableNumber = currentYear, minus3QTableNumber = currentYear;

            int currentQuarter = quarters[today.Month - 1];
            switch (currentQuarter)
            {
                case 1:
                    currentQTableNumber = (nextYear * 10) + 1;
                    minus1QTableNumber = (currentYear * 10) + 4;
                    minus2QTableNumber = (currentYear * 10) + 3;
                    minus3QTableNumber = (currentYear * 10) + 2;
                    break;
                case 2:
                    currentQTableNumber = (currentYear * 10) + 2;
                    minus1QTableNumber = (currentYear * 10) + 1;
                    minus2QTableNumber = (lastYear * 10) + 4;
                    minus3QTableNumber = (lastYear * 10) + 3;
                    break;
                case 3:
                    currentQTableNumber = (currentYear * 10) + 3;
                    minus1QTableNumber = (currentYear * 10) + 2;
                    minus2QTableNumber = (currentYear * 10) + 1;
                    minus3QTableNumber = (lastYear * 10) + 4;
                    break;
                case 4:
                    currentQTableNumber = (currentYear * 10) + 4;
                    minus1QTableNumber = (currentYear * 10) + 3;
                    minus2QTableNumber = (currentYear * 10) + 2;
                    minus3QTableNumber = (currentYear * 10) + 1;
                    break;
            }

            //the fiscalPeriodOfInterest is a numeric dentifier that is made up of FY and quarter in 5 digits format, thus the multiplier of 10
            //to get the base than add the quarter number
            List<int> fiscalPeriodsOfInterest = new()
      {
        currentQTableNumber,
        minus1QTableNumber,
        minus2QTableNumber,
        minus3QTableNumber
      };
            return fiscalPeriodsOfInterest;
        }

        /// <summary>
        /// this should be in a utility library
        /// </summary>
        /// <param name="networkID"></param>
        /// <returns></returns>
        private static string CleanUserName(string networkID)
        {
            string networkName = networkID;
            if (string.IsNullOrEmpty(networkName))
                return null;
            else
            {
                if (networkName.Contains('\\') || networkName.Contains("%2F") || networkName.Contains("//"))
                {
                    String[] separator = { "\\", "%2F", "//" };
                    var networkNameWithDomain = networkName.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    if (networkNameWithDomain.Length > 0)
                        networkName = networkNameWithDomain[1];
                    else
                        networkName = networkNameWithDomain[0];
                }
                return networkName;
            }
        }
    }
}
