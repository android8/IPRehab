using IPRehabRepository.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IPRehab.Controllers
{
  public class BaseController : Controller
  {
    protected readonly ILogger<QuestionController> _logger;
    protected readonly IConfiguration _configuration;
    protected readonly string _apiBaseUrl;
    protected readonly JsonSerializerOptions _options;

    protected BaseController(ILogger<QuestionController> logger, IConfiguration configuration)
    {
      _logger = logger;
      _configuration = configuration;
      _apiBaseUrl = _configuration.GetValue<string>("WebAPIBaseUrl");

      _options = new JsonSerializerOptions()
      {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        IgnoreNullValues = true
      };
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
