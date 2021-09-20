using IPRehab.Helpers;
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
    public PatientController(ILogger<PatientController> logger, IConfiguration configuration) 
      : base(configuration, logger)
    {
    }

    // GET: PatientController
    public async Task<ActionResult> Index(string criteria, int pageNumber, string orderBy)
    {
      criteria = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(criteria));
      ViewBag.Title = "Patient";
      List<PatientDTO> patients = new();

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

      string url;

      //Sending request to find web api REST service resource FSODPatient using HttpClient in the APIAgent
      if (!string.IsNullOrEmpty(_impersonatedUserName))
      {
        url = $"{_apiBaseUrl}/api/FSODPatient?criteria={criteria}&withEpisode=true&impersonatedUserName={_impersonatedUserName}&pageNumber={pageNumber}&pageSize={_pageSize}&&orderBy={orderBy}";
      }
      else
      {
        url = $"{_apiBaseUrl}/api/FSODPatient?criteria={criteria}&withEpisode=true&pageNumber={pageNumber}&pageSize={_pageSize}&orderBy={orderBy}";
      }

      PatientSearchResultMeta patientsMeta;
      try
      {
        patientsMeta = await SerializationGeneric<PatientSearchResultMeta>.SerializeAsync(url, _options);
      }
      catch(Exception ex)
      {
        return PartialView("_ErrorPartial", new ErrorViewModelHelper()
          .Create("Serialization error", ex.Message, ex.InnerException.Message));
      }

      if (patientsMeta.Patients.Count == 0)
        return View("_NoDataPartial");

      PatientListViewModel patientListVM = new();
      patientListVM.SearchCriteria = criteria;
      patientListVM.TotalPatients = patientsMeta.Patients.Count;
      foreach (var pat in patientsMeta.Patients)
      {
        PatientViewModel thisPatVM = new();
        thisPatVM.Patient = pat;

        foreach (var episode in pat.CareEpisodes)
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
        patientListVM.Patients.Add(thisPatVM);
        patientListVM.TotalPatients = patientsMeta.TotalCount;
      }

      //returning the question list to view  
      return View(patientListVM);
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
