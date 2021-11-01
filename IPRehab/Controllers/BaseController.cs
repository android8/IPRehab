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
    protected IIdentity _windowsIdentity;
    protected readonly IConfiguration _configuration;
    protected readonly string _apiBaseUrl;
    protected readonly string _appVersion;
    protected readonly JsonSerializerOptions _options;
    protected List<MastUserDTO> userAccessLevels;
    protected readonly ILogger _logger;
    protected readonly string _impersonatedUserName;
    protected readonly int _pageSize;
    protected readonly string _office;

    protected BaseController(IConfiguration configuration, ILogger logger)
    {
      _configuration = configuration;
      _logger = logger;
      _apiBaseUrl = _configuration.GetSection("AppSettings").GetValue<string>("WebAPIBaseUrl");
      _appVersion = _configuration.GetSection("AppSettings").GetValue<string>("Version");
      _impersonatedUserName = System.Web.HttpUtility.UrlEncode(_configuration.GetSection("AppSettings").GetValue<string>("Impersonate"));
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
      string apiUrlBase = $"{_apiBaseUrl}/api/MasterReportsUser";

      //no impersonation so get identity from User.Claims
      //string trueUser = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value; 

      //drop the last zero if the admin ZERO account is used
      string trueUser = HttpContext.User.Identity.Name.Replace("0","");

      string encodedTrueUser = System.Web.HttpUtility.UrlEncode(trueUser);
      List<MastUserDTO> accessLevels = new();

      ViewBag.Office = _office;
      ViewBag.CurrentUser = trueUser;
      ViewBag.CurrentNetworkID = HttpContext.User.Identity.Name;
      CancellationToken cancellationToken = new();
      await HttpContext.Session.LoadAsync(cancellationToken);

      string userAccessLevelSessionKey = "UserAccessLevel";

      //get userAccessLevels from session
      string jsonStringFromSession = HttpContext.Session.GetString(userAccessLevelSessionKey);
      bool callWebAPI = true;
      //if (!string.IsNullOrEmpty(jsonStringFromSession))
      //{
      //  //the impersonation can be manually changed in appSettings.json during a session
      //  //retrieve serialized object from session and check if it is the same log in user 
      //  accessLevels = JsonSerializer.Deserialize<IEnumerable<MastUserDTO>>(jsonStringFromSession).ToList();

      //  string userInSession = accessLevels.First().NTUserName;
      //  if (!string.IsNullOrEmpty(_impersonatedUserName) && userInSession == _impersonatedUserName)
      //  {
      //    callWebAPI = false;
      //  }
      //  else
      //  {
      //    if (userInSession == ParseNetworkID.CleanUserName(System.Web.HttpUtility.UrlDecode(encodedtrueUser)))
      //      callWebAPI = false;
      //  }
      //}

      if (callWebAPI)
      {
        if (string.IsNullOrEmpty(_impersonatedUserName))
        {
          /* use current log in user network name */
          accessLevels = await SerializationGeneric<List<MastUserDTO>>.SerializeAsync($"{apiUrlBase}/{encodedTrueUser}", _options);
        }
        else
        {
          /* use impersonated network name */
          accessLevels = await SerializationGeneric<List<MastUserDTO>>.SerializeAsync($"{apiUrlBase}/{_impersonatedUserName}", _options);
        }


        if (accessLevels != null && accessLevels.Any())
        {
          //update session key UserAccessLevels value
          string serializedString = JsonSerializer.Serialize(accessLevels);
          HttpContext.Session.SetString(userAccessLevelSessionKey, serializedString);
        }
      }

      MastUserDTO thisUser = accessLevels.FirstOrDefault(u => !string.IsNullOrEmpty(u.NTUserName));
      if (thisUser != null)
      {
        ViewBag.CurrentUser = $"{thisUser.LName}, {thisUser.FName}";
        ViewBag.CurrentNetworkID = thisUser.NTUserName;
      }
      
      ViewBag.AppVersion = $"Version {_appVersion}";

      var routeData = this.RouteData;
      var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
      await UserAudit.AuditUserAsync(trueUser, RouteData, remoteIpAddress);
      await next();
    }
  }
}
