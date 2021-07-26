using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FSODPatientController : ControllerBase
  {
    private readonly IFSODPatientRepository _patientRepository;
    private readonly IEpisodeOfCareRepository _episodeOfCareRepository;
    private readonly MasterreportsContext _masterReportsContext;
    private readonly IMemoryCache _memoryCache;
    public FSODPatientController(IFSODPatientRepository patientRepository, IEpisodeOfCareRepository episodeOfCareRepository, MasterreportsContext masterReportContext, IMemoryCache memoryCache)
    {
      _patientRepository = patientRepository;
      _episodeOfCareRepository = episodeOfCareRepository;
      _masterReportsContext = masterReportContext;
      _memoryCache = memoryCache;
    }

    /// <summary>
    /// get patient matching the criteria and limit to user facility level
    /// do not across facility boundary unless the user has access level beyond facility level
    /// </summary>
    /// <param name="criteria"></param>
    /// <param name="withEpisode"></param>
    /// <returns></returns>
    // GET: api/Patients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatient(string criteria, bool withEpisode)
    {
      string networkName = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
      if (string.IsNullOrEmpty(networkName))
      {
        return NotFound(new { Message = "Windows Identity is null.  Make sure the service allows Windows Authentication" });
      }

      var inMemoryCache = new CacheHelper(_memoryCache); 
      var userAccessLevels = await inMemoryCache.GetUserAccessLevels(_masterReportsContext, networkName);
      var userFacilities = userAccessLevels.Select(x => x.Facility).Distinct().ToArray();

      int[] quarters = new int[] { 2, 2, 2, 3, 3, 3, 4, 4, 4, 1, 1, 1};
      var currentQuarterNumber = quarters[DateTime.Today.Month - 1];

      int defaultQuarter = int.Parse($"{DateTime.Today.Year}{currentQuarterNumber}"); //result like 20213, 20221

      List<PatientDTO> patients;
      List<EpisodeOfCareDTO> episodes;

      patients = await inMemoryCache.GetPatients(_patientRepository, defaultQuarter, criteria);

      List<PatientDTO> viewablePatients = new List<PatientDTO>();
      foreach(string s in userFacilities)
      {
        var thesePatients = patients.Where(x => x.Facility.Contains(s)).ToList();
        viewablePatients.AddRange(thesePatients);
      }

      if (withEpisode)
      {
        foreach (var p in viewablePatients)
        {
          episodes = await _episodeOfCareRepository.FindByCondition(p =>
            p.PatientIcnfkNavigation.Icn == p.PatientIcnfk).Select(e => HydrateDTO.HydrateEpisodeOfCare(e)).ToListAsync();
          p.CareEpisodes = episodes;
        }
      }

      return Ok(viewablePatients);
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
