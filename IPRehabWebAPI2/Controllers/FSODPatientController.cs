using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
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
    public FSODPatientController(IFSODPatientRepository patientRepository, IEpisodeOfCareRepository episodeOfCareRepository, MasterreportsContext masterReportContext)
    {
      _patientRepository = patientRepository;
      _episodeOfCareRepository = episodeOfCareRepository;
      _masterReportsContext = masterReportContext;
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
      //internally retrieve windows identity from User.Claims
      string networkName = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
      if (string.IsNullOrEmpty(networkName))
      {
        return NotFound("Windows Identity is null.  Make sure the service allows Windows Authentication");
      }

      var cacheHelper = new CacheHelper(); 
      List<MastUserDTO> userAccessLevels = await cacheHelper.GetUserAccessLevels(_masterReportsContext, networkName);

      if (!userAccessLevels.Any())
      {
        return BadRequest("You are not permitted to view any facility's patients");
      }
      else
      {
        List<string> userFacilities = userAccessLevels.Select(x => x.Facility).Distinct().ToList();
        userFacilities = new List<string>() { "648" };

        int[] quarters = new int[] { 2, 2, 2, 3, 3, 3, 4, 4, 4, 1, 1, 1 };
        var currentQuarterNumber = quarters[DateTime.Today.Month - 1];

        int defaultQuarter = int.Parse($"{DateTime.Today.Year}{currentQuarterNumber}"); //result like 20213, 20221

        IEnumerable<PatientDTO> patients = null;
        try
        {
          patients = await cacheHelper.GetPatients(_patientRepository, defaultQuarter, criteria);
          
          var patientFacilities = patients.Select(x => x.Facility).Distinct().ToList();
          
          patients = patients.Where(p=> userFacilities.Any(i=>p.Facility.Contains(i))).ToList();

          //List<PatientDTO> viewablePatients = new List<PatientDTO>();
          //foreach (PatientDTO pat in patients)
          //{
          //  foreach (string fac in userFacilities)
          //  {
          //    if (pat.Facility.IndexOf(fac) >= 0)
          //    {
          //      viewablePatients.Add(pat);
          //      break;
          //    }
          //  }
          //}

          //patients = viewablePatients;
          if (withEpisode)
          {
            foreach (var p in patients)
            {
             var  theseEpisodes = await _episodeOfCareRepository.FindByCondition(episode =>
                episode.PatientIcnfk == p.PTFSSN).ToListAsync();
              if (theseEpisodes.Any()) { 
                p.CareEpisodes = theseEpisodes.Select(e => HydrateDTO.HydrateEpisodeOfCare(e));
              }
            }
          }
        }
        catch(Exception ex)
        {
          Console.WriteLine(ex.Message);
          throw;
        }
      return Ok(patients);
      }
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
