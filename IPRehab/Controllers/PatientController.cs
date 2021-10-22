﻿using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  //ToDo: [Authorize]
  public class PatientController : BaseController
  {
    public PatientController(ILogger<PatientController> logger, IConfiguration configuration)
      : base(configuration, logger)
    {
    }

    // GET: PatientController
    public async Task<ActionResult> Index(string searchCriteria, int pageNumber, string orderBy)
    {
      searchCriteria = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(searchCriteria));

      string sessionCriteria;
      string sessionKey = "SearchCriteria";
      CancellationToken cancellationToken = new CancellationToken();
      await HttpContext.Session.LoadAsync(cancellationToken);
      sessionCriteria = HttpContext.Session.GetString(sessionKey);
      if (searchCriteria != sessionCriteria)
      {
        if (string.IsNullOrEmpty(searchCriteria))
          HttpContext.Session.Remove(sessionKey);
        else
          HttpContext.Session.SetString(sessionKey, searchCriteria);
      }

      string url;

      //Sending request to find web api REST service resource FSODPatient using HttpClient in the APIAgent
      if (!string.IsNullOrEmpty(_impersonatedUserName))
      {
        url = $"{_apiBaseUrl}/api/FSODPatient?networkID={_impersonatedUserName}&criteria={searchCriteria}&withEpisode=true&&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={_pageSize}";
      }
      else
      {
        url = $"{_apiBaseUrl}/api/FSODPatient?criteria={searchCriteria}&withEpisode=true&orderBy={orderBy}&pageNumber={pageNumber}&pageSize={_pageSize}";
      }

      IEnumerable<PatientDTO> patients;
      HttpResponseMessage res = null;
      try
      {
        patients = await SerializationGeneric<IEnumerable<PatientDTO>>.SerializeAsync(url, _options);
        res = SerializationGeneric<IEnumerable<PatientDTO>>.Res;
      }
      catch (Exception ex)
      {
        var vm = new ErrorViewModelHelper();
        return PartialView("_ErrorPartial",
          vm.Create("Serialization error", message: $"{ex?.Message} {Environment.NewLine} HttpResponseMessage={res}", ex.InnerException?.Message)
        );
      }

      PatientListViewModel patientListVM = new();
      patientListVM.TotalPatients = patients.Count();
      patientListVM.PageTitle = "Inpatients Rehab";
      patientListVM.SearchCriteria = searchCriteria;
      patientListVM.PageNumber = pageNumber;
      patientListVM.OrderBy = orderBy;

      if (patients == null || patients.Count() == 0)
      {
        patientListVM.TotalPatients = 0;
        return View("_NoDataPartial", patientListVM);
      }
      else
      {
        foreach (PatientDTO pat in patients)
        {
          PatientViewModel thisPatVM = new();
          thisPatVM.Patient = pat;

          //don't use FSODSSN, it may be null
          string rawSSN = pat.PTFSSN;

          //ToDo: encrypt the SSN, only when patient has no existing episode
          thisPatVM.Patient.PTFSSN = rawSSN.Substring(rawSSN.Length - 4, 4);

          if (!pat.CareEpisodes.Any())
          {
            RehabActionViewModel episodeCommandBtn = new()
            {
              //since no episode ID we have to use patient ID to find patient
              PatientID = rawSSN,
              HostingPage = "Patient",
              SearchCriteria = searchCriteria,
              PageNumber = pageNumber,
              OrderBy = orderBy,
              EpisodeID = -1,
              EnableThisPatient = false
            };

            PatientEpisodeAndCommandVM thisEpisodeAndCommands = new();
            //Don't assign episode properties for patient without episode
            thisEpisodeAndCommands.ActionButtonVM = episodeCommandBtn;
            thisPatVM.EpisodeBtnConfig.Add(thisEpisodeAndCommands);
          }
          else
          {
            foreach (EpisodeOfCareDTO episode in pat.CareEpisodes)
            {
              episode.PatientIcnFK = rawSSN.Substring(rawSSN.Length - 4, 4);

              RehabActionViewModel episodeCommandBtn = new()
              {
                //to avoid exposing PHI/PII, leave the PatientID blank and use the EpisodeID to search for patient ID
                PatientID = string.Empty,
                HostingPage = "Patient",
                SearchCriteria = searchCriteria,
                PageNumber = pageNumber,
                OrderBy = orderBy,
                EpisodeID = episode.EpisodeOfCareID,
                EnableThisPatient = true
              };

              //PatientEpisodeAndCommandVM derivedClass = episode as PatientEpisodeAndCommandVM;

              PatientEpisodeAndCommandVM thisEpisodeAndCommands = new() {
                EpisodeOfCareID= episode.EpisodeOfCareID,
                OnsetDate = episode.OnsetDate,
                AdmissionDate = episode.AdmissionDate,
                PatientIcnFK = episode.PatientIcnFK
              };
              thisEpisodeAndCommands.ActionButtonVM = episodeCommandBtn;
              thisPatVM.EpisodeBtnConfig.Add(thisEpisodeAndCommands);
            }
          }
          patientListVM.Patients.Add(thisPatVM);
        }

        //returning the question list to view  
        return View(patientListVM);
      }
    }

    // POST: PatientController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
      return View();
    }
  }
}
