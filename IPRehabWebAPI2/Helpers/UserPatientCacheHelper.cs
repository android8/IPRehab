using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    protected readonly MasterreportsContext _context;
    protected readonly IConfiguration _configuration;

    /// <summary>
    /// constructor injection of MasterreportsContext in order to execute _context.SqlQueryAsync()
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    public UserPatientCacheHelper(IConfiguration configuration, MasterreportsContext context)
    {
      _context = context;
      _configuration = configuration;
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
    /// use generic IFODPatientRepository to get patient filtered by quarter and criteria
    /// </summary>
    /// <param name="_patientRepository"></param>
    /// <param name="networkName">optional</param>
    /// <param name="criteria">optional</param>
    /// <param name="orderBy">optional</param>
    /// <param name="pageNumber">optional</param>
    /// <param name="PageSize">optional</param>
    /// <param name="patientID">optional, used by individual patient search only</param>

    /// <returns></returns>
    public async Task<List<PatientDTO>> GetPatients(IFSODPatientRepository _patientRepository, string networkName, string criteria, string orderBy, int pageNumber, int PageSize, string patientID)
    {
      List<MastUserDTO> distinctUserFacilities = new()
      {
        new()
        {
          Facility = _configuration.GetSection("AppSettings").GetValue<string>("TestSite")
        }
      };

      if (string.IsNullOrEmpty(distinctUserFacilities.First().Facility))
      {
        //get user access level from external stored proc
        distinctUserFacilities = await GetUserAccessLevels(networkName);
      }

      if (!distinctUserFacilities.Any())
      {
        return null;
      }
      else
      {
        List<string> userFacilitySta3 = distinctUserFacilities.Select(x => x.Facility).Distinct().ToList();

        string cacheKey = criteria;
        if (string.IsNullOrEmpty(criteria))
          cacheKey = "No Criteria";

        List<PatientDTO> patients = null;
        int totalViewablePatientCount = 0;
        string searchCriteriaType = string.Empty;
        int numericCriteria = -1;

        if (string.IsNullOrEmpty(criteria))
          searchCriteriaType = "none";
        else if (int.TryParse(criteria, out numericCriteria))
          searchCriteriaType = "numeric";
        else
          searchCriteriaType = "non-numeric";

        foreach (int thisPeriod in GetQuarterOfInterest())
        {
          switch (searchCriteriaType)
          {
            case "none":
              {
                var rawPatients = await _patientRepository
                  .FindByCondition(p => thisPeriod == p.FiscalPeriodInt).OrderBy(x=>x.Name).ToListAsync();

                if (rawPatients.Any())
                {
                  //applying facility filter cannot be done in previous SQL server side query
                  //it must be filtered in IIS memory after the last ToListAsync()
                  var viewablePatients = rawPatients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));

                  if (!string.IsNullOrEmpty(patientID))
                    viewablePatients = viewablePatients.Where(x => x.PTFSSN == patientID);

                  totalViewablePatientCount = viewablePatients.Count();

                  if (pageNumber <= 0)
                  {
                    viewablePatients = viewablePatients.Take(PageSize);
                  }
                  else
                  {
                    viewablePatients = viewablePatients.Skip((pageNumber - 1) * PageSize).Take(PageSize);
                  }

                  patients = viewablePatients.Select(p => HydrateDTO.HydratePatient(p)).ToList();

                  break; //break out foreach loop since patients are in this period
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
                                ).OrderBy(x=>x.Name).ToListAsync();

                if (rawPatients.Any())
                {
                  //facility filter cannot be done in previous SQL server side LINQ. must be filtered in IIS memory 
                  var viewablePatients = rawPatients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));

                  if (!string.IsNullOrEmpty(patientID))
                    viewablePatients = viewablePatients.Where(x => x.PTFSSN == patientID);

                  totalViewablePatientCount = viewablePatients.Count();
                  patients = viewablePatients.Skip((pageNumber - 1) * PageSize).Take(PageSize)
                    .Select(p => HydrateDTO.HydratePatient(p)).ToList();

                  break; //break out foreach loop since patients are in this period
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
                                ).OrderBy(x=>x.Name).ToListAsync();

                if (rawPatients.Any())
                {
                  //facility filter cannot be done in previous SQL server side query. must be filtered by IIS memory 
                  var viewablePatients = rawPatients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf)));

                  if (!string.IsNullOrEmpty(patientID))
                    viewablePatients = viewablePatients.Where(x => x.PTFSSN == patientID);

                  totalViewablePatientCount = viewablePatients.Count();

                  patients = viewablePatients.Skip((pageNumber - 1) * PageSize).Take(PageSize)
                    .Select(p => HydrateDTO.HydratePatient(p)).ToList();

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

      }
      return patients.FirstOrDefault();
    }

    private static List<int> GetQuarterOfInterest() {
      DateTime today = DateTime.Today;

      int currentYear = today.Year;
      int lastYear = currentYear - 1;
      int nextYear = currentYear + 1;

      int[] quarters = new int[] { 2, 2, 2, 3, 3, 3, 4, 4, 4, 1, 1, 1 };
      int currentQTableNumber = currentYear, minus1QTableNumber = currentYear, minus2QTableNumber = currentYear, minus3QTableNumber = currentYear;

      int currentQuarter = quarters[today.Month-1];
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
