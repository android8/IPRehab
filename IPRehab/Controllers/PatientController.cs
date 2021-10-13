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
        bool firstPatient = true;
        foreach (var pat in patients)
        {
          PatientViewModel thisPatVM = new();
          thisPatVM.Patient = pat;

          //don't use FSODSSN, it may be null
          string rawSSN = pat.PTFSSN;

          //ToDo: encrypt the SSN, only when patient has no existing episode
          thisPatVM.Patient.PTFSSN = rawSSN.Substring(rawSSN.Length - 4, 4);
         
          RehabActionViewModel episodeCommandBtn = new();
          if (!pat.CareEpisodes.Any())
          {
            episodeCommandBtn.EpisodeID = -1;
            episodeCommandBtn.PatientID = rawSSN; 
          }
          else
          {
            foreach (var episode in pat.CareEpisodes)
            {
              //don't use FSODSSN, it may be null
              episode.PatientIcnFK = rawSSN.Substring(rawSSN.Length - 4, 4);
              episodeCommandBtn.EpisodeID = episode.EpisodeOfCareID;
            }
          }

          episodeCommandBtn.HostContainer = "Patient";
          episodeCommandBtn.SearchCriteria = searchCriteria;
          episodeCommandBtn.PageNumber = pageNumber;
          episodeCommandBtn.OrderBy = orderBy;
          thisPatVM.ActionButtonVM = episodeCommandBtn;

          patientListVM.Patients.Add(thisPatVM);
          patientListVM.TotalPatients = patients.Count();

          if (firstPatient)
          {
            episodeCommandBtn.enableThisPatient = true;
            firstPatient = false;
          }
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
