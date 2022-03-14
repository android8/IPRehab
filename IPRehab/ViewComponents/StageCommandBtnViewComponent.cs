using IPRehab.Helpers;
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
      Dictionary<string, CommandBtnConfig> commandBtnConfig = EpisodeCommandButtonSettings.CommandBtnConfigDictionary;
      List<StageCommandViewComponentTemplateModel> cmdButtonVMList = new();

      if (EpisodeBtnConfig.EpisodeOfCareID > 0)
      {
        EpisodeBtnConfig.PatientIcnFK = EpisodeBtnConfig.PatientIcnFK.Substring(EpisodeBtnConfig.PatientIcnFK.Length - 4, 4);
      }
      else
      {
        EpisodeBtnConfig.PatientIcnFK = EpisodeBtnConfig.PatientIcnFK;
      }

      /* iterate 8 stage types */
      foreach (KeyValuePair<string, CommandBtnConfig> entry in commandBtnConfig)
      {
        StageCommandViewComponentTemplateModel cmdBtnTemplateVM = new();
        cmdBtnTemplateVM.Title = entry.Value.ButtonTitle;
        cmdBtnTemplateVM.Stage = entry.Key;
        cmdBtnTemplateVM.ActionBtnCssClass = entry.Value.ButtonCss;

        if (entry.Key == "Patient")
        {
          if (EpisodeBtnConfig.ActionButtonVM.HostingPage == "Question")
            cmdBtnTemplateVM.ShowThisButton = true;

          cmdBtnTemplateVM.TextNode = "Patient List";

          RehabActionViewModel clonedPatientActionVM = EpisodeBtnConfig.ActionButtonVM.Clone() as RehabActionViewModel;
          clonedPatientActionVM.ControllerName = "Patient";
          clonedPatientActionVM.ActionName = "Index";
          clonedPatientActionVM.PatientID = string.Empty;
          clonedPatientActionVM.EpisodeID = -1;
          //use cloned
          cmdBtnTemplateVM.ActionVM = clonedPatientActionVM;
        }
        else
        {
          if (entry.Key == "New" || EpisodeBtnConfig.EpisodeOfCareID > 0 || EpisodeBtnConfig.ActionButtonVM.HostingPage == "Question")
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
            default:
              cmdBtnTemplateVM.TextNode = entry.Key;
              break;
          }
        }

        if (cmdBtnTemplateVM.ShowThisButton)
        {
          if (EpisodeBtnConfig.ActionButtonVM.EpisodeID != -1 && cmdBtnTemplateVM.Stage != "New")
            cmdBtnTemplateVM.Title = cmdBtnTemplateVM.Title.Replace("Create new", "Edit existing");
          
          cmdButtonVMList.Add(cmdBtnTemplateVM);
        }
      }

      string viewName = "StageCommandBtn";
      return Task.FromResult<IViewComponentResult>(View(viewName, cmdButtonVMList));
    }
  }
}
