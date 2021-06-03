﻿using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IPRehab.Controllers
{
  //ToDo: [Authorize]
  public class PatientController : BaseController
  {
    public PatientController(ILogger<QuestionController> logger, IConfiguration configuration) : base(logger, configuration)
    {
    }

    // GET: PatientController
    public async Task<ActionResult> Index(string criteria)
    {
      ViewBag.Title = "Patient";
      List<PatientDTO> patients = new List<PatientDTO>();
      try
      {
        //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
        HttpResponseMessage Res = await APIAgent.GetDataAsync(new Uri($"{_apiBaseUrl}/api/FSODPatient?criteria={criteria}"));

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
                patients = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PatientDTO>>(patientsString);
                break;

              case "ReadAsAsync":
                patients = await Res.Content.ReadAsAsync<List<PatientDTO>>();
                break;

              //use .Net 5 built-in deserializer
              case "ReadAsStreamAsync":
                var contentStream = await Res.Content.ReadAsStreamAsync();
                patients = await JsonSerializer.DeserializeAsync<List<PatientDTO>>(contentStream, _options);
                break;
            }

            //returning the question list to view  
            return View(patients);
          }
          catch (Exception ex) // Could be ArgumentNullException or UnsupportedMediaTypeException
          {
            DeserialExceptionHandler(ex);
            return null;
          }
        }
        else
        {
          return null;
        }
      }
      catch (Exception ex)
      {
        WebAPIExceptionHander(ex);
        return null;
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
      ViewBag.Title = "Patient";
      PatientDTO patient = new PatientDTO();

      HttpResponseMessage Res = new HttpResponseMessage();
      try
      {
        if (stage != RehabStageEnum.Undefined && !string.IsNullOrEmpty(ssn))
        {
          //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
          Res = await APIAgent.GetDataAsync(new Uri($"{_apiBaseUrl}/api/FSODPatient/{stage.ToString()}?{ssn}"));
        }
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
                patient = Newtonsoft.Json.JsonConvert.DeserializeObject<PatientDTO>(patientsString);
                break;

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
            DeserialExceptionHandler(ex);
            return null;
          }
        }
        else
        {
          return null;
        }
      }
      catch (Exception ex)
      {
        WebAPIExceptionHander(ex);
        return null;
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
