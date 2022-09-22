using IPRehabModel;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TreatingSpecialtyPatientController : ControllerBase
    {
        private readonly ITreatingSpecialtyPatientRepository _treatingSpecialtyRepository;
        private readonly IEpisodeOfCareRepository _episodeOfCareRepository;
        private readonly IUserPatientCacheHelper _cacheHelper;

        public TreatingSpecialtyPatientController(ITreatingSpecialtyPatientRepository treatingSpecialtyRepository, IEpisodeOfCareRepository episodeOfCareRepository, IUserPatientCacheHelper cacheHelper)
        {
            _treatingSpecialtyRepository = treatingSpecialtyRepository;
            _episodeOfCareRepository = episodeOfCareRepository;
            _cacheHelper = cacheHelper;
        }

        /// <summary>
        /// get patients matching the search criteria with visibility by the user facility level
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="networkID"></param>
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
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients(string patientID, string networkID, string criteria, bool withEpisode, string orderBy, int pageNumber = 1, int pageSize = 50)
        {
            //ToDo: use Registration to authorize user.  Otherwise, any string in the currentUser parameter containing a valid network ID will be parameterized for  cacheHelper.GetUserAccessLevels()

            //internally retrieve windows identity from User.Claims
            //string networkName = string.IsNullOrEmpty(impersonatedUserName) ? HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value : impersonatedUserName;
            networkID = string.IsNullOrEmpty(networkID) ? HttpContext.User.Identity.Name : networkID;

            //ToDo: remove this hard coded network name
            //networkName = "VHALEBBIDELD";

            //get all patient with criteria and quarter filter
            List<PatientDTOTreatingSpecialty> patients;
            patients = await _cacheHelper.GetPatients(_treatingSpecialtyRepository, networkID, criteria, orderBy, pageNumber, pageSize, patientID);

            if (patients is null)
            {
                //string noDataMesage = "No patient is found in rehab in your facility in the past 90 days";

                //Response.Headers.Location = new Uri("NoData").ToString();
                //return new ContentResult
                //{
                //    StatusCode = 302,
                //    Content = noDataMesage
                //};

                //return NotFound(noDataMesage);
                patients = new();

                return Ok(patients);
            }
            else
            {
                if (withEpisode)
                {
                    foreach (PatientDTOTreatingSpecialty p in patients)
                    {
                        //p.CareEpisodes = new();
                        var episodes = _episodeOfCareRepository.FindByCondition(episode => episode.PatientICNFK == p.PTFSSN);
                        //.OrderBy(x => x.AdmissionDate).ToListAsync();

                        foreach (var episode in episodes)
                        {
                            EpisodeOfCareDTO thisEpisode = HydrateDTO.HydrateEpisodeOfCare(episode);
                            //hydrate and add the dates from the tblAnswer because they are more up-to-date than the episode dates
                            p.CareEpisodes.Add(thisEpisode);
                        }
                        p.CareEpisodes?.Sort();
                    }
                }
                return Ok(patients);
            }
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
        public async Task<ActionResult<PatientDTOTreatingSpecialty>> GetPatient(string patientID, string networkID, bool withEpisode, string orderBy, int pageNumber = 1, int pageSize = 1)
        {
            if (string.IsNullOrEmpty(networkID))
            {
                networkID = HttpContext.User.Identity.Name;
            }
            string criteria = string.Empty; //no criteria is needed for patient search based on id
            List<PatientDTOTreatingSpecialty> patients = await _cacheHelper.GetPatients(_treatingSpecialtyRepository, networkID, criteria, orderBy, pageNumber, pageSize, patientID);
            if (patients == null)
            {
                patients = new();
                return Ok(patients);
            }
            else
            {
                PatientDTOTreatingSpecialty thisPatient = patients.Find(x => x.PTFSSN == patientID || x.PatientICN == patientID);
                if (withEpisode)
                {
                    var episodes = _episodeOfCareRepository.FindByCondition(p => p.PatientICNFK == p.PatientICNFK);
                    if (episodes != null)
                    {
                        List<EpisodeOfCareDTO> listOfEpisodes = new List<EpisodeOfCareDTO>();
                        foreach (var episode in episodes)
                        {
                            listOfEpisodes.Add(HydrateDTO.HydrateEpisodeOfCare(episode));
                        }
                        thisPatient.CareEpisodes = listOfEpisodes;
                    }
                }
                return Ok(thisPatient);
            }
        }

        /// <summary>
        /// Get individual patient by episode since patient ID and patient name cannot be used in querystring
        /// For new episode use [HttpGet("{patientID}")] endpoint.
        /// This endpoint should not be used since the episodeID would be -1 which will not be found and  error out.
        /// </summary>
        /// <param name="episodeID"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // GET: api/Patients/5
        [HttpGet("Episode/{episodeID}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int episodeID)
        {
            var thisEpisode = _episodeOfCareRepository.FindByCondition(x => x.EpisodeOfCareID == episodeID);

            var thisPatient = await _cacheHelper.GetPatientByEpisode(_episodeOfCareRepository, _treatingSpecialtyRepository, episodeID);
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
