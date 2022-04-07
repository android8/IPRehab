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
        if (EpisodeBtnConfig.ActionButtonVM.EpisodeID != -1 && cmdBtnTemplateVM.Stage != "New")
          cmdBtnTemplateVM.Title = entry.Value.ButtonTitle.Replace("Create new", "Edit existing");
        else
          cmdBtnTemplateVM.Title = entry.Value.ButtonTitle;

        cmdBtnTemplateVM.Stage = entry.Key;
        cmdBtnTemplateVM.ActionBtnCssClass = entry.Value.ButtonCss;

        if (entry.Key == "Patient")
        {
          if (EpisodeBtnConfig.ActionButtonVM.HostingPage == "Patient")
          {
            cmdBtnTemplateVM.ShowThisButton = false;
          }
          else
          {
            //only visible when in Question stage form
            cmdBtnTemplateVM.ShowThisButton = true;

            cmdBtnTemplateVM.TextNode = "Patient List";

            RehabActionViewModel clonedPatientActionVM = EpisodeBtnConfig.ActionButtonVM.Clone() as RehabActionViewModel;
            clonedPatientActionVM.ControllerName = "Patient";
            clonedPatientActionVM.ActionName = "Index";
            clonedPatientActionVM.PatientID = string.Empty;
            clonedPatientActionVM.EpisodeID = -1;
            //use cloned
            cmdBtnTemplateVM.ActionVM = clonedPatientActionVM;
            cmdButtonVMList.Add(cmdBtnTemplateVM);
          }
        }
        else
        {
          if (EpisodeBtnConfig.EpisodeOfCareID == -1)
          {
            cmdBtnTemplateVM.ShowThisButton = false;
          }
          else { 
            cmdBtnTemplateVM.ShowThisButton = true;

            // use invoked parameter
            cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;
            if (entry.Value.ButtonTitle == "Base")
              cmdBtnTemplateVM.TextNode = "Episode of Care";
            else
              cmdBtnTemplateVM.TextNode = entry.Value.ButtonTitle;
            cmdButtonVMList.Add(cmdBtnTemplateVM);
          }
        }
      }

      string viewName = "StageCommandBtn";
      return Task.FromResult<IViewComponentResult>(View(viewName, cmdButtonVMList));
    }
  }
}
