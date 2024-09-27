using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserModel;

namespace IPRehab.Controllers
{
    //ToDo: [Authorize]
    public class QuestionController : BaseController
    {
        //private readonly IMemoryCache _memoryCache;
        private readonly MasterreportsContext _masterReportsContext;
        private readonly IUserPatientCacheHelper _userPatientCacheHelper;
        private readonly IUserPatientCacheHelper_TreatingSpecialty _userPatientCacheHelper_TreatingSpecialty;

        #region public
        public QuestionController(IWebHostEnvironment environment, IMemoryCache memoryCache, IConfiguration configuration,
           MasterreportsContext masterReportsContext, IUserPatientCacheHelper_TreatingSpecialty userPatientCacheHelper_TreatingSpecialty, IUserPatientCacheHelper userPatientCacheHelper)
            : base(environment, memoryCache, configuration)
        {
            //_memoryCache = memoryCache;
            _masterReportsContext = masterReportsContext;
            _userPatientCacheHelper = userPatientCacheHelper;
            _userPatientCacheHelper_TreatingSpecialty = userPatientCacheHelper_TreatingSpecialty;
        }

        /// <summary>
        /// https://www.stevejgordon.co.uk/sending-and-receiving-json-using-httpclient-with-system-net-http-json
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="patientID"></param>
        /// <param name="episodeID"></param>
        /// <param name="searchCriteria"></param>
        /// <param name="pageNumber"></param>
        /// <param name="admitDate"></param>
        /// <returns></returns> 
        public async Task<IActionResult> Edit(string stage, string patientID, int episodeID, string searchCriteria, int pageNumber, string admitDate)
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

            var thisUserAccessLevel = await ValidateThisUserAccess(currentUserID);
            if (thisUserAccessLevel == null || !thisUserAccessLevel.Any())
            {
                return View("AccessDenied");
            }

            var thisPatient = await GetThisPatient(thisUserAccessLevel, episodeID, patientID, currentUserID);
            if (thisPatient == null)
            {
                return PartialView("_QestionNoPatient");
            }

            patientID = thisPatient.PTFSSN;
            patientName = thisPatient.Name;
            facilityID = thisPatient.Sta6a;

            string stageTitle = string.IsNullOrEmpty(stage) ? "Full" : (stage == "Followup" ? "Follow Up" : (stage == "Base" ? "Episode Of Care" : $"{stage}"));
            string action = nameof(Edit);
            bool includeAnswer = action == "Edit";
            if (stage == "New")
            {
                includeAnswer = false;
            }

            List<QuestionDTO> questions = new();
            string apiUrlRoot = string.Empty, queryString = string.Empty;
            switch (stage)
            {
                case null:
                case "":
                case "FULL":
                case "Full":
                case "full":
                    apiUrlRoot += $"{ApiBaseUrl}/api/Question/GetAll";
                    queryString = $"includeAnswer={includeAnswer}&episodeID={episodeID}";
                    break;
                default:
                    apiUrlRoot += $"{ApiBaseUrl}/api/Question/GetStageAsync/{stage}";
                    queryString = $"includeAnswer={includeAnswer}&episodeID={episodeID}&admitDate={admitDate}";
                    break;
            }

            questionApiEndpoint = $"{apiUrlRoot}?{queryString}";

            //questionApiEndpoint = stage switch
            //{
            //    null or "" or "Full" => $"{ApiBaseUrl}/api/Question/GetAll?includeAnswer={includeAnswer}&episodeID={episodeID}",
            //    _ => $"{ApiBaseUrl}/api/Question/GetStageAsync/{stage}?includeAnswer={includeAnswer}&episodeID={episodeID}&admitDate={admitDate}",
            //};

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

                //forward the postbackModel to web api Answer controller.
                //the {ApiBAseUrl} is read from appSetting.development.json if launch setting is DEVELOPMENT,
                //otherwise from appSetting.json
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

        #endregion public

