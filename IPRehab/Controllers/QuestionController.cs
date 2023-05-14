using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    public class QuestionController : BaseController
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IUserPatientCacheHelper _userPatientCacheHelper;
        public QuestionController(IWebHostEnvironment environment, IMemoryCache memoryCache, IConfiguration configuration,
            IUserPatientCacheHelper userPatientCacheHelper)
          : base(environment, memoryCache, configuration)
        {
            _memoryCache = memoryCache;
            _userPatientCacheHelper = userPatientCacheHelper;
        }

        /// <summary>
        /// https://www.stevejgordon.co.uk/sending-and-receiving-json-using-httpclient-with-system-net-http-json
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="patientID"></param>
        /// <param name="episodeID"></param>
        /// <param name="searchCriteria"></param>
        /// <param name="pageNumber"></param>
        /// <param name="orderBy"></param>
        /// <param name="admitDate"></param>
        /// <returns></returns> 
        public async Task<IActionResult> Edit(string stage, string patientID, int episodeID, string searchCriteria, int pageNumber, string orderBy, string admitDate)
        {
            string healthFactoreApiEndpoint = string.Empty;
            string treatingSpecialtyEndpoint = string.Empty;
            string questionApiEndpoint = string.Empty;
            string facilityID = string.Empty;
            string currentUserID = ViewBag.CurrentUserID;
            string patientName = string.Empty;
            stage = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(stage));


            //to enforce PHI/PII, no patient ID nor patient name can be used in querystring
            //so use episode id to search for the target patient

            PatientDTOTreatingSpecialty patientstreatingSpecialty = await PatientsFromTreatingSpecialty(episodeID, patientID, currentUserID);

            if (patientstreatingSpecialty == null) {
                return PartialView("_QestionNoPatient");
            }

            patientID = patientstreatingSpecialty.PTFSSN;
            patientName = patientstreatingSpecialty.Name;
            facilityID = patientstreatingSpecialty.Sta6a;

            string stageTitle = string.IsNullOrEmpty(stage) ? "Full" : (stage == "Followup" ? "Follow Up" : (stage == "Base" ? "Episode Of Care" : $"{stage}"));
            string action = nameof(Edit);
            bool includeAnswer = (action == "Edit");
            if (stage == "New")
            {
                includeAnswer = false;
            }

            List<QuestionDTO> questions = new();

            questionApiEndpoint = stage switch
            {
                null or "" or "Full" => $"{ApiBaseUrl}/api/Question/GetAll?includeAnswer={includeAnswer}&episodeID={episodeID}",
                _ => $"{ApiBaseUrl}/api/Question/GetStageAsync/{stage}?includeAnswer={includeAnswer}&episodeID={episodeID}&admitDate={admitDate}",
            };

            questions = await SerializationGeneric<List<QuestionDTO>>.DeserializeAsync($"{questionApiEndpoint}", base.BaseOptions);

            string actionBtnColor = EpisodeCommandButtonSettings.CommandBtnConfigDictionary[stage].ButtonCss;

            RehabActionViewModel episodeCommandBtn = new()
            {
                HostingPage = "Question",
                SearchCriteria = searchCriteria,
                PageNumber = pageNumber,
                EpisodeID = episodeID
            };

            if (DateTime.TryParse(System.Web.HttpUtility.UrlDecode(admitDate), out DateTime thisDate))
                episodeCommandBtn.AdmitDate = thisDate;

            if (episodeID == -1)
            {
                episodeCommandBtn.EnableThisPatient = false;
                episodeCommandBtn.PatientID = patientID;
            }
            PatientEpisodeAndCommandVM thisEpisodeAndCommands = new()
            {
                //PatientEpisodeAndCommandVM inherit from EpisodeOfCareDTo so just explicit cast the episode instance
                ActionButtonVM = episodeCommandBtn
            };

            QuestionHierarchy qh = HydrateVM.HydrateHierarchically(questions, thisDate);
            qh.ReadOnly = false;
            qh.EpisodeID = episodeID;
            qh.StageTitle = stageTitle;
            qh.StageCode = stage;
            qh.FacilityID = facilityID;
            qh.PatientID = patientID;
            qh.PatientName = patientName;
            qh.EpisodeBtnConfig.Add(thisEpisodeAndCommands);
            qh.CurrentAction = $"{action} Mode";
            qh.ModeColorCssClass = actionBtnColor;
            qh.WebApiBaseUrl = ApiBaseUrl;
            return View(qh);
        }

        // POST: QuestionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] PostbackModel postbackModel)
        {
            if (ModelState.IsValid)
            {
                //List<UserAnswer> newAnswers = postbackModel.NewAnswers;
                //List<UserAnswer> oldAnswers = postbackModel.OldAnswers;
                //List<UserAnswer> updatedAnswers = postbackModel.UpdatedAnswers;

                //forward the postbackModel to web api Answer controller
                Uri uri = new($"{ApiBaseUrl}/api/Answer/Post");

                var Res = await APIAgent.PostDataAsync(uri, postbackModel);
                if (Res.IsSuccessStatusCode)
                {
                    Response.StatusCode = (int)Res.StatusCode;
                    //return Json("Data saved successfully");
                    return new JsonResult("Data saved successfully");
                }
                else
                {
                    Response.StatusCode = (int)Res.StatusCode;
                    return new JsonResult($"Data not saved. {Res}");
                }
            }
            else
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult("Data not saved.  The post back model is not valid.");
            }
        }

        // GET: QuestionController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //  return View();
        //}

        // POST: QuestionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            return RedirectToAction(nameof(Edit));
        }

        private async Task<PatientDTOTreatingSpecialty> PatientsFromTreatingSpecialty(int episodeID, string patientID, string currentUserID)
        {
            List<MastUserDTO> thisUserAccessLevel = await _userPatientCacheHelper.GetUserAccessLevels(currentUserID);

            if (thisUserAccessLevel == null || !thisUserAccessLevel.Any())
            {
                return null;    //no access
            }

            //PatientDTOTreatingSpecialty patient = GetFromMemoryCache(CacheKeys.CacheKeyAllPatients, patientID);
            var thisFacilityPatients = await _userPatientCacheHelper.GetThisFacilityPatients(thisUserAccessLevel);
            if (thisFacilityPatients == null || !thisFacilityPatients.Any())
            {
                return null;    //no patient in this facility 
            }

            var inFacilityPatient = thisFacilityPatients.FirstOrDefault(p => p.scrssn.ToString() == patientID || p.PatientICN == patientID);
            if (inFacilityPatient != null)
            {
                PatientDTOTreatingSpecialty thisPatient = new()
                {
                    Sta6a = inFacilityPatient.bsta6a,
                    Name = inFacilityPatient.PatientName,
                    PTFSSN = inFacilityPatient.scrssn.Value.ToString(),
                    PatientICN = inFacilityPatient.PatientICN,
                    DoB = inFacilityPatient.DoB,
                    Bedsecn = inFacilityPatient.bedsecn
                };
                return thisPatient;
            }
            else
            {
                PatientDTOTreatingSpecialty thisPatient = null;
                string webAPIendpoint;

                if (thisFacilityPatients == null || !thisFacilityPatients.Any())
                {
                    if (episodeID <= 0)
                    {
                        webAPIendpoint = $"{ApiBaseUrl}/api/TreatingSpecialtyPatient/{patientID}?networkID={currentUserID}&withEpisode=false&pageSize={base.PageSize}";
                    }
                    else
                    {
                        webAPIendpoint = $"{ApiBaseUrl}/api/TreatingSpecialtyPatient/Episode/{episodeID}";
                    }

                    thisPatient = await SerializationGeneric<PatientDTOTreatingSpecialty>.DeserializeAsync($"{webAPIendpoint}", base.BaseOptions);

                    _memoryCache.Set(CacheKeys.CacheKeyAllPatients, thisPatient, TimeSpan.FromDays(1));
                }

                return thisPatient;
            }
        }

        private PatientDTOTreatingSpecialty GetFromMemoryCache(string cacheKeyAllPatients, string patientID)
        {
            var memoryCachePatients = _memoryCache.Get<List<IPRehabModel.vTreatingSpecialtyRecent3Yrs>>(cacheKeyAllPatients);
            if (memoryCachePatients != null && memoryCachePatients.Any())
            {
                //get single patient matching the patientID
                var thisTS3rs = memoryCachePatients.FirstOrDefault(p => p.scrssn == int.Parse(patientID));

                if (thisTS3rs == null)
                    return null;

                PatientDTOTreatingSpecialty thisPatient = new()
                {
                    Sta6a = thisTS3rs.bsta6a,
                    Name = thisTS3rs.PatientName,
                    PTFSSN = thisTS3rs.scrssn.Value.ToString(),
                    PatientICN = thisTS3rs.PatientICN,
                    DoB = thisTS3rs.DoB,
                    Bedsecn = thisTS3rs.bedsecn
                };

                return thisPatient;
            }
            return null;
        }

        //private async Task<PatientDTOTreatingSpecialty> GetFromSession(string sessionKeyAllPatients, string patientID)
        //{
        //    string sessionAllPatients = HttpContext.Session.GetString(sessionKeyAllPatients);
        //    if (string.IsNullOrEmpty(sessionAllPatients))
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        var patients = await HttpContext.Session.Get(sessionKeyAllPatients)?.DeserializeAsync<List<IPRehabModel.vTreatingSpecialtyRecent3Yrs>>(base.BaseOptions);
        //        if (patients == null)
        //        {
        //            return null;
        //        }

        //        //get single patient matching the patientID
        //        var thisTS3rs = patients.FirstOrDefault(p => p.scrssn == int.Parse(patientID));

        //        if (thisTS3rs == null)
        //            return null;

        //        PatientDTOTreatingSpecialty thisPatient = new()
        //        {
        //            Sta6a = thisTS3rs.bsta6a,
        //            Name = thisTS3rs.PatientName,
        //            PTFSSN = thisTS3rs.scrssn.Value.ToString(),
        //            PatientICN = thisTS3rs.PatientICN,
        //            DoB = thisTS3rs.DoB,
        //            Bedsecn = thisTS3rs.bedsecn
        //        };

        //        return thisPatient;
        //    }
        //}
    }
}
