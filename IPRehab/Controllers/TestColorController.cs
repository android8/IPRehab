using IPRehab.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  public class TestColorController : BaseController
  {
    public TestColorController(IWebHostEnvironment environment, ILogger<TestColorController> logger, IConfiguration configuration) 
      : base(environment, configuration, logger)
    {
    }

    public async Task<ActionResult> IndexAsync()
    {
      ViewBag.Title = "Color Test";
      List<TestColor> colors = new();
      //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
      HttpResponseMessage Res = await APIAgent.GetDataAsync(new Uri($"{ApiBaseUrl}/api/TestColor"));

      string httpMsgContentReadMethod = "ReadAsStringAsync";
      if (Res.Content?.Headers.ContentType.MediaType == "application/json")
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
            colors = await JsonSerializer.DeserializeAsync<List<TestColor>>(contentStream, base.BaseOptions);
            break;
        }

        //returning the question list to view  
        return View(colors);
      }
      else
      {
        return View("No content");
      }
    }
  }
}
