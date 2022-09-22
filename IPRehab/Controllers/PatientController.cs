using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
    //ToDo: [Authorize]
    public class PatientController : BaseController
    {
        public PatientController(IWebHostEnvironment environment, ILogger<PatientController> logger, IConfiguration configuration)
          : base(environment, configuration, logger)
        {
        }

        // GET: PatientController
        public async Task<ActionResult> Index(string searchCriteria, int pageNumber, string orderBy)
        {
            searchCriteria = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(searchCriteria));

            string sessionCriteria;
            string sessionKey = "SearchCriteria";
            CancellationToken cancellationToken = new();
            await HttpContext.Session.LoadAsync(cancellationToken);
            sessionCriteria = HttpContext.Session.GetString(sessionKey);
            if (searchCriteria != sessionCriteria)
            {
                if (string.IsNullOrEmpty(searchCriteria))
                    HttpContext.Session.Remove(sessionKey);
                else
                    HttpContext.Session.SetString(sessionKey, searchCriteria);
            }

            string url;
            string currentUserID = ViewBag.CurrentUserID;
            //Sending request to find web api REST service resource FSODPatient using HttpClient in the APIAgent
            if (!string.IsNullOrEmpty(currentUserID))
            {
                url = $"{ApiBaseUrl}/api/FSODPatient?networkID={currentUserID}&criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={base.PageSize}";

                /* ToDo: to be deleted after test */
                //url = $"{apiBaseUrl}/api/TestFSODPatient?networkID={currentUserID}&criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={_pageSize}";
            }
            else
            {
                url = $"{ApiBaseUrl}/api/FSODPatient?criteria={searchCriteria}&withEpisode=true&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={base.PageSize}";

                /* ToDo: to be deleted after test */
                //url = $"{apiBaseUrl}/api/TestFSODPatient?criteria={searchCriteria}&withEpisode=true&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={_pageSize}";
            }

            IEnumerable<PatientDTO> patients;

            //patients = await SerializationGeneric<IEnumerable<PatientDTO>>.SerializeAsync(url, base.BaseOptions);
            patients = await NewtonSoftSerializationGeneric<IEnumerable<PatientDTO>>.DeserializeAsync(url);
            patients = patients.OrderBy(x => x.Name);
            PatientListViewModel patientListVM = new();
            patientListVM.TotalPatients = patients.Count();
            patientListVM.PageTitle = "In-patient Rehab";
            patientListVM.PageSysTitle = "In_Patient_Rehab";
            patientListVM.SearchCriteria = searchCriteria;
            patientListVM.PageNumber = pageNumber;
            patientListVM.OrderBy = orderBy;

            if (patients == null || !patients.Any())
            {
                patientListVM.TotalPatients = 0;
                return View("_NoDataPartial", patientListVM);
            }
            else
            {
                foreach (PatientDTO pat in patients)
                {
                    PatientViewModel thisPatVM = new();
                    thisPatVM.Patient = pat;

                    //don't use FSODSSN, it may be null
                    string rawSSN = pat.PTFSSN;

                    //ToDo: encrypt the SSN, only when patient has no existing episode
                    thisPatVM.Patient.PTFSSN = rawSSN.Substring(rawSSN.Length - 4, 4);

                    if (!pat.CareEpisodes.Any())
                    {
                        RehabActionViewModel episodeCommandBtn = new()
                        {
                            //since no episode ID we have to use patient ID to find patient
                            PatientID = rawSSN,
                            HostingPage = "Patient",
                            SearchCriteria = searchCriteria,
                            PageNumber = pageNumber,
                            OrderBy = orderBy,
                            EpisodeID = -1,
                            EnableThisPatient = false
                        };

                        PatientEpisodeAndCommandVM thisEpisodeAndCommands = new();
                        //Don't assign episode properties for patient without episode
                        thisEpisodeAndCommands.ActionButtonVM = episodeCommandBtn;
                        thisPatVM.EpisodeBtnConfig.Add(thisEpisodeAndCommands);
                    }
                    else
                    {
                        foreach (EpisodeOfCareDTO episode in pat.CareEpisodes)
                        {
                            episode.PatientIcnFK = rawSSN.Substring(rawSSN.Length - 4, 4);

                            RehabActionViewModel episodeCommandBtn = new()
                            {
                                //to avoid exposing PHI/PII, leave the PatientID blank and use the EpisodeID to search for patient ID
                                PatientID = string.Empty,
                                HostingPage = "Patient",
                                SearchCriteria = searchCriteria,
                                PageNumber = pageNumber,
                                OrderBy = orderBy,
                                EpisodeID = episode.EpisodeOfCareID,
                                EnableThisPatient = true
                            };

                            //PatientEpisodeAndCommandVM derivedClass = episode as PatientEpisodeAndCommandVM;

                            PatientEpisodeAndCommandVM thisEpisodeAndCommands = new()
                            {
                                EpisodeOfCareID = episode.EpisodeOfCareID,
                                OnsetDate = episode.OnsetDate,
                                AdmissionDate = episode.AdmissionDate,
                                PatientIcnFK = episode.PatientIcnFK,
                                FormIsComplete = episode.FormIsComplete,
                                ActionButtonVM = episodeCommandBtn
                            };

                            thisPatVM.EpisodeBtnConfig.Add(thisEpisodeAndCommands);
                        }
                    }
                    patientListVM.Patients.Add(thisPatVM);
                }

                //returning the question list to view  
                return View(patientListVM);
            }
        }

        public async Task<ActionResult> IndexTreatingSpecailty(string searchCriteria, int pageNumber, string orderBy)
        {
            searchCriteria = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(searchCriteria));

            string sessionSearchCriteria;
            string sessionKey = "SearchCriteria";
            CancellationToken cancellationToken = new();
            await HttpContext.Session.LoadAsync(cancellationToken);
            sessionSearchCriteria = HttpContext.Session.GetString(sessionKey);
            if (searchCriteria != sessionSearchCriteria)
            {
                if (string.IsNullOrEmpty(searchCriteria))
                    HttpContext.Session.Remove(sessionKey);
                else
                    HttpContext.Session.SetString(sessionKey, searchCriteria);
            }

            string url;
            string currentUserID = ViewBag.CurrentUserID;
            //Sending request to find web api REST service resource FSODPatient using HttpClient in the APIAgent
            if (!string.IsNullOrEmpty(currentUserID))
            {
                url = $"{ApiBaseUrl}/api/TreatingSpecialtyPatient?networkID={currentUserID}&criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={base.PageSize}";

            }
            else
            {
                url = $"{ApiBaseUrl}/api/TreatingSpecialtyPatient?criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={base.PageSize}";
            }

            PatientTreatingSpecialtyListViewModel patientListVM = new()
            {
                PageTitle = "In-patient Rehab",
                PageSysTitle = "In_Patient_Rehab",
                SearchCriteria = searchCriteria,
                PageNumber = pageNumber,
                OrderBy = orderBy
            };

            IEnumerable<PatientDTOTreatingSpecialty> patients = null;
            string sessionAllPatients = HttpContext.Session.GetString(sessionKey);
            if (string.IsNullOrEmpty(sessionAllPatients))
            {
                patients = await SerializationGeneric<IEnumerable<PatientDTOTreatingSpecialty>>.DeserializeAsync(url, base.BaseOptions);
                //patients = await NewtonSoftSerializationGeneric<IEnumerable<PatientDTOTreatingSpecialty>>.DeserializeAsync(url);

                //Session["EveryFacilityPatients"] = patients;
            }
            //else
            //{
            //    patients = HttpContext.Session.GetString(sessionAllPatients);
            //}

            patients = patients.OrderBy(x => x.Name);
            patientListVM.TotalPatients = patients.Count();
            if (patientListVM.TotalPatients == 0)
            {
               return View("NoDataTreatingSpecialty", patientListVM);
            }
            else
            {
                foreach (PatientDTOTreatingSpecialty pat in patients)
                {
                    PatientTreatingSpecialtyViewModel thisPatVM = new();
                    thisPatVM.Patient = pat;

                    //don't use FSODSSN, it may be null
                    string rawSSN = pat.PTFSSN;

                    //ToDo: encrypt the SSN, only when patient has no existing episode
                    thisPatVM.Patient.PTFSSN = rawSSN.Substring(rawSSN.Length - 4, 4);


                    if (!pat.CareEpisodes.Any())
                    /* no episode */
                    {
                        RehabActionViewModel episodeCommandBtn = new()
                        {
                            //since no episode ID we have to use patient ID to find patient
                            PatientID = rawSSN,
                            HostingPage = "Patient",
                            SearchCriteria = searchCriteria,
                            PageNumber = pageNumber,
                            OrderBy = orderBy,
                            EpisodeID = -1,
                            EnableThisPatient = false
                        };

                        PatientEpisodeAndCommandVM thisEpisodeAndCommands = new();
                        //Don't assign episode properties for patient without episode
                        thisEpisodeAndCommands.ActionButtonVM = episodeCommandBtn;
                        thisPatVM.EpisodeBtnConfig.Add(thisEpisodeAndCommands);
                    }
                    else
                    /* has episode */
                    {
                        foreach (EpisodeOfCareDTO episode in pat.CareEpisodes)
                        {
                            episode.PatientIcnFK = rawSSN.Substring(rawSSN.Length - 4, 4);

                            RehabActionViewModel episodeCommandBtn = new()
                            {
                                //to avoid exposing PHI/PII, leave the PatientID blank and use the EpisodeID to search for patient ID
                                PatientID = string.Empty,
                                HostingPage = "Patient",
                                SearchCriteria = searchCriteria,
                                PageNumber = pageNumber,
                                OrderBy = orderBy,
                                EpisodeID = episode.EpisodeOfCareID,
                                EnableThisPatient = true
                            };

                            //PatientEpisodeAndCommandVM derivedClass = episode as PatientEpisodeAndCommandVM;

                            PatientEpisodeAndCommandVM thisEpisodeAndCommands = new()
                            {
                                EpisodeOfCareID = episode.EpisodeOfCareID,
                                OnsetDate = episode.OnsetDate,
                                AdmissionDate = episode.AdmissionDate,
                                PatientIcnFK = episode.PatientIcnFK,
                                FormIsComplete = episode.FormIsComplete,
                                ActionButtonVM = episodeCommandBtn
                            };

                            thisPatVM.EpisodeBtnConfig.Add(thisEpisodeAndCommands);
                        }
                    }
                    patientListVM.Patients.Add(thisPatVM);
                }

                return View("IndexTreatingSpecialty", patientListVM);
            }
        }

        // POST: PatientController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //  return View();
        //}
    }
}
