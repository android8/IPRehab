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
    /// <param name="currentUser"></param>
    /// <returns></returns>
    // GET: api/Patients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> Get(string criteria, bool withEpisode, string currentUser)
    {
      string theUser = currentUser;

      if (string.IsNullOrEmpty(theUser))
      {
        //replace with windows identity from User.Claims
        string claimName = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;

        theUser = claimName;

        //ToDo: remove this hard coded network name
        theUser = "VHALEBBIDELD";

        if (string.IsNullOrEmpty(theUser))
        {
          return BadRequest("User.claims or Windows Identity is null.  Make sure the service allows Windows Authentication");
        }
      }

      var cacheHelper = new CacheHelper();
      List<MastUserDTO> userAccessLevels = await cacheHelper.GetUserAccessLevels(_masterReportsContext, theUser);

      if (userAccessLevels == null)
      {
        return NotFound("You do not have asccess to any facility data");
      }
      else
      {
        List<string> userFacilities = userAccessLevels.Where(x=>!string.IsNullOrEmpty(x.Facility)).Select(x => x.Facility).Distinct().ToList();
        if (userFacilities == null)
          return NotFound("None of the items in your access level matches any valid facility");

        //ToDo: remove this hard coded facility
        //userFacilities = new List<string>() { "648" };

        int[] quarters = new int[] { 2, 2, 2, 3, 3, 3, 4, 4, 4, 1, 1, 1 };
        var currentQuarterNumber = quarters[DateTime.Today.Month - 1];

        int defaultQuarter = int.Parse($"{DateTime.Today.Year}{currentQuarterNumber}"); //result like 20213, 20221

        IEnumerable<PatientDTO> patients = null;
        try
        {
          patients = await cacheHelper.GetPatients(_patientRepository, defaultQuarter, criteria);

          //var patientFacilities = patients.Select(x => x.Facility).Distinct().ToList();

          if (patients == null)
            return NotFound("Patient data is not ready.  Make sure the data for the current quarter is loaded");
          else
          {
            if (withEpisode)
            {
              patients = patients.Where(p => userFacilities.Any(uf => p.Facility.Contains(uf)));

              if (patients == null)
                return NotFound("There are no viewable patients in the facility with your access level.  Make sure the data for the current quarter is available");
              else
                patients.ToList();

              foreach (var p in patients)
              {
                var theseEpisodes = await _episodeOfCareRepository.FindByCondition(episode =>
                  episode.PatientICNFK == p.PTFSSN).ToListAsync();
                if (theseEpisodes.Any())
                {
                  p.CareEpisodes = theseEpisodes.Select(e => HydrateDTO.HydrateEpisodeOfCare(e));
                }
              }
            }
          }
        }
        catch (Exception ex)
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
          p.PatientICNFKNavigation.ICN == p.PatientICNFK).Select(e => HydrateDTO.HydrateEpisodeOfCare(e)).ToListAsync();
        patient.CareEpisodes = episodes;

      }
      return Ok(patient);
    }
  }
}
