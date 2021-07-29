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
    /// <summary>
    /// Do not use generic repository, instead use MastReportsContext to execute stored procedure to get user access levels
    /// </summary>
    /// <param name="context"></param>
    /// <param name="networkID"></param>
    /// <returns></returns>
    public async Task<List<MastUserDTO>> GetUserAccessLevels(MasterreportsContext context, string networkID)
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

    /// <summary>
    /// use generic IFODPatientRepository to get patient filtered by quarter and criteria
    /// </summary>
    /// <param name="_patientRepository"></param>
    /// <param name="currentQuarter"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public async Task<List<PatientDTO>> GetPatients(IFSODPatientRepository _patientRepository, int currentQuarter, string criteria)
    {
      string cacheKey = criteria;
      if (string.IsNullOrEmpty(criteria))
        cacheKey = "No Criteria";

      int previousQuarter = currentQuarter - 1;
      int twoQuarterAgo = currentQuarter - 2;

      List<PatientDTO> patients;
        //no criteria
        if (string.IsNullOrEmpty(criteria))
        {
          //current quarter patients
          patients = await _patientRepository.FindByCondition(x =>
            (currentQuarter == x.FiscalPeriodInt)
          ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

          //previous quarter patients
          if (patients == null || patients.Count == 0)
          {
            patients = await _patientRepository.FindByCondition(x =>
              (previousQuarter == x.FiscalPeriodInt)
            ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
          }

          //two quarter prior patients
          if (patients == null || patients.Count == 0)
          {
            patients = await _patientRepository.FindByCondition(x =>
              (twoQuarterAgo == x.FiscalPeriodInt)
            ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
          }
        }
        //with criteria
        else
        {
          int numericCriteria;
          //numeri criteria
          if (int.TryParse(criteria, out numericCriteria))
          {
            //current quarter patients
            patients = await _patientRepository.FindByCondition(x =>
              (currentQuarter == x.FiscalPeriodInt)
              && (x.ADMParent_Key == numericCriteria || x.Sta6aKey == numericCriteria || x.bedsecn == numericCriteria || x.FiscalPeriodInt == numericCriteria || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria))
            ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

            //previous quarter patients
            if (patients == null || patients.Count == 0)
            {
              patients = await _patientRepository.FindByCondition(x =>
                (previousQuarter == x.FiscalPeriodInt)
                && (x.ADMParent_Key == numericCriteria || x.Sta6aKey == numericCriteria || x.bedsecn == numericCriteria || x.FiscalPeriodInt == numericCriteria || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria))
              ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
            }

            //two quarter prior patients
            if (patients == null || patients.Count == 0)
            {
              patients = await _patientRepository.FindByCondition(x =>
                (twoQuarterAgo == x.FiscalPeriodInt)
                && (x.ADMParent_Key == numericCriteria || x.Sta6aKey == numericCriteria || x.bedsecn == numericCriteria || x.FiscalPeriodInt == numericCriteria || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria))
              ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
            }
          }
          //alpha criteria
          else
          {
            //current quarter patients
            patients = await _patientRepository.FindByCondition(x =>
              (currentQuarter == x.FiscalPeriodInt)
              && (x.Name.Contains(criteria) || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria) || x.District.Contains(criteria) || x.FiscalPeriod.Contains(criteria))
            ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

            //previous quarter patients
            if (patients == null || patients.Count == 0)
            {
              patients = await _patientRepository.FindByCondition(x =>
                (previousQuarter == x.FiscalPeriodInt)
                && (x.Name.Contains(criteria) || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria) || x.District.Contains(criteria) || x.FiscalPeriod.Contains(criteria))
              ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
            }

            //two quarter ago patients
            if (patients == null || patients.Count == 0)
            {
              patients = await _patientRepository.FindByCondition(x =>
                (twoQuarterAgo == x.FiscalPeriodInt)
                && (x.Name.Contains(criteria) || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria) || x.District.Contains(criteria) || x.FiscalPeriod.Contains(criteria))
              ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
            }
          }
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
