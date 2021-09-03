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
    public EpisodeController(IConfiguration configuration, ILogger<EpisodeController> logger) : base(configuration, logger)
    {
    }
    // GET: EpisodeController
    public async Task<ActionResult> Index()
    {
      ViewBag.Title = "Episode";
      List<EpisodeOfCareDTO> episodes = new();
      HttpResponseMessage Res;

      //Sending request to find web api REST service resource Episode using HttpClient in the APIAgent
      string url = $"{_apiBaseUrl}/api/Episode";
      Res = await APIAgent.GetDataAsync(new Uri(url));


      string httpMsgContentReadMethod = "ReadAsStreamAsync";
      if (Res.Content is object && Res.Content.Headers.ContentType.MediaType == "application/json")
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
            System.IO.Stream contentStream = await Res.Content.ReadAsStreamAsync();
            episodes = await JsonSerializer.DeserializeAsync<List<EpisodeOfCareDTO>>(contentStream, _options);
            break;
        }

        if (episodes?.Count == 0)
          return View("_NoDataPartial");

        //returning the question list to view  
        return View(episodes);

      }
      else
      {
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
      return RedirectToAction(nameof(Index));
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
      return RedirectToAction(nameof(Index));
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
      return RedirectToAction(nameof(Index));
    }
  }
}
