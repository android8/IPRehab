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
                    PatientID = pat.PatientICN,
                    EnableThisPatient = true,
                    SearchCriteria = searchCriteria,
                    PageNumber = pageNumber,
                    OrderBy = orderBy,
                };

                /* add NEW button for each admit date that has no associated episode */
                foreach (DateTime thisAdmission in pat.AdmitDates)
                {
                    episodeCommandBtn.AdmitDate = thisAdmission;

                    PatientEpisodeAndCommandVM thisEpisodeBtnConfig = new()
                    {
                        PatientIcnFK = pat.PatientICN,
                        AdmissionDate = thisAdmission
                    };


                    var episodeForThisAdmission = pat.CareEpisodes.Where(e => e.AdmissionDate == thisAdmission).FirstOrDefault();
                    if (episodeForThisAdmission == null)
                    {
                        episodeCommandBtn.EpisodeID = -1;   //New episode
                        thisEpisodeBtnConfig.ActionButtonVM = episodeCommandBtn;
                        thisPatVM.EpisodeBtnConfig.Add(thisEpisodeBtnConfig);
                    }
                    else
                    {
                        episodeCommandBtn.EpisodeID = episodeForThisAdmission.EpisodeOfCareID;  //existing episode
                        thisEpisodeBtnConfig.ActionButtonVM = episodeCommandBtn;
                        thisEpisodeBtnConfig.EpisodeOfCareID = episodeForThisAdmission.EpisodeOfCareID;
                        thisEpisodeBtnConfig.OnsetDate = episodeForThisAdmission.OnsetDate;
                        thisEpisodeBtnConfig.FormIsComplete = episodeForThisAdmission.FormIsComplete;
                        thisPatVM.EpisodeBtnConfig.Add(thisEpisodeBtnConfig);
                    }
                }
                patientListVM.Patients.Add(thisPatVM);
            }
            return View("IndexTreatingSpecialty", patientListVM);
        }
    }
}
