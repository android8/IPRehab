﻿using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FSODPatientController : ControllerBase
  {
    private readonly IFSODPatientRepository _patientRepository;
    private readonly IEpisodeOfCareRepository _episodeOfCareRepository;

    public FSODPatientController(IFSODPatientRepository patientRepository, IEpisodeOfCareRepository episodeOfCareRepository)
    {
      _patientRepository = patientRepository;
      _episodeOfCareRepository = episodeOfCareRepository;
    }

    /// <summary>
    /// get patient matching the criteria and limit to user facility level
    /// do not across facility boundary unless the user has access level beyond facility level
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    // GET: api/Patients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatient(string criteria)
    {
      int[] quarters = new int[] { 2, 2, 2, 3, 3, 3, 4, 4, 4, 1, 1, 1};
      var currentQuarter = quarters[DateTime.Today.Month - 1];

      int defaultQuarter = int.Parse($"{DateTime.Today.Year}{currentQuarter}");

      var firstGrant = await _patientRepository.FindByCondition(x =>
                         defaultQuarter == x.FiscalPeriodInt || defaultQuarter - 1 == x.FiscalPeriodInt)
                          .OrderByDescending(x => x.FiscalPeriodInt)
                        .Select(p => HydrateDTO.HydrateUserFacilityGrant(p)).FirstOrDefaultAsync();

      List<PatientDTO> patients;
      List<EpisodeOfCareDTO> episodes;

      if (string.IsNullOrEmpty(criteria))
      {
        patients = await _patientRepository.FindByCondition(x =>
          //firstGrant.Facility.Contains(x.Facility) ||| firstGrant.District.Contains(x.District) ||
          firstGrant.Division.Contains(x.Division)
        ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
      }
      else
      {
        int numericCriteria;
        if (int.TryParse(criteria, out numericCriteria))
        {
          //ToDo: find user grant from User API
          patients = await _patientRepository.FindByCondition(x =>
             (x.ADMParent_Key == numericCriteria || x.Sta6aKey == numericCriteria || x.bedsecn == numericCriteria || x.FiscalPeriodInt == numericCriteria)
          //&& (userFacilityGrants.Facility.Contains(x.Facility) || userFacilityGrants.District.Contains(x.District) || userFacilityGrants.Division.Contains(x.Division))

          ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();

          if (patients == null || patients.Count == 0)
          {
            patients = await _patientRepository.FindByCondition(x =>
              (x.Name.Contains(criteria) || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria) || x.District.Contains(criteria) || x.FiscalPeriod.Contains(criteria))
            //&& (userFacilityGrants.Facility.Contains(x.Facility) || userFacilityGrants.District.Contains(x.District) || userFacilityGrants.Division.Contains(x.Division))
            ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
          }
        }
        else
        {
          patients = await _patientRepository.FindByCondition(x =>
             (x.Name.Contains(criteria) || x.PTFSSN.Contains(criteria) || x.Facility.Contains(criteria) || x.VISN.Contains(criteria) || x.District.Contains(criteria) || x.FiscalPeriod.Contains(criteria))
          //&& (userFacilityGrants.Facility.Contains(x.Facility) || userFacilityGrants.District.Contains(x.District) || userFacilityGrants.Division.Contains(x.Division))
          ).Select(p => HydrateDTO.HydratePatient(p)).ToListAsync();
        }
      }

      if (patients == null)
      {
        return NotFound();
      }
      else
      {
        foreach (var p in patients)
        {
          episodes = await _episodeOfCareRepository.FindByCondition(p =>
            p.PatientIcnfkNavigation.Icn == p.PatientIcnfk).Select(e => HydrateDTO.HydrateEpisodeOfCare(e)).ToListAsync();
          p.CareEpisodes = episodes;
        }
      }

      return Ok(patients);
    }

    // GET: api/Patients/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDTO>> Get(string id)
    {
      var patient = await _patientRepository.FindByCondition(x => x.PTFSSN == id)
        .Select(p => HydrateDTO.HydratePatient(p)).SingleOrDefaultAsync();
      if (patient == null)
      {
        return NotFound();
      }
      else
      {
        var episodes = await _episodeOfCareRepository.FindByCondition(p =>
          p.PatientIcnfkNavigation.Icn == p.PatientIcnfk).Select(e => HydrateDTO.HydrateEpisodeOfCare(e)).ToListAsync();
         patient.CareEpisodes = episodes;

      }
      return Ok(patient);
    }
  }
}
