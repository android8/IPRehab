using IPRehab.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  public class BaseController : Controller
  {
    protected readonly IConfiguration _configuration;
    protected readonly string _apiBaseUrl;
    protected readonly string _appVersion;
    protected readonly string _impersonated;
    protected readonly JsonSerializerOptions _options;
    protected readonly ILogger _logger;
    protected readonly int _pageSize;
    protected readonly string _office;

    protected IIdentity _windowsIdentity;
    protected List<MastUserDTO> userAccessLevels;
    protected string _userID;

    protected BaseController(IConfiguration configuration, ILogger logger)
    {
      _configuration = configuration;
      _logger = logger;
      _apiBaseUrl = _configuration.GetSection("AppSettings").GetValue<string>("WebAPIBaseUrl");
      _appVersion = _configuration.GetSection("AppSettings").GetValue<string>("Version");
      _impersonated = _configuration.GetSection("AppSettings").GetValue<string>("Impersonate");

      if (!string.IsNullOrEmpty(_impersonated))
      {
        //get impersonated user from appSettings.json
        _userID = ParseNetworkID.CleanUserName(System.Web.HttpUtility.UrlDecode(_impersonated));
      }

      _pageSize = _configuration.GetSection("AppSettings").GetValue<int>("DefaultPageSize");
      _office = _configuration.GetSection("AppSettings").GetValue<string>("Office");
      _options = new JsonSerializerOptions()
      {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
        IgnoreNullValues = true,
        AllowTrailingCommas = true
      };
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      string userAccessLevelSessionKey = "UserAccessLevel";
      List<MastUserDTO> accessLevels = new();
      string viewBagCurrentUserName = "Sun, Jonathan";
      string sourceOfCredential = "Master Report";
      CancellationToken cancellationToken = new();
      await HttpContext.Session.LoadAsync(cancellationToken);

      //get user ID from Windows Identity
      if (string.IsNullOrEmpty(_userID))
      {
        _userID = ParseNetworkID.CleanUserName(System.Web.HttpUtility.UrlDecode(HttpContext.User.Identity.Name));
      }

      //get userAccessLevels from session
      string jsonStringFromSession = HttpContext.Session.GetString(userAccessLevelSessionKey);

      if (sourceOfCredential == "Master Report")
      {

        //get userAccessLevel from web API, if not in the HttpContext.Session
        if (string.IsNullOrEmpty(jsonStringFromSession))
        {
          string apiUrlBase = $"{_apiBaseUrl}/api/MasterReportsUser";
          accessLevels = await SerializationGeneric<List<MastUserDTO>>.SerializeAsync($"{apiUrlBase}/{_userID}", _options);
          if (accessLevels == null && !accessLevels.Any())
          {
            sourceOfCredential = "(WebAPI: access level is null)";
            viewBagCurrentUserName = "Unknown";
          }
          else
          {
            MastUserDTO thisUser = accessLevels.FirstOrDefault(u => !string.IsNullOrEmpty(u.NTUserName));
            if (thisUser == null)
            {
              sourceOfCredential = "(WebAPI: user not contained in access level)";
              viewBagCurrentUserName = "Unknown";
            }
            else
            {
              sourceOfCredential = "(WebAPI)";
              viewBagCurrentUserName = $"{thisUser.LName}, {thisUser.FName}";
              //update session key UserAccessLevels value
              string serializedString = JsonSerializer.Serialize(accessLevels);
              HttpContext.Session.SetString(userAccessLevelSessionKey, serializedString);
            }
          }
        }
        //user in HttpContext.Session then don't call web API
        else
        {
          accessLevels = JsonSerializer.Deserialize<IEnumerable<MastUserDTO>>(jsonStringFromSession).ToList();
          if (accessLevels == null || accessLevels.Count == 0)
          {
            sourceOfCredential = "(Session: deserialization issue)";
            viewBagCurrentUserName = "Unknown";
          }
          else
          {
            MastUserDTO thisUser = accessLevels.FirstOrDefault(u => !string.IsNullOrEmpty(u.NTUserName));
            if (thisUser == null)
            {
              sourceOfCredential = "(Session: has no user information)";
              viewBagCurrentUserName = "Unknown";
            }
            else
            {
              sourceOfCredential = "(Session)";
              viewBagCurrentUserName = $"{thisUser.LName}, {thisUser.FName}";
            }
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
        ViewBag.SourceOfCredential = sourceOfCredential;
        ViewBag.CurrentUserID = $"{_userID}";
        ViewBag.CurrentUserName = viewBagCurrentUserName;
        ViewBag.AppVersion = $"Version {_appVersion}";
        ViewBag.Office = _office;

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
