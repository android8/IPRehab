using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IPRehab.Controllers
{
  public class TestColorController : BaseController
  {
    public TestColorController(ILogger<TestColorController> logger, IConfiguration configuration) : base(configuration, logger)
    {
    }

    public async Task<ActionResult> IndexAsync()
    {
      ViewBag.Title = "Color Test";
      List<TestColor> colors = new List<TestColor>();
      try
      {
        //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
        HttpResponseMessage Res = await APIAgent.GetDataAsync(new Uri($"{_apiBaseUrl}/api/TestColor"));

        string httpMsgContentReadMethod = "ReadAsStringAsync";
        if (Res.Content is object && Res.Content.Headers.ContentType.MediaType == "application/json")
        {
          try
          {
            switch (httpMsgContentReadMethod)
            {
              //use Newtonsoft json deserializer
              case "ReadAsStringAsync":
                string patientsString = await Res.Content.ReadAsStringAsync();
                colors = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TestColor>>(patientsString);
                break;

              case "ReadAsAsync":
                colors = await Res.Content.ReadAsAsync<List<TestColor>>();
                break;

              //use .Net 5 built-in deserializer
              case "ReadAsStreamAsync":
                var contentStream = await Res.Content.ReadAsStreamAsync();
                colors = await JsonSerializer.DeserializeAsync<List<TestColor>>(contentStream, _options);
                break;
            }

            //returning the question list to view  
            return View(colors);
          }
          catch (Exception ex) // Could be ArgumentNullException or UnsupportedMediaTypeException
          {
            return PartialView("_ErrorPartial", new ErrorViewModelHelper()
              .Create("JSON serialization error", ex.Message, ex.InnerException?.Message));
          }
        }
        else
        {
          return View("No content");
        }
      }
      catch (Exception ex)
      {
        return PartialView("_ErrorPartial", new ErrorViewModelHelper()
          .Create("API call exception", ex.Message, ex.InnerException?.Message));
      }
    }
  }
}
