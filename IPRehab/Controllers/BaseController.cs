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
using System.IO;
using System.Reflection;

namespace IPRehab.Controllers
{
    public class BaseController : Controller
    {
        private string _gitVersion;
        protected string GitVersion { get => _gitVersion; set => _gitVersion = value; } 

        private readonly IWebHostEnvironment _environment;
        protected IWebHostEnvironment BaseEnvironment => _environment;

        private readonly IConfiguration _configuration;
        protected IConfiguration BaseConfiguration => _configuration;

        private readonly string _apiBaseUrl;
        protected string ApiBaseUrl=> _apiBaseUrl;

        private readonly string _appVersion;
        protected string AppVersion => _appVersion;

        private readonly string _impersonated;
        protected string Impersonated => _impersonated;

        private readonly JsonSerializerOptions _options;
        protected JsonSerializerOptions BaseOptions => _options;

        private readonly int _pageSize;
        protected int PageSize => _pageSize; 

        private readonly string _office;
        protected string Office =>_office;

        //private readonly List<MastUserDTO> _userAccessLevels;
        //protected List<MastUserDTO> UserAccessLevels { get { return _userAccessLevels; }}

        private readonly IMemoryCache _memoryCache;
        protected IMemoryCache MemoryCache => _memoryCache; 

        private string _userID;
        protected string UserID =>_userID;

        protected BaseController(IWebHostEnvironment environment, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
            _apiBaseUrl = _configuration.GetSection("AppSettings").GetValue<string>("WebAPIBaseUrl");
            _appVersion = _configuration.GetSection("AppSettings").GetValue<string>("Version");
            _impersonated = _configuration.GetSection("AppSettings").GetValue<string>("Impersonate");
            _memoryCache = memoryCache;
            
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("IPRehab.GitVersion.txt"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    GitVersion = reader.ReadToEnd();
                    ViewBag.GitVersion = GitVersion;
                }
            }

            if (!string.IsNullOrEmpty(_impersonated))
            {
                //get impersonated user from appSettings.json
                _userID = ParseNetworkID.CleanUserName(System.Web.HttpUtility.UrlDecode(_impersonated));
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
                this._userID = ParseNetworkID.CleanUserName(System.Web.HttpUtility.UrlDecode(HttpContext.User.Identity.Name));
            }

            //get userAccessLevels from session
            //string jsonStringFromSession = HttpContext.Session.GetString(userAccessLevelSessionKey);
            IEnumerable<MastUserDTO> thisUserAccessLevel = MemoryCache.Get<IEnumerable<MastUserDTO>>($"{CacheKeys.CacheKeyThisUserAccessLevel}_{this.UserID}");

            if (sourceOfCredential == "Master Report")
            {
                //get userAccessLevel from web API, if not in the HttpContext.Session
                //if (string.IsNullOrEmpty(jsonStringFromSession))
                if (thisUserAccessLevel == null || !thisUserAccessLevel.Any())
                {
                    string apiUrl = $"{ApiBaseUrl}/api/MasterReportsUser/{this.UserID}";
                    thisUserAccessLevel = await SerializationGeneric<List<MastUserDTO>>.DeserializeAsync($"{apiUrl}", this.BaseOptions);

                    if (thisUserAccessLevel == null || !thisUserAccessLevel.Any())
                    {
                        sourceOfCredential = "(WebAPI: access level is null)";
                        viewBagCurrentUserName = "Unknown";
                    }
                    else
                    {
                        MemoryCache.Set($"{CacheKeys.CacheKeyThisUserAccessLevel}_{this.UserID}", thisUserAccessLevel, TimeSpan.FromHours(2));

                        MastUserDTO thisUser = thisUserAccessLevel.First(u => !string.IsNullOrEmpty(u.NTUserName));
                        sourceOfCredential = "(WebAPI)";
                        viewBagCurrentUserName = $"{thisUser.LName}, {thisUser.FName}";
                        //update session key UserAccessLevels value
                    }
                }
                //user is in the cache 
                else
                {
                    MastUserDTO thisUser = thisUserAccessLevel.FirstOrDefault();
                    sourceOfCredential = "(Session)";
                    viewBagCurrentUserName = $"{thisUser.LName}, {thisUser.FName}";
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
