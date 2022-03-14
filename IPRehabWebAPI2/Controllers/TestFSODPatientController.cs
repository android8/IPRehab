using IPRehabModel;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Controllers
{
  // ToDo: to be deleted after test

  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  public class TestFSODPatientController : ControllerBase
  {
    private readonly IFSODPatientRepository _patientRepository;
    private readonly IEpisodeOfCareRepository _episodeOfCareRepository;
    private readonly ITestUserPatientCacheHelper _cacheHelper;

    public TestFSODPatientController(IFSODPatientRepository patientRepository, IEpisodeOfCareRepository episodeOfCareRepository, ITestUserPatientCacheHelper cacheHelper)
    {
      _patientRepository = patientRepository;
      _episodeOfCareRepository = episodeOfCareRepository;
      _cacheHelper = cacheHelper;
    }

    /// <summary>
    /// get patients matching the search criteria with visibility by the user facility level
    /// </summary>
    /// <param name="patientID"></param>
    /// <param name="criteria"></param>
    /// <param name="withEpisode"></param>
    /// <param name="orderBy"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    // GET: api/Patients
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients(string patientID, string criteria, bool withEpisode, string orderBy, int pageNumber = 1, int pageSize = 50)
    {
      //ToDo: use Registration to authorize user.  Otherwise, any string in the currentUser parameter containing a valid network ID will be parameterized for  cacheHelper.GetUserAccessLevels()

      //internally retrieve windows identity from User.Claims
      //string networkName = string.IsNullOrEmpty(impersonatedUserName) ? HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value : impersonatedUserName;
      //networkID = string.IsNullOrEmpty(networkID) ? HttpContext.User.Identity.Name : networkID;

      //ToDo: remove this hard coded network name
      //networkName = "VHALEBBIDELD";

      //get all patient with criteria and quarter filter
      List<PatientDTO> patients = await _cacheHelper.GetPatients(_patientRepository, criteria, orderBy, pageNumber, pageSize, patientID);

      if (patients == null)
      {
        return NotFound("No patient is in rehab for the quarter with spcified criteria and visible with your access level.  First, check if patient data is avaialbe for the specified quarter. Second, remove or widen the search criteria, Third, check your level of facility access");
      }
      else
      {
        if (withEpisode)
        {
          foreach (PatientDTO p in patients)
          {
            //p.CareEpisodes = new();
            List<tblEpisodeOfCare> episodes = await _episodeOfCareRepository.FindByCondition(episode =>
              episode.PatientICNFK == p.PTFSSN).ToListAsync();

            foreach (var episode in episodes)
            {
              EpisodeOfCareDTO  thisEpisode = HydrateDTO.HydrateEpisodeOfCare(episode);
              //hydrate and add the dates from the tblAnswer because they are more up-to-date than the episode dates
              p.CareEpisodes.Add(thisEpisode);
            }
          }
        }
      }
      return Ok(patients);
    }

    /// <summary>
    /// get only meta data of an individual patient by id
    /// </summary>
    /// <param name="patientID"></param>
    /// <param name="networkID"></param>
    /// <param name="withEpisode"></param>
    /// <param name="orderBy"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // GET: api/Patients/5
    [HttpGet("{patientID}")]
    public async Task<ActionResult<PatientDTO>> GetPatient(string patientID, string networkID, bool withEpisode, string orderBy, int pageNumber = 1, int pageSize = 1)
    {
      if (string.IsNullOrEmpty(networkID)) {
        networkID = HttpContext.User.Identity.Name;
      }
      string criteria = string.Empty; //no criteria is needed for patient search based on id
      List<PatientDTO> patients = await _cacheHelper.GetPatients(_patientRepository, criteria, orderBy, pageNumber, pageSize, patientID);
      PatientDTO thisPatient = patients.FirstOrDefault(x => x.PTFSSN == patientID);
      if (thisPatient == null)
      {
        return NotFound();
      }
      else
      {
        if (withEpisode)
        {
          List<EpisodeOfCareDTO> episodes = await _episodeOfCareRepository.FindByCondition(p =>
            p.PatientICNFK == p.PatientICNFK).Select(e => HydrateDTO.HydrateEpisodeOfCare(e)).ToListAsync();
          thisPatient.CareEpisodes = episodes;
        }
        return Ok(thisPatient);
      }
    }

    /// <summary>
    /// get individual patient by episode since patient ID and patient name cannot be used in querystring
    /// </summary>
    /// <param name="episodeID"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // GET: api/Patients/5
    [HttpGet("Episode/{episodeID}")]
    public async Task<ActionResult<PatientDTO>> GetPatient(int episodeID)
    {
      var thisEpisode = _episodeOfCareRepository.FindByCondition(x => x.EpisodeOfCareID == episodeID).FirstOrDefault();
      var thisPatient = await _cacheHelper.GetPatientByEpisode(_episodeOfCareRepository, _patientRepository, episodeID);
      if (thisPatient == null)
      {
        return NotFound();
      }
      else
      {
        return Ok(thisPatient);
      }
    }
  }
}
