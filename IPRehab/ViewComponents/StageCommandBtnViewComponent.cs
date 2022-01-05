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
      Dictionary<string, string> stageType = IPRehab.Helpers.EpisodeCommandButtonSettings.stageType;
      Dictionary<string, string> actionBtnClass = IPRehab.Helpers.EpisodeCommandButtonSettings.actionBtnColor;
      List<StageCommandViewComponentTemplateModel> cmdButtonVMList = new();

      PatientEpisodeAndCommandVM episodeVM = new()
      {
        ActionButtonVM = EpisodeBtnConfig.ActionButtonVM,
        EpisodeOfCareID = EpisodeBtnConfig.EpisodeOfCareID,
        OnsetDate = EpisodeBtnConfig.OnsetDate,
        AdmissionDate = EpisodeBtnConfig.AdmissionDate
      };

      if (EpisodeBtnConfig.EpisodeOfCareID > 0)
      {
        episodeVM.PatientIcnFK = EpisodeBtnConfig.PatientIcnFK.Substring(EpisodeBtnConfig.PatientIcnFK.Length - 4, 4);
      }
      else
      {
        episodeVM.PatientIcnFK = EpisodeBtnConfig.PatientIcnFK;
      }

      /* iterate 8 stage types */
      foreach (KeyValuePair<string, string> entry in stageType)
      {
        StageCommandViewComponentTemplateModel cmdBtnTemplateVM = new();
        cmdBtnTemplateVM.ShowThisButton = false;

        if (entry.Key == "Patient")
        {
          if (EpisodeBtnConfig.ActionButtonVM.HostingPage == "Question")
          {
            cmdBtnTemplateVM.ShowThisButton = true;
          }

          cmdBtnTemplateVM.TextNode = "Patient List";
          cmdBtnTemplateVM.Title = entry.Value;
          cmdBtnTemplateVM.Stage = entry.Key;
          cmdBtnTemplateVM.ActionBtnCssClass = actionBtnClass[entry.Key];

          RehabActionViewModel clonedPatientActionVM = episodeVM.ActionButtonVM.Clone() as RehabActionViewModel;
          clonedPatientActionVM.ControllerName = "Patient";
          clonedPatientActionVM.ActionName = "Index";
          clonedPatientActionVM.PatientID = string.Empty;
          clonedPatientActionVM.EpisodeID = -1;
          //use cloned
          cmdBtnTemplateVM.ActionVM = clonedPatientActionVM;
        }
        else
        {
          cmdBtnTemplateVM.TextNode = entry.Key;
          cmdBtnTemplateVM.Title = entry.Value;
          cmdBtnTemplateVM.Stage = entry.Key;
          cmdBtnTemplateVM.ActionBtnCssClass = actionBtnClass[entry.Key];
          cmdBtnTemplateVM.ShowThisButton = true;
          // use invoked parameter
          cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;

          switch (entry.Key)
          {
            case "Full":
              cmdBtnTemplateVM.TextNode = "IRF-PAI";
              break;
            case "Followup":
              cmdBtnTemplateVM.TextNode = "Follow Up";
              break;
            case "Base":
              cmdBtnTemplateVM.TextNode = "Episode of Care";
              break;
          }
        }

        if (cmdBtnTemplateVM.ShowThisButton)
        {
          if (episodeVM.ActionButtonVM.EpisodeID == -1 && cmdBtnTemplateVM.Stage != "Base" && cmdBtnTemplateVM.Stage != "Patient")
            cmdBtnTemplateVM.DisableThisButton = true;

          if (episodeVM.ActionButtonVM.EpisodeID != -1 && cmdBtnTemplateVM.Stage != "New")
            cmdBtnTemplateVM.Title = cmdBtnTemplateVM.Title.Replace("Create new", "Edit existing");
          
          cmdButtonVMList.Add(cmdBtnTemplateVM);
        }
      }

      string viewName = "StageCommandBtn";
      return Task.FromResult<IViewComponentResult>(View(viewName, cmdButtonVMList));
    }
  }
}
