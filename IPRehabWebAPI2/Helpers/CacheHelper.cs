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
    /// <summary>
    /// Do not use generic repository, instead use MastReportsContext to execute stored procedure to get user access levels
    /// </summary>
    /// <param name="context"></param>
    /// <param name="networkID"></param>
    /// <returns></returns>
    public async Task<List<MastUserDTO>> GetUserAccessLevels(MasterreportsContext context, string networkID)
    {
      try
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
        var userPermission = await context.SqlQueryAsync<uspVSSCMain_SelectAccessInformationFromNSSDResult>(
          $"execute [Apps].[uspVSSCMain_SelectAccessInformationFromNSSD] @UserName", paramNetworkID);

        foreach (var item in userPermission)
        {
          var userDTO = HydrateDTO.HydrateUser(item);
          userAccessLevels.Add(userDTO);
        }
        return userAccessLevels;
      }
      catch(Exception ex){
        Console.WriteLine(ex.Message);
        return null;
      }
    }

    /// <summary>
    /// use generic IFODPatientRepository to get patient filtered by quarter and criteria
    /// </summary>
    /// <param name="_patientRepository"></param>
    /// <param name="currentQuarter"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public async Task<IEnumerable<PatientDTO>> GetPatients(IFSODPatientRepository _patientRepository, int currentQuarter, string criteria)
    {
      string cacheKey = criteria;
      if (string.IsNullOrEmpty(criteria))
        cacheKey = "No Criteria";

      IEnumerable<PatientDTO> patients=null;

      List<int> theseQuarters = new List<int>() { currentQuarter, currentQuarter - 1, currentQuarter - 2 };

      //no criteria
      try
      {
        if (string.IsNullOrEmpty(criteria))
        {
          foreach (int thisQuarter in theseQuarters)
          {
            patients = await _patientRepository.FindByCondition(p =>
                thisQuarter == p.FiscalPeriodInt
                )
              .Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

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
              patients = await _patientRepository.FindByCondition(x => (thisQuarter == x.FiscalPeriodInt)
                && (x.ADMParent_Key == numericCriteria || x.Sta6aKey == numericCriteria || x.bedsecn == numericCriteria || x.FiscalPeriodInt == numericCriteria || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria)))
                .Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

              if (patients.Any())
                break;
            }
          }
          //non-numeric criteria
          else
          {
            foreach(int thisQuarter in theseQuarters)
            {
              patients = await _patientRepository.FindByCondition(x =>
                (thisQuarter == x.FiscalPeriodInt)
                  && (x.Name.Contains(criteria) || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria) || x.District.Contains(criteria) || x.FiscalPeriod.Contains(criteria)))
                .Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
              if (patients.Any())
                break;
            }
          }
        }
      }
      catch (Exception ex){
        Console.WriteLine(ex.Message);
        throw;
      }
      return patients;
    }

    private string CleanUserName(string networkID)
    {
      string networkName = networkID;
      if (string.IsNullOrEmpty(networkName))
        return null;
      else
      {
        if (networkName.Contains('\\') || networkName.Contains("%2F"))
        {
          String[] separator = { "\\", "%2F" };
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
