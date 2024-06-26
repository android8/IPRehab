﻿using IPRehabModel;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Helpers
{
    public class UserPatientCacheHelper : IUserPatientCacheHelper
    {
        protected readonly MasterreportsContext _masterReportsContext;
        protected readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly ITreatingSpecialtyPatientRepository _treatingSpecialtyPatientRepository;
        private readonly IEpisodeOfCareRepository _episodeRepository;
        /// <summary>
        /// constructor injection of MasterreportsContext in order to execute _context.SqlQueryAsync()
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="memoryCache"></param>
        /// <param name="masterReportsContext"></param>
        /// <param name="episodeRepository"></param>
        /// <param name="treatingSpecialtyPatientRepository"></param>
        public UserPatientCacheHelper(IConfiguration configuration, IMemoryCache memoryCache, MasterreportsContext masterReportsContext,
            IEpisodeOfCareRepository episodeRepository, ITreatingSpecialtyPatientRepository treatingSpecialtyPatientRepository)
        {
            _masterReportsContext = masterReportsContext;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _episodeRepository = episodeRepository;
            _treatingSpecialtyPatientRepository = treatingSpecialtyPatientRepository;
            //https://www.bing.com/ck/a?!&&p=9f40ffcae2103a86JmltdHM9MTY2MjQyMjQwMCZpZ3VpZD0yMGE2ODFkNi05OTNiLTZiN2YtMThjYS05M2MxOThiZjZhNTEmaW5zaWQ9NTQ0MA&ptn=3&hsh=3&fclid=20a681d6-993b-6b7f-18ca-93c198bf6a51&u=a1aHR0cHM6Ly93d3cuYy1zaGFycGNvcm5lci5jb20vYXJ0aWNsZS9yZWFkaW5nLXZhbHVlcy1mcm9tLWFwcHNldHRpbmdzLWpzb24taW4tYXNwLW5ldC1jb3JlLyM6fjp0ZXh0PVRoZXJlJTIwYXJlJTIwdHdvJTIwbWV0aG9kcyUyMHRvJTIwcmV0cmlldmUlMjBvdXIlMjB2YWx1ZXMlMkMsYXJlJTIwZ2V0dGluZyUyMGFub3RoZXIlMjBzZWN0aW9uJTIwdGhhdCUyMGNvbnRhaW5zJTIwdGhlJTIwdmFsdWUu&ntb=1
        }

        /// <summary>
        /// Do not use generic repository, instead use MastReportsContext to execute stored procedure to get user access levels
        /// </summary>
        /// <param name="networkID"></param>
        /// <returns></returns>
        public async Task<List<MastUserDTO>> GetUserAccessLevels(string networkID)
        {
            string userName = CleanUserName(networkID); //use network ID without domain

            List<MastUserDTO> thisUserAccessLevel = _memoryCache.Get<List<MastUserDTO>>($"{CacheKeys.CacheKeyThisUserAccessLevel}_{networkID}");

            if (thisUserAccessLevel != null && thisUserAccessLevel.Any())
            {
                return thisUserAccessLevel;
            }
            else
            {
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
                var userPermission = await _masterReportsContext.SqlQueryAsync<uspVSSCMain_SelectAccessInformationFromNSSDResult>(
                  $"execute [Apps].[uspVSSCMain_SelectAccessInformationFromNSSD] @UserName", paramNetworkID);

                if (userPermission == null || !userPermission.Any())
                    return null;    //no permssion

                var distinctFacilities = userPermission
                  .Where(x => !string.IsNullOrEmpty(x.Facility)).Distinct()
                  .Select(x => HydrateDTO.HydrateUser(x)).ToList();

                if (distinctFacilities == null || !distinctFacilities.Any())
                    return null;    //no permitted facilities

                //using var MasterReportsDb = new MasterreportsContext();
                //var procedure = new MasterreportsContextProcedures(MasterReportsDb);
                //var accessLevel = procedure.uspVSSCMain_SelectAccessInformationFromNSSDAsync(userName);

                _memoryCache.Set($"{CacheKeys.CacheKeyThisUserAccessLevel}_{networkID}", distinctFacilities, TimeSpan.FromHours(2));
                return distinctFacilities;
            }
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
                thisFacilityPatients = thisFacilityPatients.Where(p => p.ScrSSNT.Trim() == patientID).ToList();

                if (thisFacilityPatients == null || !thisFacilityPatients.Any())
                    return null;    //no patient matches the patientID in permitted facilities list
            }
            else
            {
                string searchCriteriaType = string.Empty;
                int numericCriteria = -1;

                if (string.IsNullOrEmpty(criteria))
                    searchCriteriaType = "none";
                else if (int.TryParse(criteria, out numericCriteria))
                    searchCriteriaType = "numeric";
                else
                    searchCriteriaType = "non-numeric";

                switch (searchCriteriaType)
                {
                    case "numeric":
                        thisFacilityPatients = thisFacilityPatients.Where(p =>
                                            p.scrssn == numericCriteria ||
                                            p.bedsecn == numericCriteria ||
                                            p.bsta6a.Contains(numericCriteria.ToString(), StringComparison.OrdinalIgnoreCase)
                                        ).ToList();

                        break; //break case

                    case "non-numeric":
                        criteria = criteria.Trim().ToLower();
                        thisFacilityPatients = thisFacilityPatients.Where(p =>
                                            p.Last_Name.Trim().ToLower().Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                            p.First_Name.Trim().ToLower().Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                            p.ScrSSNT.Trim().Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                            p.RealSSN.Value.ToString().Trim().Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                                            p.PatientICN.Trim().Contains(criteria, StringComparison.OrdinalIgnoreCase)
                                        ).ToList();
                        break;
                }

                if (thisFacilityPatients == null || !thisFacilityPatients.Any())
                    return null;    //no patient matches the search criteria in permitted facilities list
            }

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
                    p.ScrSSNT.ToString() == thisEpisode.PatientICNFK).FirstOrDefault();

                if (patientInThisEpisode != null)
                    patient = HydrateDTO.HydrateTreatingSpecialtyPatient(patientInThisEpisode);
            }

            return patient;
        }

        //  private static List<int> GetQuarterOfInterest()
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
                return string.Empty;
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
        /// get the treating speciatlty patients cohort base from session, otherwise get them from the WebAPI
        /// </summary>
        /// <returns></returns>
        public async Task<List<vTreatingSpecialtyRecent3Yrs>> GetAllFacilityPatients()
        {
            //List<vTreatingSpecialtyRecent3Yrs> allPatientsFromCache = null;
            //var tmp = _memoryCache.Get(CacheKeys.CacheKeyAllPatients);
            //if (tmp != null)
            //    allPatientsFromCache = (List<vTreatingSpecialtyRecent3Yrs>)tmp;

            //get all facilities patients from the memory cache
            var allPatientsFromCache = _memoryCache.Get<List<vTreatingSpecialtyRecent3Yrs>>(CacheKeys.CacheKeyAllPatients);

            if (allPatientsFromCache != null && allPatientsFromCache.Any())
            {
                //renew cache for 24 hours
                _memoryCache.Set(CacheKeys.CacheKeyAllPatients, allPatientsFromCache, TimeSpan.FromDays(1));
                return allPatientsFromCache;
            }
            else
            {
                //get from sql view without user facilities filter
                var allPatientsFromSqlView = await _treatingSpecialtyPatientRepository.FindAll().ToListAsync();
                //update cache for 24 hours
                _memoryCache.Set(CacheKeys.CacheKeyAllPatients, allPatientsFromSqlView, TimeSpan.FromDays(1));
                return allPatientsFromSqlView;
            }
        }

        public async Task<List<vTreatingSpecialtyRecent3Yrs>> GetThisFacilityPatients(List<MastUserDTO> distinctUserFacilities)
        {
            //get smaller set of this facility patients from the memory cache
            string cacheKeyOfThisFacility = $"{CacheKeys.CacheKeyThisFacilityPatients}_{distinctUserFacilities.First().Facility}";

            var thisFacilityPatientsInCache = _memoryCache.Get<List<vTreatingSpecialtyRecent3Yrs>>(cacheKeyOfThisFacility);

            if (thisFacilityPatientsInCache != null && thisFacilityPatientsInCache.Any())
            {
                //get patient in current user facilities with cache key cacheKeyOfThisFacility
                return thisFacilityPatientsInCache;
            }
            else
            {
                //get all facilities patients
                var allFacilityPatients = await GetAllFacilityPatients();

                List<vTreatingSpecialtyRecent3Yrs> patientsInCurrentUserFacility = new();
                List<vTreatingSpecialtyRecent3Yrs> thisFacilityPatients = null;

                //get from sql view with user facilities filter on bsta6
                //this cannot be done at server side and must use {column}.StartWith()
                foreach (var fac in distinctUserFacilities)
                {
                    thisFacilityPatients = allFacilityPatients
                        .Where(p => p.bsta6a.StartsWith(fac.Facility.Substring(0, 3))).ToList();

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

        private List<PatientDTOTreatingSpecialty> ConvertToPatientDTO(IEnumerable<vTreatingSpecialtyRecent3Yrs> thisFacilityPatients, int pageNumber, int pageSize)
        {
            List<PatientDTOTreatingSpecialty> patients = new();

            var distinctedPatientsInThisFacility = thisFacilityPatients.DistinctBy(x => x.PatientICN).ToList();
            foreach (var thisDistinctP in distinctedPatientsInThisFacility)
            {
                //the annonymous thisDistinctP cannot be hydrated, so get vTreatingSpecialtyRecent3Yrs type
                var hydratedPatient = HydrateDTO.HydrateTreatingSpecialtyPatient(thisDistinctP);

                var admissions = thisFacilityPatients.Where(p => p.PatientICN == thisDistinctP.PatientICN).Select(p => p.admitday.Value).ToList();

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
