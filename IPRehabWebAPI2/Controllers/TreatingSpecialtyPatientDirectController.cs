using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Controllers
{
    /// <summary>
    /// Instread of using link server, directly gets Treating Specialty patients from BI13 server
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TreatingSpecialtyPatientDirectController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEpisodeOfCareRepository _episodeOfCareRepository;
        private readonly IUserPatientCacheHelper _cacheHelper;
        private readonly IUserPatientCacheHelper_TreatingSpecialty _cacheHelper_TreatingSpeciality;

        protected string CurrentNetworkID { get; set; }

        public TreatingSpecialtyPatientDirectController(
            IEpisodeOfCareRepository episodeOfCareRepository,
            IUserPatientCacheHelper cacheHelper,
            IUserPatientCacheHelper_TreatingSpecialty cacheHelper_TreatingSpeciality,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _episodeOfCareRepository = episodeOfCareRepository;
            _cacheHelper = cacheHelper;
            _cacheHelper_TreatingSpeciality = cacheHelper_TreatingSpeciality;
        }

        /// <summary>
        /// directly from BI13 TreatingSpecailty DB the patients matching the search criteria with visibility by the user facility level
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
            string Impersonated = _configuration.GetSection("AppSettings").GetValue<string>("Impersonate");

            if (string.IsNullOrEmpty(Impersonated))
                CurrentNetworkID = string.IsNullOrEmpty(networkID) ? HttpContext.User.Identity.Name : networkID;
            else
            {
                //ToDo: use Registration to authorize user.  Otherwise, any string in the currentUser parameter containing a valid network ID will be parameterized for cacheHelper.GetUserAccessLevels()

                //internally retrieve windows identity from User.Claims
                //string networkName = string.IsNullOrEmpty(impersonatedUserName) ? HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value : impersonatedUserName;
                CurrentNetworkID = Impersonated;
            }

            //get all patient with criteria and quarter filter
            List<PatientDTOTreatingSpecialty> facilityPatients;
            var distinctUserFacilities = await _cacheHelper.GetUserAccessLevels(CurrentNetworkID);
            if (distinctUserFacilities == null || !distinctUserFacilities.Any())
            {
                return NotFound($"{networkID}.You do not have permission to view any facility patients");
                //return BadRequest($"{networkID}.You do not have permission to view any facility patients");
            }

            facilityPatients = await _cacheHelper_TreatingSpeciality.GetPatients(distinctUserFacilities, criteria, orderBy, pageNumber, pageSize, null);


            if (facilityPatients == null || !facilityPatients.Any())
            {
                string permittedFacilities = String.Join(',', distinctUserFacilities.Select(f => f.Facility));
                string noDataMessage = $"No in-patient rehab in your facilities ({permittedFacilities})";

                //Response.Headers.Location = new Uri("NoData").ToString();
                //return new ContentResult
                //{
                //    StatusCode = 302,
                //    Content = noDataMessage
                //};

                //return NotFound(noDataMessage);
                return NoContent();
                //return BadRequest();
                //facilityPatients = new();
                //return Ok(facilityPatients);
            }
            else
            {
                //perform client list sort here, not before this step
                facilityPatients = [.. facilityPatients.OrderBy(p => p.Name)];


                //inner join
                //var facilityPatientEpisodes1 = facilityPatients.Join(_episodeOfCareRepository.FindAll(),
                //    p => p.PTFSSN, e => e.PatientICNFK,
                //    (p, e) => new
                //    {
                //        patient = p,
                //        episode = HydrateDTO.HydrateEpisodeOfCare(e)
                //    }).ToList();


                //left join with GroupJoin()
                //var patientEpisodeJoin = facilityPatients.GroupJoin(_episodeOfCareRepository.FindAll(),
                //    p => p.PTFSSN, 
                //    e => e.PatientICNFK, 
                //    (p, e) => new { p, e })
                //    .SelectMany(x => x.e.DefaultIfEmpty(), (p, e) => new { Patient = p.p, Episode = e == null ? null : HydrateDTO.HydrateEpisodeOfCare(e) });

                //LINQ join operators(Join, GroupJoin) support only equi-joins. All other join types have to be implemented as correlated subqueries.

                //left join (correlated subqueries) with SelectMany
                var patientLeftJoinEpisode = facilityPatients.SelectMany(
                    p => _episodeOfCareRepository.FindAll().Where(e => p.PTFSSN == e.PatientICNFK) /* must join on p.PTFSSN and e.PatientICNFK */
                    .DefaultIfEmpty(),
                    (p, e) => new
                    {
                        Patient = p,
                        Episode = e == null ? null : HydrateDTO.HydrateEpisodeOfCare(e)
                    });

                List<PatientDTOTreatingSpecialty> jaggedPatient = [];
                var uniquePatients = patientLeftJoinEpisode.Select(pe => pe.Patient).DistinctBy(p => p.RealSSN);
                var episodes = patientLeftJoinEpisode.Where(pe => pe.Episode != null).Select(pe => pe.Episode).ToList();

                if (!withEpisode || !episodes.Any())
                {
                    jaggedPatient.AddRange(facilityPatients);
                }
                else
                {
                    foreach (var pat in uniquePatients)
                    {
                        var thisPatientEpisodes = episodes.Where(episode => episode.PatientIcnFK == pat.PTFSSN).ToList();
                        if (thisPatientEpisodes.Any())
                        {
                            pat.CareEpisodes.Clear();
                            pat.CareEpisodes.AddRange(thisPatientEpisodes.OrderBy(x => x.AdmissionDate));
                        }
                        //cannot use DistinctBy() and cannot cast to List<EpisodeOfCareDTO>
                        //thisPatient.CareEpisodes = (List<EpisodeOfCareDTO>)thisPatient.CareEpisodes.DistinctBy(x => x.EpisodeOfCareID);

                        jaggedPatient.Add(pat);
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
            CurrentNetworkID = string.IsNullOrEmpty(networkID) ? HttpContext.User.Identity.Name : networkID;

            string criteria = string.Empty; //no criteria is needed for patient search based on id

            var distinctUserFacilities = await _cacheHelper.GetUserAccessLevels(CurrentNetworkID);

            List<PatientDTOTreatingSpecialty> facilityPatients = await _cacheHelper_TreatingSpeciality.GetPatients(distinctUserFacilities, criteria, orderBy, pageNumber, pageSize, patientID);
            if (facilityPatients == null || !facilityPatients.Any())
            {
                return NotFound($"The patient {patientID} is not found in your facility");
                //return BadRequest($"The patient {patientID} is not found in your facility");
            }
            else
            {
                PatientDTOTreatingSpecialty thisPatient = facilityPatients.First();
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
                return BadRequest("Episode ID must be greater than 0");
            }
            var thisEpisode = _episodeOfCareRepository.FindByCondition(x => x.EpisodeOfCareID == episodeID).FirstOrDefault();

            if (thisEpisode == null)
            {
                return NotFound($"The episode ({episodeID}) is not found");
                //return BadRequest($"The episode ({episodeID}) is not found");
            }
            var thisPatient = await _cacheHelper_TreatingSpeciality.GetPatientByEpisode(episodeID);
            if (thisPatient == null)
            {
                return NotFound($"This episode ({episodeID}) doesn't contain patient {thisEpisode.PatientICNFK} (scrssnt)");
                //return BadRequest($"This episode ({episodeID}) doesn't contain patient {thisEpisode.PatientICNFK} (scrssnt)");
            }
            else
            {
                return Ok(thisPatient);
            }
        }
    }
}
