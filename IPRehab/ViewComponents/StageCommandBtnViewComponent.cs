using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehab.ViewComponents
{
  public class StageCommandBtnViewComponent : ViewComponent
  {
    public StageCommandBtnViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(PatientEpisodeAndCommandVM EpisodeBtnConfig)
    {
      Dictionary<string,string> stageType = IPRehab.Helpers.EpisodeCommandButtonSettings.stageType;
      Dictionary<string, string> actionBtnClass = IPRehab.Helpers.EpisodeCommandButtonSettings.actionBtnColor;
      List<StageCommandViewComponentTemplateModel> cmdButtonVMList = new();
      string rawSSN;
      
      if (EpisodeBtnConfig.EpisodeOfCareID > 0)
      {
        rawSSN = EpisodeBtnConfig.PatientIcnFK;
        EpisodeBtnConfig.PatientIcnFK = rawSSN.Substring(rawSSN.Length - 4, 4);
      }

      /* iterate 8 stages */
      foreach (var entry in stageType)
      {
        StageCommandViewComponentTemplateModel cmdBtnTemplateVM = new();
        cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;
        cmdBtnTemplateVM.Title = entry.Value;
        cmdBtnTemplateVM.Stage = entry.Key;
        cmdBtnTemplateVM.TextNode = entry.Key;
        cmdBtnTemplateVM.ActionBtnCssClass = actionBtnClass[entry.Key];
        cmdBtnTemplateVM.ShowThisButton = true;
        cmdBtnTemplateVM.DisableThisButton = false;

        switch (cmdBtnTemplateVM.Stage)
        {
          case "Patient":
            cmdBtnTemplateVM.TextNode = "Patient List";
            if (EpisodeBtnConfig.ActionButtonVM.HostingPage == "Question")
            {
              cmdBtnTemplateVM.ActionVM.ControllerName = "Patient";
              cmdBtnTemplateVM.ActionVM.ActionName = "Index";
              //stage = string.Empty;
              cmdBtnTemplateVM.ActionVM.PatientID = string.Empty;
              cmdBtnTemplateVM.ActionVM.EpisodeID = -1;
            }
            else
              cmdBtnTemplateVM.ShowThisButton = false;
            break;

          case "Full":
            cmdBtnTemplateVM.TextNode = "IRF-PAI";
            break;

          case "Followup":
            cmdBtnTemplateVM.TextNode = "Follow Up";
            break;
        }

        if (EpisodeBtnConfig.ActionButtonVM.EpisodeID == -1 && (cmdBtnTemplateVM.Stage != "Patient" && cmdBtnTemplateVM.Stage != "Base"))
          cmdBtnTemplateVM.DisableThisButton = true;

        if (EpisodeBtnConfig.ActionButtonVM.EpisodeID != -1 && cmdBtnTemplateVM.Stage != "New")
          cmdBtnTemplateVM.Title = cmdBtnTemplateVM.Title.Replace("Create new", "Edit existing");

        cmdButtonVMList.Add(cmdBtnTemplateVM);
      }
      string viewName = "StageCommandBtn";
      return Task.FromResult<IViewComponentResult>(View(viewName, cmdButtonVMList));
    }
  }
}
