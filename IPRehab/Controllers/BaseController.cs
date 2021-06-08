using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  public class BaseController : Controller
  {
    protected readonly IConfiguration _configuration;
    protected readonly string _apiBaseUrl;
    protected readonly string _appVersion;
    protected readonly JsonSerializerOptions _options;

    protected BaseController(IConfiguration configuration)
    {
      _configuration = configuration;
      //_apiBaseUrl = _configuration.GetValue<string>("WebAPIBaseUrl");
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
      ViewBag.AppVersion = $"Version {_appVersion}";
      await next();
    }


    protected void DeserialExceptionHandler(Exception ex)
    {
      Console.WriteLine("HTTP Response was invalid or could not be deserialised.");
      Console.WriteLine($"{ex.Message}");
      if (ex.InnerException != null)
      {
        Console.WriteLine($"{ex.InnerException.Message}");
      }
    }

    protected void WebAPIExceptionHander(Exception ex)
    {
      Console.WriteLine("WebAPI call failure.");
      Console.WriteLine($"{ex.Message}");
      if (ex.InnerException != null)
      {
        Console.WriteLine($"{ex.InnerException.Message}");
      }
    }
  }
}
