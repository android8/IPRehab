using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TreatingSpecialtyPatientController : ControllerBase
    {
        private readonly IEpisodeOfCareRepository _episodeOfCareRepository;
        private readonly IUserPatientCacheHelper _cacheHelper;

        public TreatingSpecialtyPatientController(
            IEpisodeOfCareRepository episodeOfCareRepository, 
            IUserPatientCacheHelper cacheHelper)
        {
            _episodeOfCareRepository = episodeOfCareRepository;
            _cacheHelper = cacheHelper;
        }

        /// <summary>
        /// get patients matching the search criteria with visibility by the user facility level
        /// </summary>
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
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients(string networkID, string criteria, bool withEpisode, string orderBy, int pageNumber = 1, int pageSize = 50)
        {
            //ToDo: use Registration to authorize user.  Otherwise, any string in the currentUser parameter containing a valid network ID will be parameterized for  cacheHelper.GetUserAccessLevels()

            //internally retrieve windows identity from User.Claims
            //string networkName = string.IsNullOrEmpty(impersonatedUserName) ? HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value : impersonatedUserName;
            networkID = string.IsNullOrEmpty(networkID) ? HttpContext.User.Identity.Name : networkID;

            //get all patient with criteria and quarter filter
            List<PatientDTOTreatingSpecialty> facilityPatients;
            string patientID = string.Empty;
            facilityPatients = await _cacheHelper.GetPatients(networkID, criteria, orderBy, pageNumber, pageSize, patientID);

            if (facilityPatients == null || !facilityPatients.Any())
            {
                string noDataMesage = "No patient is found in your facility";

                //Response.Headers.Location = new Uri("NoData").ToString();
                //return new ContentResult
                //{
                //    StatusCode = 302,
                //    Content = noDataMesage
                //};

                return NotFound(noDataMesage);

                //facilityPatients = new();
                //return Ok(facilityPatients);
            }
            else
            {
                List<PatientDTOTreatingSpecialty> jaggedPatient = new();
                if (withEpisode)
                {
                    //inner join
                    //var facilityPatientEpisodes1 = facilityPatients.Join(_episodeOfCareRepository.FindAll(),
                    //    p => p.PTFSSN, e => e.PatientICNFK,
                    //    (p, e) => new
                    //    {
                    //        patient = p,
                    //        episode = HydrateDTO.HydrateEpisodeOfCare(e)
                    //    }).ToList();

                    //left join
                    var facilityPatientEpisodes = facilityPatients.GroupJoin(_episodeOfCareRepository.FindAll().ToList(),
                        p => p.PTFSSN, e => e.PatientICNFK, (p, e) => new { p, e })
                        .SelectMany(x => x.e.DefaultIfEmpty(), (p, e) => new { patient = p.p, episode = e == null ? null : HydrateDTO.HydrateEpisodeOfCare(e) })
                        .ToList();

                    foreach (var fpe in facilityPatientEpisodes)
                    {
                        if (fpe.episode != null)
                        {
                            fpe.patient.CareEpisodes.Add(fpe.episode);
                        }
                        jaggedPatient.Add(fpe.patient);
                    }
                }
                return Ok(jaggedPatient);
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
        [HttpGet("{patientID:int}")]
        public async Task<ActionResult<PatientDTOTreatingSpecialty>> GetPatient(string patientID, string networkID, bool withEpisode, string orderBy, int pageNumber = 1, int pageSize = 1)
        {
            networkID = string.IsNullOrEmpty(networkID) ? HttpContext.User.Identity.Name : networkID;

            string criteria = string.Empty; //no criteria is needed for patient search based on id
            List<PatientDTOTreatingSpecialty> patients = await _cacheHelper.GetPatients(networkID, criteria, orderBy, pageNumber, pageSize, patientID);
            if (patients == null || !patients.Any())
            {
                return NotFound($"The patient {patientID} is not found in your facility");
            }
            else
            {
                PatientDTOTreatingSpecialty thisPatient = patients.First();
                if (withEpisode)
                {
                    var episodes = _episodeOfCareRepository.FindByCondition(e => e.PatientICNFK == thisPatient.PatientICN || e.PatientICNFK == thisPatient.PTFSSN);
                    if (episodes != null)
                    {
                        List<EpisodeOfCareDTO> listOfEpisodes = new();
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
        /// </summary>
        /// <param name="episodeID"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // GET: api/Patients/5
        [HttpGet("Episode/{episodeID:int}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int episodeID)
        {
            if (episodeID <= 0)
            {
                return BadRequest("Episode ID must be greather than 0");
            }
            var thisEpisode = _episodeOfCareRepository.FindByCondition(x => x.EpisodeOfCareID == episodeID).FirstOrDefault();

            if (thisEpisode == null)
                return NotFound($"The episode ({episodeID}) is not found");

            var thisPatient = await _cacheHelper.GetPatientByEpisode(episodeID);
            if (thisPatient == null)
            {
                return NotFound($"This episode ({episodeID}) patient ICN ({thisEpisode.PatientICNFK}) does not match with any PatientICN and scrssn in the database");
            }
            else
            {
                return Ok(thisPatient);
            }
        }
    }
}
