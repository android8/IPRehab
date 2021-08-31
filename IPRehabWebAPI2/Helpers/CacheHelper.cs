using IPRehabRepository.Contracts;
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
    /// <param name="pageNumber"></param>
    /// <param name="PageSize"></param>
    /// <returns></returns>
    public async Task<IEnumerable<PatientDTO>> GetPatients(IFSODPatientRepository _patientRepository, string networkName, string criteria,  int pageNumber, int PageSize)
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
        var currentQuarterNumber = quarters[DateTime.Today.Month - 1];

        int defaultQuarter = int.Parse($"{DateTime.Today.Year}{currentQuarterNumber}"); //result like 20213, 20221
        List<int> theseQuarters = new List<int>() { defaultQuarter, defaultQuarter - 1, defaultQuarter - 2 };

        string cacheKey = criteria;
        if (string.IsNullOrEmpty(criteria))
          cacheKey = "No Criteria";

        IEnumerable<PatientDTO> patients = null;

        //no criteria
        if (string.IsNullOrEmpty(criteria))
        {
          foreach (int thisQuarter in theseQuarters)
          {
            patients = await _patientRepository.FindByCondition(p => thisQuarter == p.FiscalPeriodInt).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

            //facility filter cannot be done in previous SQL server side query. must be filtered by IIS memory 
            patients = patients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf))).Skip((pageNumber - 1) * PageSize).Take(PageSize);

            if (patients.Any())
              break;
          }
        }
        //with criteria
        else
        {
          int numericCriteria;
          //numeri criteria
          if (int.TryParse(criteria, out numericCriteria))
          {
            foreach (int thisQuarter in theseQuarters)
            {
              patients = await _patientRepository.FindByCondition(p =>
                (thisQuarter == p.FiscalPeriodInt) &&
                (
                  p.ADMParent_Key == numericCriteria || p.Sta6aKey == numericCriteria || p.bedsecn == numericCriteria ||
                  p.FiscalPeriodInt == numericCriteria || p.PTFSSN.Contains(criteria) || p.Facility.Contains(criteria) || p.VISN.Contains(criteria)
                )
              )
              .Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

              //facility filter cannot be done in previous SQL server side query. must be filtered by IIS memory 
              patients = patients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf))).Skip((pageNumber - 1) * PageSize).Take(PageSize);

              if (patients.Any())
                break;
            }
          }
          //non-numeric criteria
          else
          {
            foreach (int thisQuarter in theseQuarters)
            {
              patients = await _patientRepository.FindByCondition(p =>
                (thisQuarter == p.FiscalPeriodInt) &&
                (
                p.Name.Contains(criteria) || p.PTFSSN.Contains(criteria) || p.Facility.Contains(criteria) ||
                p.VISN.Contains(criteria) || p.District.Contains(criteria) || p.FiscalPeriod.Contains(criteria)
                )
              )
              .Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

              //facility filter cannot be done in previous SQL server side query. must be filtered by IIS memory 
              patients = patients.Where(p => userFacilitySta3.Any(uf => p.Facility.Contains(uf))).Skip((pageNumber - 1) * PageSize).Take(PageSize);

              if (patients.Any())
                break;
            }
          }
        }
        return patients;
      }
    }
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
