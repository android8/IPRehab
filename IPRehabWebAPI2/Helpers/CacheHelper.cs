﻿using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Helpers
{
  public class CacheHelper
  {
    readonly MasterreportsContext _context;

    public CacheHelper(MasterreportsContext context)
    {
      _context = context;
    }
    /// <summary>
    /// Do not use generic repository, instead use MastReportsContext to execute stored procedure to get user access levels
    /// </summary>
    /// <param name="networkID"></param>
    /// <returns></returns>
    public async Task<List<MastUserDTO>> GetUserAccessLevels(string networkID)
    {
      string userName = CleanUserName(networkID); //use network ID without domain
      List<MastUserDTO> userAccessLevels = new List<MastUserDTO>();

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
    /// use generic IFODPatientRepository to get patient filtered by quarter and criteria
    /// </summary>
    /// <param name="_patientRepository"></param>
    /// <param name="networkName"></param>
    /// <param name="criteria"></param>
    /// <param name="orderBy"></param>
    /// <param name="pageNumber"></param>
    /// <param name="PageSize"></param>

    /// <returns></returns>
    public async Task<IEnumerable<PatientDTO>> GetPatients(IFSODPatientRepository _patientRepository, string networkName, string criteria, string orderBy, int pageNumber, int PageSize)
    {
      //get user access level from external stored proc
      var distinctUserFacilities = await GetUserAccessLevels(networkName);

      if (!distinctUserFacilities.Any())
      {
        return null;
      }
      else
      {
        List<string> userFacilitySta3 = distinctUserFacilities.Select(x => x.Facility).Distinct().ToList();

        int[] quarters = new int[] { 2, 2, 2, 3, 3, 3, 4, 4, 4, 1, 1, 1 };
        DateTime today = DateTime.Today;
        int currentFY = today.Year;
        if (today.Month >= 10)
          currentFY = today.Year + 1;

        List<int> fiscalPeriodsOfInterest = new()
        {
          /* use month posistion in the quarters[] for the target quarter data whichever is available */
          /* current Q */
          (currentFY * 10) + quarters[today.Month],
          /* last Q */
          (today.AddMonths(-3).Year * 10) + quarters[today.AddMonths(-3).Month],
          /* 2nd Q */
          (today.AddMonths(-6).Year * 10) + quarters[today.AddMonths(-6).Month]
        };

        string cacheKey = criteria;
        if (string.IsNullOrEmpty(criteria))
          cacheKey = "No Criteria";

        IEnumerable<PatientDTO> patients = null;
        int totalViewablePatientCount = 0;
        string searchCriteriaType = string.Empty;
        int numericCriteria = -1;

        if (string.IsNullOrEmpty(criteria))
          searchCriteriaType = "none";
        else if (int.TryParse(criteria, out numericCriteria))
          searchCriteriaType = "numeric";
        else
          searchCriteriaType = "non-numeric";

        foreach (int thisPeriod in fiscalPeriodsOfInterest)
        {
          patients = await _patientRepository.FindByCondition(p => thisPeriod == p.FiscalPeriodInt)
            .Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

          switch (searchCriteriaType)
          {
            case "none":
              {
                if (patients.Any())
                {
                  //applying facility filter cannot be done in previous SQL server side query
                  //it must be filtered in IIS memory after the last ToListAsync()
                  var viewablePatients = patients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));
                  totalViewablePatientCount = viewablePatients.Count();
                  if (pageNumber <= 0)
                  {
                    patients = viewablePatients.Take(PageSize);
                  }
                  else
                  {
                    patients = viewablePatients.Skip((pageNumber - 1) * PageSize).Take(PageSize);
                  }
                  break; //break out foreach loop since patients are in this period
                }
              }
              break; //break case
            case "numeric":
              {
                patients = await _patientRepository.FindByCondition(p =>
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
                                )
                                .Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

                if (patients.Any())
                {
                  //facility filter cannot be done in previous SQL server side LINQ. must be filtered in IIS memory 
                  var viewablePatients = patients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));
                  totalViewablePatientCount = viewablePatients.Count();
                  patients = viewablePatients.Skip((pageNumber - 1) * PageSize).Take(PageSize);
                  
                  break; //break out foreach loop since patients are in this period
                }
              }
              break; //break case
            case "non-numeric":
              {
                patients = await _patientRepository.FindByCondition(p =>
                                  (thisPeriod == p.FiscalPeriodInt) &&
                                  (
                                    p.Name.Contains(criteria) || p.PTFSSN.Contains(criteria) || p.Facility.Contains(criteria) ||
                                    p.VISN.Contains(criteria) || p.District.Contains(criteria) || p.FiscalPeriod.Contains(criteria)
                                  )
                                ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

                if (patients.Any())
                {
                  //facility filter cannot be done in previous SQL server side query. must be filtered by IIS memory 
                  var viewablePatients = patients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));
                  totalViewablePatientCount = viewablePatients.Count();
                  patients = viewablePatients.Skip((pageNumber - 1) * PageSize).Take(PageSize);

                  break;  //break out foreach loop since patients are in this period
                }
              }
              break;
          }
        }
        //PatientSearchResultDTO meta = new() { Patients = patients.ToList(), TotalCount = totalViewablePatientCount };
        //return (meta);

        return patients;
      }
    }

    /// <summary>
    /// this should be in a utility library
    /// </summary>
    /// <param name="networkID"></param>
    /// <returns></returns>
    private string CleanUserName(string networkID)
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
