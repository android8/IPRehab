using IPRehab.Helpers;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
    public class BaseController : Controller
    {
        protected string GitVersion { get; set; }
        protected IWebHostEnvironment BaseEnvironment { get; }

        protected IConfiguration BaseConfiguration { get; }

        protected string ApiBaseUrl { get; }

        protected readonly string _TreatingSpecialtyApiControllerName = "TreatingSpecialtyPatientDirect";

        protected string EnvironmentName { get; }

        protected string Impersonated { get; }

        protected JsonSerializerOptions BaseOptions { get; }

        protected int PageSize { get; }
        protected string Office { get; }

        protected IMemoryCache MemoryCache { get; }
        protected string UserID { get; private set; }

        protected BaseController(IWebHostEnvironment environment, IMemoryCache memoryCache, IConfiguration configuration)
        {
            BaseEnvironment = environment;
            BaseConfiguration = configuration;
            ApiBaseUrl = BaseConfiguration.GetSection("AppSettings").GetValue<string>("WebAPIBaseUrl");
            EnvironmentName = environment.EnvironmentName;  //BaseConfiguration.GetSection("AppSettings").GetValue<string>("Version");
            Impersonated = BaseConfiguration.GetSection("AppSettings").GetValue<string>("Impersonate");
            MemoryCache = memoryCache;

            //get GitVersion
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("IPRehab.GitVersion.txt"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    GitVersion = $"{reader.ReadToEnd()} - {EnvironmentName}";
                }
            }

            if (!string.IsNullOrEmpty(Impersonated))
            {
                //get impersonated user from appSettings.json
                UserID = ParseNetworkID.CleanUserName(System.Web.HttpUtility.UrlDecode(Impersonated));
            }

            PageSize = BaseConfiguration.GetSection("AppSettings").GetValue<int>("DefaultPageSize");
            Office = BaseConfiguration.GetSection("AppSettings").GetValue<string>("Office");
            BaseOptions = new JsonSerializerOptions()
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
                ViewBag.WebRootPath = BaseEnvironment.WebRootPath;
                ViewBag.ContentRootPath = BaseEnvironment.ContentRootPath;
                ViewBag.Environment = BaseEnvironment.EnvironmentName;
                ViewBag.SourceOfCredential = sourceOfCredential;
                ViewBag.CurrentUserID = $"{this.UserID}";
                ViewBag.CurrentUserName = viewBagCurrentUserName;
                ViewBag.GitVersion = GitVersion;
                ViewBag.Office = this.Office;
                var routeData = this.RouteData;
                var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
                //don't await the external audit result
                UserAudit.AuditUserAsync(BaseConfiguration, HttpContext.User.Identity.Name, RouteData, remoteIpAddress);
                await next();
            }
        }

        protected IActionResult AccessDenied()
        {
            return View();
        }
    }
}
