using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
    //ToDo: [Authorize]
    [ResponseCache(CacheProfileName = "PrivateCache")]
    public class PatientController : BaseController
    {
        public PatientController(IWebHostEnvironment environment, IMemoryCache memoryCache, IConfiguration configuration)
          : base(environment, memoryCache, configuration)
        {
        }

        // GET: PatientController
        public async Task<ActionResult> IndexTreatingSpecailty(string searchCriteria, int pageNumber, string orderBy, int scrollToPatientID)
        {
            searchCriteria = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(searchCriteria));

            string currentUserID = ViewBag.CurrentUserID;

            string webApiEndpoint;

            IEnumerable<PatientDTOTreatingSpecialty> patients = null;

            if (!string.IsNullOrEmpty(currentUserID))
            {
                webApiEndpoint = $"{ApiBaseUrl}/api/{base._TreatingSpecialtyApiControllerName}?networkID={currentUserID}&criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={base.PageSize}";
            }
            else
            {
                webApiEndpoint = $"{ApiBaseUrl}/api/{base._TreatingSpecialtyApiControllerName}?criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={base.PageSize}";
            }

            //Sending request to find web api REST service resource TreatingSpecialtyPatientController using HttpClient in the APIAgent
            patients = await SerializationGeneric<IEnumerable<PatientDTOTreatingSpecialty>>.DeserializeAsync(webApiEndpoint, base.BaseOptions);
            //patients = await NewtonSoftSerializationGeneric<IEnumerable<PatientDTOTreatingSpecialty>>.DeserializeAsync(webApiEndpoint);


            //patients = patients.OrderBy(x => x.Name);

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

            PatientTreatingSpecialtyListViewModel patientListViewModel = new()
            {
                PageTitle = "In-patient Rehab",
                PageSysTitle = "In_Patient_Rehab",
                SearchCriteria = searchCriteria,
                PageNumber = pageNumber,
                OrderBy = orderBy,
                TotalPatients = patients.Count()
            };

            if (patientListViewModel.TotalPatients == 0)
            {
                return View("NoDataTreatingSpecialty", patientListViewModel);
            }

            int tmpCounter = 0;
            foreach (PatientDTOTreatingSpecialty pat in patients)
            {
                tmpCounter++;
                //inside the foreach PatientDTOTreatingSpecialty loop, each episode must have its own episodeCommandBtn and must be new() here

                //ToDo: encrypt the SSN when patient has no existing episode

                PatientTreatingSpecialtyViewModel thisPatVM = new();
                thisPatVM.Patient = pat;

                PatientEpisodeAndCommandVM thisEpisodeBtnConfig = null;
                var admitDatesInPat = pat.AdmitDates.Distinct();    //pat admissions may be different than the episode admissions
                foreach (DateTime thisAdmission in admitDatesInPat)
                {
                    /* must NEW button to create episode for each admission or some admission but not episode yet */
                    thisEpisodeBtnConfig = new();   //the button set must be new() here for each admit date
                    var EpisodesWithThisAdmitDate = pat.CareEpisodes.Where(e => e.AdmissionDate == thisAdmission).ToList();  //existing episodes may be duplicated due to the change from HealtherFactor to TreatingSpecialty cubes 

                    if (EpisodesWithThisAdmitDate == null || EpisodesWithThisAdmitDate.Count() == 0)
                    {
                        RehabActionViewModel episodeCommandBtn = new()
                        {
                            //since no episode ID we have to use patient ID to find patient
                            HostingPage = "Patient",
                            PatientID = pat.PTFSSN,
                            EnableThisPatient = true,
                            SearchCriteria = searchCriteria,
                            PageNumber = pageNumber,
                            OrderBy = orderBy,
                            EpisodeID = -1,   //New episode
                            AdmitDate = thisAdmission
                        };

                        thisEpisodeBtnConfig.ActionButtonVM = episodeCommandBtn;
                        thisEpisodeBtnConfig.AdmissionDate = thisAdmission;
                        thisEpisodeBtnConfig.PatientIcnFK = pat.PTFSSN;
                    }
                    else
                    {
                        foreach (var thisEpisode in EpisodesWithThisAdmitDate)   //existing episodes may be duplicated due to the change from HealtherFactor to TreatingSpecialty cubes 
                        {
                            RehabActionViewModel episodeCommandBtn = new()
                            {
                                HostingPage = "Patient",
                                PatientID = pat.PTFSSN,
                                EnableThisPatient = true,
                                SearchCriteria = searchCriteria,
                                PageNumber = pageNumber,
                                OrderBy = orderBy,
                                EpisodeID = thisEpisode.EpisodeOfCareID,
                                AdmitDate = thisEpisode.AdmissionDate   //could be duplicated admission. old duplicated episodes needs to be deleted
                            };

                            thisEpisodeBtnConfig.ActionButtonVM = episodeCommandBtn;
                            thisEpisodeBtnConfig.AdmissionDate = thisAdmission;
                            thisEpisodeBtnConfig.PatientIcnFK = pat.PTFSSN;

                            thisEpisodeBtnConfig.EpisodeOfCareID = thisEpisode.EpisodeOfCareID;
                            thisEpisodeBtnConfig.OnsetDate = thisEpisode.OnsetDate;
                            thisEpisodeBtnConfig.FormIsComplete = thisEpisode.FormIsComplete;
                        }
                    }

                    thisPatVM.EpisodeBtnConfig.Add(thisEpisodeBtnConfig);
                }
                patientListViewModel.Patients.Add(thisPatVM);
            }
            return View("IndexTreatingSpecialty", patientListViewModel);
        }
    }
}
