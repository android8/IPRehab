﻿using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  //ToDo: [Authorize]
  public class PatientController : BaseController
  {
    ILogger<PatientController> _logger;
    public PatientController(ILogger<PatientController> logger, IConfiguration configuration) : base(configuration)
    {
      _logger = logger;
    }

    // GET: PatientController
    public async Task<ActionResult> Index(string criteria)
    {
      criteria = System.Web.HttpUtility.UrlEncode(criteria);
      ViewBag.Title = "Patient";
      List<PatientDTO> patients = new List<PatientDTO>();

      string sessionCriteria;
      string sessionKey = "SearchCriteria";
      CancellationToken cancellationToken = new CancellationToken();
      await HttpContext.Session.LoadAsync(cancellationToken);
      sessionCriteria = HttpContext.Session.GetString(sessionKey);
      if (criteria != sessionCriteria)
      {
        if (string.IsNullOrEmpty(criteria))
          HttpContext.Session.Remove(sessionKey);
        else
          HttpContext.Session.SetString(sessionKey, criteria);
      }

      ViewBag.PreviousCriteria = criteria;

      HttpResponseMessage Res;
      string url;
      try
      {
        //Sending request to find web api REST service resource FSODPatient using HttpClient in the APIAgent
        if (!string.IsNullOrEmpty(_currentUser))
        {
          url = $"{_apiBaseUrl}/api/FSODPatient?criteria={criteria}&withEpisode=true&currentUser={_currentUser}";
        }
        else
          url = $"{_apiBaseUrl}/api/FSODPatient?criteria={criteria}&withEpisode=true";

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
              patients = await Res.Content.ReadAsAsync<List<PatientDTO>>();
              break;

            //use .Net 5 built-in deserializer
            case "ReadAsStreamAsync":
              contentStream = await Res.Content.ReadAsStreamAsync();
              patients = await JsonSerializer.DeserializeAsync<List<PatientDTO>>(contentStream, _options);
              break;
          }

          if (patients?.Count == 0)
            return View("_NoDataPartial");

          List<PatientViewModel> vm = new();
          foreach(var pat in patients)
          {
            PatientViewModel thisPatVM = new();
            thisPatVM.Patient = pat;

            foreach(var episode in pat.CareEpisodes)
            {
              RehabActionViewModel episodeCommandBtn = new();
              episodeCommandBtn.EpisodeID = episode.EpisodeOfCareID;
              episodeCommandBtn.PatientID = episode.PatientIcnFK;
              episodeCommandBtn.PatientName = pat.Name;
              thisPatVM.ActionButtonVM = episodeCommandBtn;
            }
            if (thisPatVM.ActionButtonVM == null)
            {
              RehabActionViewModel episodeCommandBtn = new();
              episodeCommandBtn.EpisodeID = -1;
              episodeCommandBtn.PatientID = pat.PTFSSN;
              episodeCommandBtn.PatientName = pat.Name;
              thisPatVM.ActionButtonVM = episodeCommandBtn;
            }
            vm.Add(thisPatVM);
          }

          //returning the question list to view  
          return View(vm);
        }
        catch (Exception ex)// Could be ArgumentNullException or UnsupportedMediaTypeException
        {
          return PartialView("_ErrorPartial", new ErrorViewModelHelper()
            .Create("Json deserialization error", ex.Message, ex.InnerException?.Message));
        }
      }
      else
      {
        return PartialView("_ErrorPartial", new ErrorViewModelHelper()
        .Create("Web API content is not an object or mededia type is not applicaiton/json", string.Empty, string.Empty));
      }
    }

    // GET: PatientController/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }

    // GET: PatientController/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: PatientController/Create
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

    // GET: PatientController/Edit/5
    public async Task<ActionResult> EditAsync(RehabStageEnum stage, string ssn)
    {
      ssn = System.Web.HttpUtility.UrlEncode(ssn);
      ViewBag.Title = "Patient";
      PatientDTO patient = new PatientDTO();

      HttpResponseMessage Res = new HttpResponseMessage();
      try
      {
        if (stage != RehabStageEnum.Undefined && !string.IsNullOrEmpty(ssn))
        {
          //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
          Res = await APIAgent.GetDataAsync(new Uri($"{_apiBaseUrl}/api/FSODPatient/{ssn}"));
        }
        string httpMsgContentReadMethod = "ReadAsStringAsync";
        if (Res.Content is object && Res.Content.Headers.ContentType.MediaType == "application/json")
        {
          try
          {
            switch (httpMsgContentReadMethod)
            {
              //use Newtonsoft json deserializer
              //case "ReadAsStringAsync":
              //  string patientsString = await Res.Content.ReadAsStringAsync();
              //  patient = JsonConvert.DeserializeObject<PatientDTO>(patientsString);
              //  break;

              case "ReadAsAsync":
                patient = await Res.Content.ReadAsAsync<PatientDTO>();
                break;

              //use .Net 5 built-in deserializer
              case "ReadAsStreamAsync":
                var contentStream = await Res.Content.ReadAsStreamAsync();
                patient = await JsonSerializer.DeserializeAsync<PatientDTO>(contentStream, _options);
                break;
            }

            //returning the question list to view  
            return View(patient);
          }
          catch (Exception ex) // Could be ArgumentNullException or UnsupportedMediaTypeException
          {
            return PartialView("_ErrorPartial", new ErrorViewModelHelper()
              .Create("Json deserialization error", ex.Message, ex.InnerException?.Message));
          }
        }
        else
        {
          return PartialView("_ErrorPartial", new ErrorViewModelHelper()
            .Create("JSOn content is not an object or not in application/json medial type.", string.Empty, string.Empty));
        }
      }
      catch (Exception ex)
      {
        return PartialView("_ErrorPartial", new ErrorViewModelHelper()
          .Create("Web API call failure", ex.Message, ex.InnerException?.Message));
      }
    }

    // POST: PatientController/Edit/5
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

    // GET: PatientController/Delete/5
    public ActionResult Delete(int id)
    {
      return View();
    }

    // POST: PatientController/Delete/5
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
