using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
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
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  public class FSODPatientController : ControllerBase
  {
    private readonly IFSODPatientRepository _patientRepository;
    private readonly IEpisodeOfCareRepository _episodeOfCareRepository;
    private readonly MasterreportsContext _masterReportsContext;
    private readonly CacheHelper cacheHelper;

    public FSODPatientController(IFSODPatientRepository patientRepository, IEpisodeOfCareRepository episodeOfCareRepository, MasterreportsContext masterReportContext)
    {
      _patientRepository = patientRepository;
      _episodeOfCareRepository = episodeOfCareRepository;
      _masterReportsContext = masterReportContext;
      cacheHelper = new CacheHelper(_masterReportsContext);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    /// <summary>
    /// get patients matching the search criteria with visibility by the user facility level
    /// </summary>
    /// <param name="criteria"></param>
    /// <param name="withEpisode"></param>
    /// /// <param name="currentUser"></param>
    /// <returns></returns>
    // GET: api/Patients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatient(string impersonatedUserName, string criteria, bool withEpisode, int pageNumber = 1, int pageSize = 50)
    {
      //ToDo: use Registration to authorize user.  Otherwise, any string in the currentUser parameter containing a valid network ID will be parameterized for  cacheHelper.GetUserAccessLevels()

      //internally retrieve windows identity from User.Claims
      string networkName = string.IsNullOrEmpty(impersonatedUserName) ? HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value : impersonatedUserName;

      //ToDo: remove this hard coded network name
      //networkName = "VHALEBBIDELD";

      if (string.IsNullOrEmpty(networkName))
      {
        return NotFound("Windows Identity is null.  Make sure the service allows Windows Authentication");
      }


      IEnumerable<PatientDTO> patients = null;
      //get all patient with criteria and quarter filter
      patients = await cacheHelper.GetPatients(_patientRepository, networkName, criteria, pageNumber, pageSize);

      if (patients == null)
      {
        return NotFound("No patient is in rehab for the quarter with spcified criteria and visible with your access level.  First, check if patient data is avaialbe for the specified quarter. Second, remove or widen the search criteria, Third, check your level of facility access");
      }
      else
      {
        if (withEpisode)
        {
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
      return Ok(patients);
    }

    /// <summary>
    /// get only meta data of an individual patient by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="criteria"></param>
    /// <param name="networkName"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // GET: api/Patients/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDTO>> Get(string id, string networkName, string criteria, int pageNumber = 1, int pageSize = 1)
    {
      var patients = await cacheHelper.GetPatients(_patientRepository, networkName, criteria, pageNumber, pageSize);
      var thisPatient = patients.FirstOrDefault(x => x.PTFSSN == id);
      if (thisPatient == null)
      {
        return NotFound();
      }
      else
      {
        var episodes = await _episodeOfCareRepository.FindByCondition(p =>
          p.PatientICNFKNavigation.ICN == p.PatientICNFK).Select(e => HydrateDTO.HydrateEpisodeOfCare(e)).ToListAsync();
        thisPatient.CareEpisodes = episodes;
        return Ok(thisPatient);
      }
    }
  }
}
