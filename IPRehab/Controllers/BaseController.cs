using IPRehab.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
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
    protected readonly string sessionKey = "UserAccessLevels";
    protected List<MastUserDTO> userAccessLevels;
    readonly ILogger<EpisodeController> _logger;
    protected string _currentUser;

    protected BaseController(IConfiguration configuration)
    {
      _configuration = configuration;
      _apiBaseUrl = _configuration.GetSection("AppSettings").GetValue<string>("WebAPIBaseUrl");
      _appVersion = _configuration.GetSection("AppSettings").GetValue<string>("Version");
      _currentUser = _configuration.GetSection("AppSettings").GetValue<string>("Impersonate");
      _options = new JsonSerializerOptions()
      {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        IgnoreNullValues = true
      };
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      if (string.IsNullOrEmpty(_currentUser))
      {
        //no impersonation so get identity from User.Claims
        _currentUser = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value; //HttpContext.User.Identity.Name;
      }
      _currentUser = System.Web.HttpUtility.UrlEncode(_currentUser);

      ViewBag.CurrentUser = "Unknown";

      var accessLevels = await SerializationGeneric<MastUserDTO>.SerializeAsync($"{_apiBaseUrl}/api/MasterReportsUser/{_currentUser}", _options);
      //List<MastUserDTO> accessLevels = await UserPermissionFromWebAPIAsync(_currentUser);

      if (accessLevels != null && accessLevels.Any())
      {
        MastUserDTO thisUser = accessLevels.FirstOrDefault(u => !string.IsNullOrEmpty(u.NTUserName));
        if (thisUser != null)
        {
          ViewBag.CurrentUser = $"{thisUser.LName}, {thisUser.FName}";
        }
      }

      ViewBag.AppVersion = $"Version {_appVersion}";
      await next();
    }

    protected async Task<List<MastUserDTO>> UserPermissionFromSessionAsync()
    {
      try
      {
        CancellationToken cancellationToken = new();
        await HttpContext.Session.LoadAsync(cancellationToken);

        //get userAccessLevels from session
        string jsonStringFromSession = HttpContext.Session.GetString(sessionKey);
        if (!string.IsNullOrEmpty(jsonStringFromSession))
        {
          userAccessLevels = JsonSerializer.Deserialize<List<MastUserDTO>>(jsonStringFromSession, _options);
        }
      }
      catch (Exception ex)
      {
        //WebAPIExceptionHander(ex);
        RedirectToAction("Error", "Home", new { ex.Message });
      }
      return userAccessLevels;
    }

    private async Task<List<MastUserDTO>> UserPermissionFromWebAPIAsync(string thisName)
    {
      List<MastUserDTO> accessLevelsFromWebAPI = null;

      try
      {
        HttpResponseMessage Res = await APIAgent.GetDataAsync(new Uri($"{_apiBaseUrl}/api/MasterReportsUser/{thisName}"));

        string httpMsgContentReadMethod = "ReadAsStreamAsync";
        if (Res.Content is object && Res.Content.Headers.ContentType.MediaType == "application/json")
        {
          //update session key UserAccessLevels value
          HttpContext.Session.SetString(sessionKey, Res.Content.ReadAsStringAsync().Result);

          switch (httpMsgContentReadMethod)
          {
            case "ReadAsAsync":
              accessLevelsFromWebAPI = await Res.Content.ReadAsAsync<List<MastUserDTO>>();
              break;

            //use .Net 5 built-in deserializer
            case "ReadAsStreamAsync":
              var contentStream = await Res.Content.ReadAsStreamAsync();
              accessLevelsFromWebAPI = await JsonSerializer.DeserializeAsync<List<MastUserDTO>>(contentStream, _options);
              break;
          }
        }
      }
      catch (Exception ex)
      {
        RedirectToAction("Error", "Home", new { ex.Message });
      }
      return accessLevelsFromWebAPI;
    }
  }
}
