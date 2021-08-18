using IPRehab.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

    protected BaseController(IConfiguration configuration)
    {
      _configuration = configuration;
      _apiBaseUrl = _configuration.GetSection("AppSettings").GetValue<string>("WebAPIBaseUrl");
      _appVersion = _configuration.GetSection("AppSettings").GetValue<string>("Version");
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
      _windowsIdentity = HttpContext.User.Identity;
      ViewBag.WindowsIdentityName = _windowsIdentity.Name;
      ViewBag.AppVersion = $"Version {_appVersion}";
      //userAccessLevels = await UserPermissionFromSessionAsync();
      //if (userAccessLevels == null || userAccessLevels.Count == 0)
      //{
      //  userAccessLevels = await UserPermissionFromWebAPIAsync(_windowsIdentity.Name);
      //}
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
      catch(Exception ex)
      {
        //WebAPIExceptionHander(ex);
        RedirectToAction("Error", "Home", new { ex.Message });
      }
      return userAccessLevels;
    }

    private async Task<List<MastUserDTO>> UserPermissionFromWebAPIAsync(string WindowsIdentityName)
    {
      List<MastUserDTO> accessLevelsFromWebAPI = null;

      try
      {
        HttpResponseMessage Res = await APIAgent.GetDataAsync(new Uri($"{_apiBaseUrl}/api/MasterReportsUser/{WindowsIdentityName}"));

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
      catch(Exception ex)
      {
        RedirectToAction("Error", "Home", new { ex.Message });
      }
      return accessLevelsFromWebAPI;
    }
  }
}
