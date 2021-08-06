using IPRehab.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  public class EpisodeController : BaseController
  {
    ILogger<EpisodeController> _logger;
    public EpisodeController(ILogger<EpisodeController> logger, IConfiguration configuration) : base(configuration)
    {
      _logger = logger;
    }
    // GET: EpisodeController
    public async Task<ActionResult> Index()
    {
      ViewBag.Title = "Episode";
      List<EpisodeOfCareDTO> episodes = new List<EpisodeOfCareDTO>();
      HttpResponseMessage Res;

      string url = string.Empty;
      try
      {
        //Sending request to find web api REST service resource Episode using HttpClient in the APIAgent
        url = $"{_apiBaseUrl}/api/Episode";
        Res = await APIAgent.GetDataAsync(new Uri(url));
      }
      catch (Exception ex)
      {
        return PartialView("_ErrorPartial", new ErrorViewModelHelper()
          .Create("Fail to call Web API", ex.Message, ex.InnerException?.Message));
      }

      string httpMsgContentReadMethod = "ReadAsStreamAsync";
      System.IO.Stream contentStream = null;
      if (Res.Content is object && Res.Content.Headers.ContentType.MediaType == "application/json")
      {
        try
        {
          switch (httpMsgContentReadMethod)
          {
            //use Newtonsoft json deserializer
            //case "ReadAsStringAsync":
            //  string patientsString = await Res.Content.ReadAsStringAsync();
            //  patients = JsonConvert.DeserializeObject<List<PatientDTO>>(patientsString);
            //  break;

            case "ReadAsAsync":
              episodes = await Res.Content.ReadAsAsync<List<EpisodeOfCareDTO>>();
              break;

            //use .Net 5 built-in deserializer
            case "ReadAsStreamAsync":
              contentStream = await Res.Content.ReadAsStreamAsync();
              episodes = await JsonSerializer.DeserializeAsync<List<EpisodeOfCareDTO>>(contentStream, _options);
              break;
          }

          if (episodes?.Count == 0)
            return View("_NoDataPartial");

          //returning the question list to view  
          return View(episodes);
        }
        catch (Exception ex)// Could be ArgumentNullException or UnsupportedMediaTypeException
        {
          return PartialView("_ErrorPartial", new ErrorViewModelHelper()
            .Create("Json deserialization error", ex.Message, ex.InnerException?.Message));
        }
      }
      else
      {
        var ex = new Exception();
        return PartialView("_ErrorPartial", new ErrorViewModelHelper()
        .Create("Web API content is not an object or mededia type is not applicaiton/json", string.Empty, string.Empty));
      }
    }

    // GET: EpisodeController/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }

    // GET: EpisodeController/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: EpisodeController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(IFormCollection collection)
    {
      try
      {
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }

    // GET: EpisodeController/Edit/5
    public ActionResult Edit(int id)
    {
      return View();
    }

    // POST: EpisodeController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
      try
      {
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }

    // GET: EpisodeController/Delete/5
    public ActionResult Delete(int id)
    {
      return View();
    }

    // POST: EpisodeController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
      try
      {
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }
  }
}
