﻿using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
    //ToDo: [Authorize]
    [ResponseCache(CacheProfileName = "PrivateCache")]
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
            PatientListViewModel patientListVM = new()
            {
                TotalPatients = patients.Count(),
                PageTitle = "In-patient Rehab",
                PageSysTitle = "In_Patient_Rehab",
                SearchCriteria = searchCriteria,
                PageNumber = pageNumber,
                OrderBy = orderBy
            };

            if (patients == null || !patients.Any())
            {
                patientListVM.TotalPatients = 0;
                return View("_NoDataPartial", patientListVM);
            }
            else
            {
                foreach (PatientDTO pat in patients)
                {
                    PatientViewModel thisPatVM = new()
                    {
                        Patient = pat
                    };

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
                            AdmitDate = pat.AdmitDate,
                            EnableThisPatient = false
                        };

                        PatientEpisodeAndCommandVM thisEpisodeAndCommands = new()
                        {
                            //Don't assign episode properties for patient without episode
                            ActionButtonVM = episodeCommandBtn
                        };
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
                                AdmitDate = episode.AdmissionDate,
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

        public async Task<ActionResult> IndexTreatingSpecailty(string searchCriteria, int pageNumber, string orderBy, int scrollToPatientID)
        {
            searchCriteria = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(searchCriteria));

            string currentUserID = ViewBag.CurrentUserID;

            string webApiEndpoint;

            IEnumerable<PatientDTOTreatingSpecialty> patients = null;

            if (!string.IsNullOrEmpty(currentUserID))
            {
                webApiEndpoint = $"{ApiBaseUrl}/api/TreatingSpecialtyPatient?networkID={currentUserID}&criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={base.PageSize}";

            }
            else
            {
                webApiEndpoint = $"{ApiBaseUrl}/api/TreatingSpecialtyPatient?criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={base.PageSize}";
            }

            //Sending request to find web api REST service resource TreatingSpecialtyPatientController using HttpClient in the APIAgent
            patients = await SerializationGeneric<IEnumerable<PatientDTOTreatingSpecialty>>.DeserializeAsync(webApiEndpoint, base.BaseOptions);
            //patients = await NewtonSoftSerializationGeneric<IEnumerable<PatientDTOTreatingSpecialty>>.DeserializeAsync(webApiEndpoint);


             patients = patients.OrderBy(x => x.Name);

            //string sessionSearchCriteria;
            //CancellationToken cancellationToken = new();
            //await HttpContext.Session.LoadAsync(cancellationToken);
            //sessionSearchCriteria = HttpContext.Session.GetString("SearchCriteria");
            //if (searchCriteria != sessionSearchCriteria)
            //{
            //    if (string.IsNullOrEmpty(searchCriteria))
            //        HttpContext.Session.Remove("SearchCriteria");
            //    else
            //        HttpContext.Session.SetString("SearchCriteria", searchCriteria);
            //}            

            PatientTreatingSpecialtyListViewModel patientListVM = new()
            {
                PageTitle = "In-patient Rehab",
                PageSysTitle = "In_Patient_Rehab",
                SearchCriteria = searchCriteria,
                PageNumber = pageNumber,
                OrderBy = orderBy,
                TotalPatients = patients.Count()
            };

            if (patientListVM.TotalPatients == 0)
            {
                return View("NoDataTreatingSpecialty", patientListVM);
            }

            foreach (PatientDTOTreatingSpecialty pat in patients)
            {
                PatientTreatingSpecialtyViewModel thisPatVM = new()
                {
                    Patient = pat
                };

                //ToDo: encrypt the SSN, only when patient has no existing episode
                thisPatVM.Patient.PatientICN = pat.PatientICN;
                if (!string.IsNullOrEmpty(pat.PTFSSN))
                {
                    thisPatVM.Patient.PTFSSN = pat.PTFSSN;//.Substring(pat.PTFSSN.Length - 4, 4);
                }

                RehabActionViewModel episodeCommandBtn = new()
                {
                    //since no episode ID we have to use patient ID to find patient
                    HostingPage = "Patient",
                    SearchCriteria = searchCriteria,
                    PageNumber = pageNumber,
                    OrderBy = orderBy,
                };

                /* add new button for each admit date that has no associated episode */
                foreach (DateTime thisAdmission in pat.AdmitDates)
                {
                    var episodeForThisAdmission = pat.CareEpisodes.Where(e => e.AdmissionDate == thisAdmission).FirstOrDefault();
                    if (episodeForThisAdmission == null)
                    {
                        episodeCommandBtn.PatientID = pat.PatientICN;
                        episodeCommandBtn.EpisodeID = -1;   //Don't assign episode properties for patient without episode
                        episodeCommandBtn.EnableThisPatient = true;
                        episodeCommandBtn.AdmitDate = thisAdmission;
                        PatientEpisodeAndCommandVM thisEpisodeBtnConfig = new()
                        {
                            AdmissionDate = thisAdmission,
                            PatientIcnFK = pat.PatientICN,
                            ActionButtonVM = episodeCommandBtn
                        };
                        thisPatVM.EpisodeBtnConfig.Add(thisEpisodeBtnConfig);
                    }
                    else
                    {
                        //to avoid exposing PHI/PII, leave the PatientID blank and use the EpisodeID to search for patient ID
                        episodeCommandBtn.PatientID = pat.PatientICN;
                        episodeCommandBtn.EpisodeID = episodeForThisAdmission.EpisodeOfCareID;
                        episodeCommandBtn.EnableThisPatient = true;
                        episodeCommandBtn.AdmitDate= thisAdmission;

                        //PatientEpisodeAndCommandVM derivedClass = episode as PatientEpisodeAndCommandVM;
                        PatientEpisodeAndCommandVM thisEpisodeBtnConfig = new()
                        {
                            EpisodeOfCareID = episodeForThisAdmission.EpisodeOfCareID,
                            OnsetDate = episodeForThisAdmission.OnsetDate,
                            AdmissionDate = thisAdmission,
                            PatientIcnFK = episodeForThisAdmission.PatientIcnFK,
                            FormIsComplete = episodeForThisAdmission.FormIsComplete,
                            ActionButtonVM = episodeCommandBtn
                        };
                        thisPatVM.EpisodeBtnConfig.Add(thisEpisodeBtnConfig);
                    }
                }
                patientListVM.Patients.Add(thisPatVM);
            }
            return View("IndexTreatingSpecialty", patientListVM);
        }
    }
}
