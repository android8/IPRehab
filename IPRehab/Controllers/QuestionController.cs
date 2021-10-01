﻿using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  //ToDo: [Authorize]
  public class QuestionController : BaseController
  {
    public QuestionController(IConfiguration configuration, ILogger<QuestionController> logger) 
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// https://www.stevejgordon.co.uk/sending-and-receiving-json-using-httpclient-with-system-net-http-json
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns> 
    // GET: QuestionController/Edit/5
    public async Task<ActionResult> Edit1(string stage, string patientID, string patientName, int episodeID)
    {
      stage = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(stage));
      patientID = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(patientID));
      string encodedPatientName = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(patientName));

      string action = "Edit";
      ViewBag.StageTitle = string.IsNullOrEmpty(stage) ? "Full" : (stage == "Followup" ? "Follow Up" : $"{stage}");
      ViewBag.Action = $"{action} Mode";

      List<QuestionDTO> questions = new List<QuestionDTO>();
      bool includeAnswer = (action == "Edit");

      RehabActionViewModel actionButtonVM = new() { 
        HostContainer = "Question",
        EpisodeID = episodeID,
        PatientID = patientID,
        PatientName = encodedPatientName
      };

      ViewBag.ActionBtnVM = actionButtonVM;

      string apiEndpoint;
      string badgeBackgroundColor;
      switch (stage)
      {
        case null:
        case "":
        case "Full":
          apiEndpoint = $"{_apiBaseUrl}/api/Question/GetAll?includeAnswer={includeAnswer}&episodeID={episodeID}";
          badgeBackgroundColor = EpisodeCommandButtonSettings.actionBtnColor[stage];
          break;
        default:
          apiEndpoint = $"{_apiBaseUrl}/api/Question/GetStageAsync/{stage}?includeAnswer={includeAnswer}&episodeID={episodeID}";
          badgeBackgroundColor = EpisodeCommandButtonSettings.actionBtnColor[stage];
          break;
      }
      ViewBag.ModeColor = badgeBackgroundColor;

      questions = await SerializationGeneric<List<QuestionDTO>>.SerializeAsync($"{apiEndpoint}", _options);

      List<QuestionWithSelectItems> vm = new List<QuestionWithSelectItems>();
      foreach (var dto in questions)
      {
        QuestionWithSelectItems qws = HydrateVM.Hydrate(dto);
        vm.Add(qws);
      }

      //model for section navigator side bar
      var distinctSections = HydrateVM.GetDistinctSections(vm);
      ViewBag.QuestionSections = distinctSections;

      //returning the question list to view  
      return View(vm);
    }

    public async Task<IActionResult> Edit(string stage, string patientID, string patientName, int episodeID, string searchCriteria, int pageNumber, string orderBy)
    {
      stage = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(stage));
      patientID = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(patientID));
      string encodedPatientName = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(patientName));
      
      string stageTitle = string.IsNullOrEmpty(stage) ? "Full" : (stage == "Followup" ? "Follow Up" : $"{stage}");
      string action = nameof(Edit);
      bool includeAnswer = (action == "Edit");
      
      RehabActionViewModel actionButtonVM = new()
      {
        HostContainer = "Question",
        EpisodeID = episodeID,
        PatientID = patientID,
        PatientName = encodedPatientName,
        SearchCriteria = searchCriteria,
        PageNumber = pageNumber
      };

      List<QuestionDTO> questions = new ();

      string apiEndpoint;
      string actionBtnColor;
      switch (stage)
      {
        case null:
        case "":
        case "Full":
          apiEndpoint = $"{_apiBaseUrl}/api/Question/GetAll?includeAnswer={includeAnswer}&episodeID={episodeID}";
          actionBtnColor = EpisodeCommandButtonSettings.actionBtnColor[stage];
          break;
        default:
          apiEndpoint = $"{_apiBaseUrl}/api/Question/GetStageAsync/{stage}?includeAnswer={includeAnswer}&episodeID={episodeID}";
          actionBtnColor = EpisodeCommandButtonSettings.actionBtnColor[stage];
          break;
      }

      questions = await SerializationGeneric<List<QuestionDTO>>.SerializeAsync($"{apiEndpoint}", _options);

      QuestionHierarchy qh = HydrateVM.HydrateHierarchically(questions, stageTitle);
      qh.ReadOnly = false;
      qh.EpisodeID = episodeID;
      qh.StageTitle = stageTitle;
      qh.PatientName = patientName;
      qh.ActionButtons = actionButtonVM;
      qh.CurrentAction = $"{action} Mode";
      qh.ModeColorCssClass = actionBtnColor;
      //model for section navigator side bar

      //returning the question list to view  
      return View(qh);
    }

    // POST: QuestionController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromBody] PostbackModel postbackModel)
    {
      if (ModelState.IsValid)
      {
        List<UserAnswer> newAnswers = postbackModel.NewAnswers;
        List<UserAnswer> oldAnswers = postbackModel.OldAnswers;
        List<UserAnswer> updatedAnswers = postbackModel.UpdatedAnswers;
        UserAnswer thisAnswer = new();
        if (newAnswers != null)
          thisAnswer = newAnswers.Find(x => x.StageName != string.Empty); 
        else if (oldAnswers != null)
          thisAnswer = oldAnswers.Find(x => x.StageName != string.Empty);
        else if (updatedAnswers != null)
          thisAnswer = updatedAnswers.Find(x => x.StageName != string.Empty);

        //forward the postbackModel to web api Answer controller
        Uri uri = new Uri($"{_apiBaseUrl}/api/Answer/Post");
        
        var Res = await APIAgent.PostDataAsync(uri, postbackModel);
        if (Res.IsSuccessStatusCode)
        {
          Response.StatusCode = (int)Res.StatusCode;
          //return Json("Data saved successfully");
          return new JsonResult("Data saved successfully");
        }
        else
        {
          Response.StatusCode = (int)Res.StatusCode;
          return new JsonResult($"Data not saved. {Res}");
        }
      }
      else
      {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        return new JsonResult("Data not saved.  The post back model is not valid.");
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
      return RedirectToAction(nameof(Edit));
    }
  }
}
