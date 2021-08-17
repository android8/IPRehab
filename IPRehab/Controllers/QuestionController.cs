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
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  //ToDo: [Authorize]
  public class QuestionController : BaseController
  {
    ILogger<QuestionController> _logger;
    public QuestionController(ILogger<QuestionController> logger, IConfiguration configuration) : base(configuration)
    {
      _logger = logger;
    }

    // GET: QuestionController
    public ActionResult Index()
    {
      return View();
    }

    // GET: QuestionController/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }

    // GET: QuestionController/Create
    public ActionResult Create(string stage)
    {
      return RedirectToAction(nameof(Edit), new { stage = stage, redirectFrom = "Create" });
    }

    // POST: QuestionController/Create
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
    
    /// <summary>
    /// https://www.stevejgordon.co.uk/sending-and-receiving-json-using-httpclient-with-system-net-http-json
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns> 
    // GET: QuestionController/Edit/5
    public async Task<ActionResult> Edit(string stage, string patientID, string patientName, int episodeID, string redirectFrom)
    {
      string badgeBackgroundColor = string.Empty;
      string action = string.IsNullOrEmpty(redirectFrom) ? "Edit" : $"{redirectFrom} ";
      string title = string.IsNullOrEmpty(stage) ? "IRF-PAI Form" : (stage == "Followup" ? "Follow Up" : $"{stage}");
      ViewBag.Title = $"{title}";
      ViewBag.Action = $"{action} Mode";

      List<QuestionDTO> questions = new List<QuestionDTO>();
      bool includeAnswer = (action == "Edit");

      RehabActionViewModel actionButtonVM = new RehabActionViewModel();
      actionButtonVM.EpisodeID = episodeID;
      actionButtonVM.PatientID = patientID;
      actionButtonVM.PatientName = patientName;
      ViewBag.ActionBtnVM = actionButtonVM;

      try
      {
        //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
        HttpResponseMessage Res;
        string apiEndpoint = $"{_apiBaseUrl}/api/Question/GetStageAsync/{stage}?includeAnswer={includeAnswer}&episodeID={episodeID}";
        switch (stage)
        {
          case null:
          case "":
            apiEndpoint = $"{_apiBaseUrl}/api/Question/GetAll?includeAnswer={includeAnswer}&episodeID={episodeID}";
            badgeBackgroundColor = (action == "Edit") ? "badge-primary" : "createActionAll";
            break;
          case "Base":
            badgeBackgroundColor = (action == "Edit") ? "badge-dark" : "createActionBase";
            break;
          case "Initial":
            badgeBackgroundColor = (action == "Edit") ? "badge-info" : "createActionInitial";
            break;
          case "Interim":
            badgeBackgroundColor = (action == "Edit") ? "badge-secondary" : "createActionInterim";
            break;
          case "Discharge":
            badgeBackgroundColor = (action == "Edit") ? "badge-success" : "createActionDischarge";
            break;
          case "Followup":
            badgeBackgroundColor = (action == "Edit") ? "badge-warning" : "createActionFollowup";
            break;
        }
        Res = await APIAgent.GetDataAsync(new Uri($"{apiEndpoint}"));

        ViewBag.ModeColor = badgeBackgroundColor;

        string httpMsgContentReadMethod = "ReadAsStringAsync";
        if (Res.Content is object && Res.Content.Headers.ContentType.MediaType == "application/json")
        {
          List<QuestionWithSelectItems> vm = new List<QuestionWithSelectItems>();
          try
          {
            switch (httpMsgContentReadMethod)
            {
              case "ReadAsStringAsync":
                string questionString = await Res.Content.ReadAsStringAsync();
                questions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuestionDTO>>(questionString);
                break;

              case "ReadAsAsync":
                questions = await Res.Content.ReadAsAsync<List<QuestionDTO>>();
                break;
              case "ReadAsStreamAsync":
                var contentStream = await Res.Content.ReadAsStreamAsync();
                questions = await JsonSerializer.DeserializeAsync<List<QuestionDTO>>(contentStream, _options);
                break;
            }

            foreach (var dto in questions)
            {
              /* convert IEnumerable<QuestionDTO> to IEnumerable<QuestionWithSelectItems>
               * where the ChoiceList property is a list of SelectListItem */
              QuestionWithSelectItems qws = HydrateVM.Hydrate(dto);
              vm.Add(qws);
            }

            //model for section navigator side bar
            var distinctSections = HydrateVM.GetDistinctSections(vm);
            ViewBag.QuestionSections = distinctSections;


            //returning the question list to view  
            return View(vm);
          }
          catch (Exception ex) // Could be ArgumentNullException or UnsupportedMediaTypeException
          {
              return PartialView("_ErrorPartial", new ErrorViewModelHelper()
                .Create("JSON serialization error.", ex.Message, ex.InnerException?.Message));
          }
        }
        else
        {
          return PartialView("_ErrorPartial", new ErrorViewModelHelper()
            .Create("JSON content is not a objec or not an applicaiton/json media type.", string.Empty, string.Empty));
        }

      }
      catch (Exception ex)
      {
        return PartialView("_ErrorPartial", new ErrorViewModelHelper()
          .Create("WebAPI call failure.", ex.Message, ex.InnerException?.Message));
      }
    }

    // POST: QuestionController/Edit/5
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

    // GET: QuestionController/Delete/5
    public ActionResult Delete(int id)
    {
      return View();
    }

    // POST: QuestionController/Delete/5
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
