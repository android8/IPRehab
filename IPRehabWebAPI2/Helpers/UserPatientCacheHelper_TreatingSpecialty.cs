using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using PatientModel_TreatingSpecialty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Helpers
{
    public class UserPatientCacheHelper_TreatingSpecialty : IUserPatientCacheHelper_TreatingSpecialty
    {
        protected readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly ITreatingSpecialtyDirectPatientRepository _treatingSpecialtyPatientRepository;
        private readonly IEpisodeOfCareRepository _episodeRepository;

        /// <summary>
        /// constructor injection of MasterreportsContext in order to execute _context.SqlQueryAsync()
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="memoryCache"></param>
        /// <param name="episodeRepository"></param>
        /// <param name="treatingSpecialtyPatientRepository"></param>
        public UserPatientCacheHelper_TreatingSpecialty(IConfiguration configuration, IMemoryCache memoryCache,
            IEpisodeOfCareRepository episodeRepository, ITreatingSpecialtyDirectPatientRepository treatingSpecialtyPatientRepository)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            _episodeRepository = episodeRepository;
            _treatingSpecialtyPatientRepository = treatingSpecialtyPatientRepository;
            //https://www.bing.com/ck/a?!&&p=9f40ffcae2103a86JmltdHM9MTY2MjQyMjQwMCZpZ3VpZD0yMGE2ODFkNi05OTNiLTZiN2YtMThjYS05M2MxOThiZjZhNTEmaW5zaWQ9NTQ0MA&ptn=3&hsh=3&fclid=20a681d6-993b-6b7f-18ca-93c198bf6a51&u=a1aHR0cHM6Ly93d3cuYy1zaGFycGNvcm5lci5jb20vYXJ0aWNsZS9yZWFkaW5nLXZhbHVlcy1mcm9tLWFwcHNldHRpbmdzLWpzb24taW4tYXNwLW5ldC1jb3JlLyM6fjp0ZXh0PVRoZXJlJTIwYXJlJTIwdHdvJTIwbWV0aG9kcyUyMHRvJTIwcmV0cmlldmUlMjBvdXIlMjB2YWx1ZXMlMkMsYXJlJTIwZ2V0dGluZyUyMGFub3RoZXIlMjBzZWN0aW9uJTIwdGhhdCUyMGNvbnRhaW5zJTIwdGhlJTIwdmFsdWUu&ntb=1
        }

        /// <summary>
        /// get patients from Treating Specialty using ITreatingSpecialtyPatientRepository
        /// </summary>
        /// <param name="distinctUserFacilities"></param>
        /// <param name="criteria"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="patientID"></param>
        /// <returns></returns>
        public async Task<List<PatientDTOTreatingSpecialty>> GetPatients(
            List<MastUserDTO> distinctUserFacilities, string criteria, string orderBy, int pageNumber, int pageSize, string patientID)
        {
            if (distinctUserFacilities == null || !distinctUserFacilities.Any())
                return null;    //no access

            //cannot filter the p.bsta6 at server side, so use ToListAsync() to client list
            var thisFacilityPatients = await GetThisFacilityPatients(distinctUserFacilities);
            if (thisFacilityPatients == null || !thisFacilityPatients.Any())
                return null;    //no patient in the permitted facilities list

            //when patientID is not blank then return a list containing single patient
            if (!string.IsNullOrEmpty(patientID))
            {
                thisFacilityPatients = thisFacilityPatients.Where(p =>
                    p.ScrSsnt == patientID ||
                    p.Realssn == patientID ||
                    p.Scrnum == int.Parse(patientID) ||
                    p.ScrssnT == int.Parse(patientID) ||
                    p.PatientIcn == patientID
                ).ToList();

                if (thisFacilityPatients == null || !thisFacilityPatients.Any())
                    return null;    //no patient matches the patientID in permitted facilities list
            }
            else
            {
                string searchCriteriaType = string.Empty;
                int numericCriteria = -1;
                DateTime dateCriteria = DateTime.MinValue;

                if (!string.IsNullOrEmpty(criteria))
                {
                    if (int.TryParse(criteria, out numericCriteria))
                        searchCriteriaType = "isNumeric";
                    else if (DateTime.TryParse(criteria, out dateCriteria))
                        searchCriteriaType = "isDate";

                    switch (searchCriteriaType)
                    {
                        case "isNumeric":
                            thisFacilityPatients = thisFacilityPatients.Where(p =>
                                p.ScrssnT == numericCriteria ||
                                p.Scrnum == numericCriteria ||
                                p.Bedsecn == numericCriteria ||
                                p.Bsta6a.Contains(numericCriteria.ToString(), StringComparison.OrdinalIgnoreCase)
                            ).ToList();

                            break; //break case

                        case "isDate":
                            thisFacilityPatients = thisFacilityPatients.Where(p =>
                                p.Admitday.Value == dateCriteria ||
                                p.DoB.Value == dateCriteria
                            ).ToList();
                            break;

                        default:
                            criteria = criteria.Trim().ToLower();
                            thisFacilityPatients = thisFacilityPatients.Where(p =>
                                p.PatientName.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                p.LastName.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                p.FirstName.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                p.PatientIcn.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                p.ScrSsnt.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                p.Realssn.Contains(criteria, StringComparison.OrdinalIgnoreCase)
                            ).ToList();
                            break;
                    }
                }

                if (thisFacilityPatients == null || !thisFacilityPatients.Any())
                    return null;    //no patient matches the search criteria in permitted facilities list
            }

            //don't sort here it will fail
            return ConvertToPatientDTO(thisFacilityPatients, pageNumber, pageSize);
        }

        /// <summary>
        /// get patient by Episode ID from Treating Specialty
        /// </summary>
        /// <param name="episodeID"></param>
        /// <returns></returns>
        public async Task<PatientDTOTreatingSpecialty> GetPatientByEpisode(int episodeID)
        {
            PatientDTOTreatingSpecialty patient = null;
            var thisEpisode = _episodeRepository.FindByCondition(x => x.EpisodeOfCareID == episodeID).FirstOrDefault();

            if (thisEpisode != null)
            {
                var allFacilityPatients = await GetAllFacilityPatients();

                var patientInThisEpisode = allFacilityPatients.Where(p =>
                    p.ScrSsnt == thisEpisode.PatientICNFK).FirstOrDefault();

                if (patientInThisEpisode != null)
                    patient = HydrateDTO.HydrateTreatingSpecialtyPatient(patientInThisEpisode);
            }

            return patient;
        }

        //  private List<int> GetQuarterOfInterest()
        //  {
        //      DateTime today = DateTime.Today;

        //      int currentYear = today.Year;
        //      int lastYear = currentYear - 1;
        //      int nextYear = currentYear + 1;

        //      int[] quarters = new int[] { 2, 2, 2, 3, 3, 3, 4, 4, 4, 1, 1, 1 };
        //      int currentQTableNumber = currentYear, minus1QTableNumber = currentYear, minus2QTableNumber = currentYear, minus3QTableNumber = currentYear;

        //      int currentQuarter = quarters[today.Month - 1];
        //      switch (currentQuarter)
        //      {
        //          case 1:
        //              currentQTableNumber = (nextYear * 10) + 1;
        //              minus1QTableNumber = (currentYear * 10) + 4;
        //              minus2QTableNumber = (currentYear * 10) + 3;
        //              minus3QTableNumber = (currentYear * 10) + 2;
        //              break;
        //          case 2:
        //              currentQTableNumber = (currentYear * 10) + 2;
        //              minus1QTableNumber = (currentYear * 10) + 1;
        //              minus2QTableNumber = (lastYear * 10) + 4;
        //              minus3QTableNumber = (lastYear * 10) + 3;
        //              break;
        //          case 3:
        //              currentQTableNumber = (currentYear * 10) + 3;
        //              minus1QTableNumber = (currentYear * 10) + 2;
        //              minus2QTableNumber = (currentYear * 10) + 1;
        //              minus3QTableNumber = (lastYear * 10) + 4;
        //              break;
        //          case 4:
        //              currentQTableNumber = (currentYear * 10) + 4;
        //              minus1QTableNumber = (currentYear * 10) + 3;
        //              minus2QTableNumber = (currentYear * 10) + 2;
        //              minus3QTableNumber = (currentYear * 10) + 1;
        //              break;
        //      }

        //      //the fiscalPeriodOfInterest is a numeric dentifier that is made up of FY and quarter in 5 digits format, thus the multiplier of 10
        //      //to get the base than add the quarter number
        //      List<int> fiscalPeriodsOfInterest = new()
        //{
        //  currentQTableNumber,
        //  minus1QTableNumber,
        //  minus2QTableNumber,
        //  minus3QTableNumber
        //};
        //      return fiscalPeriodsOfInterest;
        //  }

        /// <summary>
        /// this should be in a utility library
        /// </summary>
        /// <param name="networkID"></param>
        /// <returns></returns>
        public string CleanUserName(string networkID)
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

        /// <summary>
        /// get the treating specialty patients cohort base from session, otherwise get them from the WebAPI
        /// </summary>
        /// <returns></returns>
        public async Task<List<RptRehabDetails>> GetAllFacilityPatients()
        {
            //List<RptRehabDetails> allPatientsFromCache = null;
            //var tmp = _memoryCache.Get(CacheKeys.CacheKeyAllPatients);
            //if (tmp != null)
            //    allPatientsFromCache = (List<RptRehabDetails>)tmp;

            //get all facilities patients from the memory cache
            var allPatientsFromCache = _memoryCache.Get<List<RptRehabDetails>>(CacheKeys.CacheKeyAllPatients);

            if (allPatientsFromCache != null && allPatientsFromCache.Any())
            {
                //renew cache for 24 hours
                _memoryCache.Set(CacheKeys.CacheKeyAllPatients_TreatingSpeciality, allPatientsFromCache, TimeSpan.FromDays(1));
                return allPatientsFromCache;
            }
            else
            {
                //directly get from BI13 without user facilities filter
                var listOfRptRehabDetails = await _treatingSpecialtyPatientRepository.FindAll().ToListAsync();
                //update cache for 24 hours
                _memoryCache.Set(CacheKeys.CacheKeyAllPatients_TreatingSpeciality, listOfRptRehabDetails, TimeSpan.FromDays(1));
                return listOfRptRehabDetails;
            }
        }

        public async Task<List<RptRehabDetails>> GetThisFacilityPatients(List<MastUserDTO> distinctUserFacilities)
        {
            //get smaller set of this facility patients from the memory cache
            string cacheKeyOfThisFacility = $"{CacheKeys.CacheKeyThisFacilityPatients}_{distinctUserFacilities.First().Facility}";

            var thisFacilityPatientsInCache = _memoryCache.Get<List<RptRehabDetails>>(cacheKeyOfThisFacility);

            if (thisFacilityPatientsInCache != null && thisFacilityPatientsInCache.Any())
            {
                //get patient in current user facilities with cache key cacheKeyOfThisFacility
                return thisFacilityPatientsInCache;
            }
            else
            {
                //get all facilities patients
                var allFacilityPatients = await GetAllFacilityPatients();

                List<RptRehabDetails> patientsInCurrentUserFacility = new();
                List<RptRehabDetails> thisFacilityPatients = null;

                //get from sql view with user facilities filter on bsta6
                //this cannot be done at server side and must use {column}.StartWith()
                foreach (var fac in distinctUserFacilities)
                {
                    thisFacilityPatients = allFacilityPatients
                        .Where(p => p.Bsta6a.StartsWith(fac.Facility.Substring(0, 3))).ToList();

                    //thisFacilityPatients = allFacilityPatients
                    //    .Where(p => fac.Facility == p.bsta6a).ToList();

                    if (thisFacilityPatients != null && thisFacilityPatients.Any())
                    {
                        patientsInCurrentUserFacility.AddRange(thisFacilityPatients);
                    }
                }

                //update cache for 24 hours
                _memoryCache.Set(cacheKeyOfThisFacility, patientsInCurrentUserFacility, TimeSpan.FromDays(1));

                return patientsInCurrentUserFacility;
            }
        }

        /// <summary>
        /// convert RptRehabDetails from BI13 to DTO
        /// </summary>
        /// <param name="thisFacilityPatientsSorted"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private List<PatientDTOTreatingSpecialty> ConvertToPatientDTO(IEnumerable<RptRehabDetails> thisFacilityPatientsSorted, int pageNumber, int pageSize)
        {
            List<PatientDTOTreatingSpecialty> patients = new();

            var distinctedPatientsInThisFacility = thisFacilityPatientsSorted.DistinctBy(x => x.PatientIcn).ToList();
            foreach (var thisDistinctP in distinctedPatientsInThisFacility)
            {
                //the annonymous thisDistinctP cannot be hydrated, so get vTreatingSpecialtyRecent3Yrs type
                var hydratedPatient = HydrateDTO.HydrateTreatingSpecialtyPatient(thisDistinctP);

                var admissions = thisFacilityPatientsSorted.Where(p => p.PatientIcn == thisDistinctP.PatientIcn).Select(p => p.Admitday.Value).ToList();

                if (admissions.Any())
                {
                    hydratedPatient.AdmitDates.Clear();
                    hydratedPatient.AdmitDates.AddRange(admissions);
                }
                patients.Add(hydratedPatient);
            }

            if (pageNumber <= 0)
                patients = patients.Take(pageSize).ToList();
            else
                patients = patients.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return patients;
        }
    }
}
