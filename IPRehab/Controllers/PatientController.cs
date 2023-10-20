using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
                PatientTreatingSpecialtyViewModel thisPatVM = new();
                thisPatVM.Patient = pat;

                //ToDo: encrypt the SSN, only when patient has no existing episode
                //thisPatVM.Patient.PatientICN = pat.PatientICN;
                //thisPatVM.Patient.PTFSSN = pat.PTFSSN;//.Substring(pat.PTFSSN.Length - 4, 4);

                //PatientDTOTreatingSpecialty.CareEpisodes property is initialized with new() as empty list so we need to check if it is null
                //if null then no episode is created yet
                if (!pat.CareEpisodes.Any() || (pat.CareEpisodes.Count() == 1 && pat.CareEpisodes.First() == null))
                {
                    //inside the foreach PatientDTOTreatingSpecialty loop, each episode must have its own episodeCommandBtn and must be initiate here
                    RehabActionViewModel episodeCommandBtn = new()
                    {
                        //since no episode ID we have to use patient ID to find patient
                        HostingPage = "Patient",
                        PatientID = pat.PatientICN,
                        EnableThisPatient = true,
                        SearchCriteria = searchCriteria,
                        PageNumber = pageNumber,
                        OrderBy = orderBy,
                    };
                    /* add NEW button to create episode for each admission */
                    PatientEpisodeAndCommandVM thisEpisodeBtnConfig = new();   //the button set must be initiated here for each admit date
                    episodeCommandBtn.EpisodeID = -1;   //New episode
                    episodeCommandBtn.AdmitDate = pat.AdmitDates.FirstOrDefault();
                    thisEpisodeBtnConfig.AdmissionDate = pat.AdmitDates.FirstOrDefault();
                    thisEpisodeBtnConfig.ActionButtonVM = episodeCommandBtn;
                    thisEpisodeBtnConfig.PatientIcnFK = pat.PatientICN;
                    thisPatVM.EpisodeBtnConfig.Add(thisEpisodeBtnConfig);
                }
                else
                {
                    foreach (DateTime thisAdmission in pat.AdmitDates)
                    {
                        //inside the foreach DateTime loop, each episode must have its own episodeCommandBtn and must be initiate here
                        RehabActionViewModel episodeCommandBtn = new()
                        {
                            //since no episode ID we have to use patient ID to find patient
                            HostingPage = "Patient",
                            PatientID = pat.PatientICN,
                            EnableThisPatient = true,
                            SearchCriteria = searchCriteria,
                            PageNumber = pageNumber,
                            OrderBy = orderBy,
                        };

                        PatientEpisodeAndCommandVM thisEpisodeBtnConfig = new();   //the button set must be initiated here for each admit date
                        episodeCommandBtn.AdmitDate = thisAdmission;
                        thisEpisodeBtnConfig.AdmissionDate = thisAdmission;

                        var thisEpisode = pat.CareEpisodes.SingleOrDefault(x => x.AdmissionDate == thisAdmission);
                        if (thisEpisode == null)
                        {
                            /* add NEW button to create episode for each admission */
                            episodeCommandBtn.EpisodeID = -1;   //New episode
                        }
                        else
                        {
                            episodeCommandBtn.EpisodeID = thisEpisode.EpisodeOfCareID;  //existing episode
                            thisEpisodeBtnConfig.EpisodeOfCareID = thisEpisode.EpisodeOfCareID;
                            thisEpisodeBtnConfig.OnsetDate = thisEpisode.OnsetDate;
                            thisEpisodeBtnConfig.FormIsComplete = thisEpisode.FormIsComplete;
                        }
                        thisEpisodeBtnConfig.ActionButtonVM = episodeCommandBtn;
                        thisEpisodeBtnConfig.PatientIcnFK = pat.PatientICN;
                        thisPatVM.EpisodeBtnConfig.Add(thisEpisodeBtnConfig);
                    }
                }
                patientListVM.Patients.Add(thisPatVM);
            }
            return View("IndexTreatingSpecialty", patientListVM);
        }
    }
}
