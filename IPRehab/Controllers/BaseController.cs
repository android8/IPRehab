using IPRehab.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;
using IPRehabWebAPI2.Helpers;
using Microsoft.AspNetCore.Identity;

namespace IPRehab.Controllers
{
    public class BaseController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        protected IWebHostEnvironment BaseEnvironment
        {
            get { return _environment; }
        }

        private readonly IConfiguration _configuration;
        protected IConfiguration BaseConfiguration
        {
            get { return _configuration; }
        }

        private readonly string _apiBaseUrl;
        protected string ApiBaseUrl
        {
            get { return _apiBaseUrl; }
        }

        private readonly string _appVersion;
        protected string AppVersion
        {
            get { return _appVersion; }
        }

        private readonly string _impersonated;
        protected string Impersonated
        {
            get { return _impersonated; }
        }

        private readonly JsonSerializerOptions _options;
        protected JsonSerializerOptions BaseOptions
        {
            get { return _options; }
        }

        private readonly int _pageSize;
        protected int PageSize
        {
            get { return _pageSize; }
        }

        private readonly string _office;
        protected string Office
        {
            get { return _office; }
        }

        private readonly IMemoryCache _memoryCache;
        protected IMemoryCache MemoryCache
        {
            get { return _memoryCache; }
        }

        private List<MastUserDTO> _thisUserAccessLevels;
        protected IEnumerable<MastUserDTO> ThisUserAccessLevels
        {
            get { return _thisUserAccessLevels; }
            set { _thisUserAccessLevels = (List<MastUserDTO>)value; }
        }

        private string _userID;
        protected string UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        private string _cacheKeyThisUserAccessLevel = CacheKeysBase.CacheKeyThisUserAccessLevel;
        protected string CacheKeyThisUserAccessLevel
        {
            get { return _cacheKeyThisUserAccessLevel; }
            set { _cacheKeyThisUserAccessLevel = value; }
        }

        private string _cacheKeyThisFacilityPatients = CacheKeysBase.CacheKeyThisFacilityPatients;
        protected string CacheKeyThisFacilityPatients
        {
            get { return _cacheKeyThisFacilityPatients; }
            set { _cacheKeyThisFacilityPatients = value; }
        }

        private string _cacheKeyThisPatient = CacheKeysBase.CacheKeyThisPatient;
        protected string CacheKeyThisPatient
        {
            get { return _cacheKeyThisPatient; }
            set { _cacheKeyThisPatient = value; }
        }

        protected BaseController(IWebHostEnvironment environment, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
            _apiBaseUrl = _configuration.GetSection("AppSettings").GetValue<string>("WebAPIBaseUrl");
            _appVersion = _configuration.GetSection("AppSettings").GetValue<string>("Version");
            _impersonated = _configuration.GetSection("AppSettings").GetValue<string>("Impersonate");
            _memoryCache = memoryCache;

            if (!string.IsNullOrEmpty(_impersonated))
            {
                //get impersonated user from appSettings.json
                this.UserID = ParseNetworkID.CleanUserName(System.Web.HttpUtility.UrlDecode(_impersonated));
            }

            _pageSize = _configuration.GetSection("AppSettings").GetValue<int>("DefaultPageSize");
            _office = _configuration.GetSection("AppSettings").GetValue<string>("Office");
            _options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = false,
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            List<MastUserDTO> accessLevels = new();
            string viewBagCurrentUserName = "Sun, Jonathan";
            string sourceOfCredential = "Master Report";
            CancellationToken cancellationToken = new();
            await HttpContext.Session.LoadAsync(cancellationToken);

            //get user ID from Windows Identity
            if (string.IsNullOrEmpty(this.UserID))
            {
                this.UserID = ParseNetworkID.CleanUserName(System.Web.HttpUtility.UrlDecode(HttpContext.User.Identity.Name));
            }
            this.CacheKeyThisUserAccessLevel += $"_{this.UserID}";

            ThisUserAccessLevels = MemoryCache.Get<IEnumerable<MastUserDTO>>($"{this.CacheKeyThisUserAccessLevel}");

            if (sourceOfCredential == "Master Report")
            {
                //user is in the cache 
                if (ThisUserAccessLevels != null || ThisUserAccessLevels.Any())
                {
                    sourceOfCredential = "(Cached)";
                    viewBagCurrentUserName = $"{ThisUserAccessLevels.First().LName}, {ThisUserAccessLevels.First().FName}";
                }
                else
                {
                    //get userAccessLevel from web API
                    string apiUrl = $"{ApiBaseUrl}/api/MasterReportsUser/{this.UserID}";
                    ThisUserAccessLevels = await SerializationGeneric<List<MastUserDTO>>.DeserializeAsync($"{apiUrl}", this.BaseOptions);

                    if (ThisUserAccessLevels == null || !ThisUserAccessLevels.Any())
                    {
                        sourceOfCredential = "(WebAPI: no access level)";
                        viewBagCurrentUserName = "Unknown";
                    }
                    else
                    {
                        sourceOfCredential = "(WebAPI)";
                        viewBagCurrentUserName = $"{ThisUserAccessLevels.First().LName}, {ThisUserAccessLevels.First().FName}";
                    }
                }
            }

            if (viewBagCurrentUserName == "Unknown")
            {
                //string thisContent = "Access Denied";
                context.Result = new ViewResult() { ViewName = "AccessDenied" };
                //context.Result = Content($"<div class='accessDenied'>{thisContent}</div>");
                //context.Result = new BadRequestObjectResult("Access Denied!");
            }
            else
            {
                ViewBag.Environment = BaseEnvironment.EnvironmentName;
                ViewBag.SourceOfCredential = sourceOfCredential;
                ViewBag.CurrentUserID = $"{this.UserID}";
                ViewBag.CurrentUserName = viewBagCurrentUserName;
                ViewBag.AppVersion = $"Version {AppVersion}";
                ViewBag.Office = this.Office;
                var routeData = this.RouteData;
                var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
                //don't await the external audit result
                UserAudit.AuditUserAsync(_configuration, HttpContext.User.Identity.Name, RouteData, remoteIpAddress);
                await next();
            }
        }

        protected IActionResult AccessDenied()
        {
            return View();
        }
    }
}