        #region private
        /// <summary>
        /// this should be in a utility library
        /// </summary>
        /// <param name="networkID"></param>
        /// <returns></returns>
        private static string CleanUserName(string networkID)
        {
            string networkName = networkID;
            if (string.IsNullOrEmpty(networkName))
                return null;
            else
            {
                if (networkName.Contains('\\') || networkName.Contains("%2F") || networkName.Contains("//"))
                {
                    String[] separator = { "\\", "%2F", "//" };
                    var networkNameWithDomain = networkName.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    if (networkNameWithDomain.Length > 0)
                        networkName = networkNameWithDomain[1];
                    else
                        networkName = networkNameWithDomain[0];
                }
                return networkName;
            }
        }

        private async Task<List<MastUserDTO>> ValidateThisUserAccess(string networkID)
        {
            string userName = CleanUserName(networkID); //use network ID without domain

            List<MastUserDTO> thisUserAccessLevel = base.MemoryCache.Get<List<MastUserDTO>>($"{CacheKeys.CacheKeyThisUserAccessLevel}_{networkID}");

            if (thisUserAccessLevel != null && thisUserAccessLevel.Any())
            {
                return thisUserAccessLevel;
            }
            else
            {
                SqlParameter[] paramNetworkID = new SqlParameter[]
                {
                    new SqlParameter(){
                      ParameterName = "@UserName",
                      SqlDbType = System.Data.SqlDbType.VarChar,
                      Direction = System.Data.ParameterDirection.Input,
                      Value = userName
                    }
                };

                //use dbContext extension method
                var distinctFacilities = await _userPatientCacheHelper.GetUserAccessLevels(networkID);

                if (distinctFacilities == null || !distinctFacilities.Any())
                    return null;    //no permitted facilities

                //using var MasterReportsDb = new MasterreportsContext();
                //var procedure = new MasterreportsContextProcedures(MasterReportsDb);
                //var accessLevel = procedure.uspVSSCMain_SelectAccessInformationFromNSSDAsync(userName);

                base.MemoryCache.Set($"{CacheKeys.CacheKeyThisUserAccessLevel}_{networkID}", distinctFacilities, TimeSpan.FromHours(2));
                return distinctFacilities;
            }
        }

        /// <summary>
        /// duplicated method of IPRehabWebAPI2.Helpers.UserPatientCacheHelper class
        /// </summary>
        /// <param name="thisUserAccessLevel"></param>
        /// <param name="episodeID"></param>
        /// <param name="patientID"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        private async Task<PatientDTOTreatingSpecialty> GetThisPatient(List<MastUserDTO> thisUserAccessLevel, int episodeID, string patientID, string currentUserID)
        {
            var thisFacilityPatients = await _userPatientCacheHelper_TreatingSpecialty.GetThisFacilityPatients(thisUserAccessLevel);
            if (thisFacilityPatients == null || !thisFacilityPatients.Any())
            {
                return null;    //no patient in this facility 
            }

            var patientInFacility = thisFacilityPatients.FirstOrDefault(p => p.ScrSsnt == patientID || p.PatientIcn == patientID);
            PatientDTOTreatingSpecialty thisPatient;
            if (patientInFacility != null)
            {
                thisPatient = new()
                {
                    Sta6a = patientInFacility.Bsta6a,
                    Name = patientInFacility.PatientName,
                    PTFSSN = patientInFacility.ScrSsnt,
                    PatientICN = patientInFacility.PatientIcn,
                    DoB = patientInFacility.DoB,
                    Bedsecn = patientInFacility.Bedsecn
                };
            }
            else
            {
                string webAPIendpoint;

                if (episodeID > 0)
                {
                    webAPIendpoint = $"{ApiBaseUrl}/api/{base._TreatingSpecialtyApiControllerName}/Episode/{episodeID}";
                }
                else
                {
                    webAPIendpoint = $"{ApiBaseUrl}/api/{base._TreatingSpecialtyApiControllerName}/{patientID}?networkID={currentUserID}&withEpisode=false&pageSize={base.PageSize}";
                }

                thisPatient = await SerializationGeneric<PatientDTOTreatingSpecialty>.DeserializeAsync($"{webAPIendpoint}", base.BaseOptions);

                base.MemoryCache.Set(CacheKeys.CacheKeyThisPatient, thisPatient, TimeSpan.FromDays(1));

            }
            return thisPatient;
        }

        #endregion private
    }
}
