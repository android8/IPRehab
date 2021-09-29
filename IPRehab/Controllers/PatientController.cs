using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
      ViewBag.Title = "Patient";

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
        url = $"{_apiBaseUrl}/api/FSODPatient?criteria={searchCriteria}&withEpisode=true&impersonatedUserName={_impersonatedUserName}" +
          $"&pageNumber={pageNumber}&pageSize={_pageSize}&&orderBy={orderBy}";
      }
      else
      {
        url = $"{_apiBaseUrl}/api/FSODPatient?criteria={searchCriteria}&withEpisode=true" +
          $"&pageNumber={pageNumber}&pageSize={_pageSize}&orderBy={orderBy}";
      }

      IEnumerable<PatientDTO> patients;
      string resContent = string.Empty;
      try
      {
        patients = await SerializationGeneric<IEnumerable<PatientDTO>>.SerializeAsync(url, _options);
        var res = SerializationGeneric<IEnumerable<PatientDTO>>.Res;
        resContent = res.Content.ToString();
      }
      catch (Exception ex)
      {
        var vm = new ErrorViewModelHelper();
        return PartialView("_ErrorPartial",
          vm.Create("Serialization error", $"{ex?.Message} {Environment.NewLine} HttpResponseMessage={resContent}", ex.InnerException?.Message)
        );
      }

      PatientListViewModel patientListVM = new();
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
        foreach (var pat in patients)
        {
          PatientViewModel thisPatVM = new();
          thisPatVM.Patient = pat;

          RehabActionViewModel episodeCommandBtn = new();
          foreach (var episode in pat.CareEpisodes)
          {
            episodeCommandBtn.EpisodeID = episode.EpisodeOfCareID;
            episodeCommandBtn.PatientID = episode.PatientIcnFK;
          }
          if (thisPatVM.ActionButtonVM == null)
          {
            episodeCommandBtn.EpisodeID = -1;
            episodeCommandBtn.PatientID = pat.PTFSSN;
          }

          episodeCommandBtn.PatientName = pat.Name;
          episodeCommandBtn.SearchCriteria = searchCriteria;
          episodeCommandBtn.PageNumber = pageNumber;
          episodeCommandBtn.OrderBy = orderBy;
          episodeCommandBtn.HostContainer = "Patient";

          thisPatVM.ActionButtonVM = episodeCommandBtn;

          patientListVM.Patients.Add(thisPatVM);
          patientListVM.TotalPatients = patients.Count();
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
