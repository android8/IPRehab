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
      if (EpisodeBtnConfig.EpisodeOfCareID > 0)
      {
        EpisodeBtnConfig.PatientIcnFK = EpisodeBtnConfig.PatientIcnFK.Substring(EpisodeBtnConfig.PatientIcnFK.Length - 4, 4);
      }
      else
      {
        EpisodeBtnConfig.PatientIcnFK = EpisodeBtnConfig.PatientIcnFK;
      }

      Dictionary<string, CommandBtnConfig> commandBtnConfig = EpisodeCommandButtonSettings.CommandBtnConfigDictionary;

      List<StageCommandViewComponentTemplateModel> cmdButtonVMList = new();

      /* iterate 8 stage types */
      foreach (KeyValuePair<string, CommandBtnConfig> entry in commandBtnConfig)
      {
        StageCommandViewComponentTemplateModel cmdBtnTemplateVM = new();

        string HostingPage = EpisodeBtnConfig.ActionButtonVM.HostingPage;
        switch (entry.Key) {
          case "Patient":
            {
              switch (HostingPage)
              {
                case "Patient": {
                    cmdBtnTemplateVM.ShowThisButton = false;
                    break;
                  }
                case "Question":
                  {
                    cmdBtnTemplateVM.ActionBtnCssClass = entry.Value.ButtonCss;
                    cmdBtnTemplateVM.ShowThisButton = true;
                    cmdBtnTemplateVM.TextNode = "Patient List";
                    RehabActionViewModel clonedPatientActionVM = EpisodeBtnConfig.ActionButtonVM.Clone() as RehabActionViewModel;
                    clonedPatientActionVM.ControllerName = "Patient";
                    clonedPatientActionVM.ActionName = "Index";
                    clonedPatientActionVM.PatientID = string.Empty;
                    clonedPatientActionVM.EpisodeID = -1;
                    cmdBtnTemplateVM.ActionVM = clonedPatientActionVM;  //use cloned
                    break;
                  }
              }
              break;
            }
          case "New":
            {
              cmdBtnTemplateVM.ActionBtnCssClass = entry.Value.ButtonCss;
              cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;  // use invoked parameter as is
              cmdBtnTemplateVM.Stage = entry.Key;
              cmdBtnTemplateVM.TextNode = (entry.Value.ButtonTitle == "Base") ? "Episode of Care" :
                entry.Value.ButtonTitle;
              cmdBtnTemplateVM.Title = (cmdBtnTemplateVM.Stage == "New") ?
                entry.Value.ButtonTitle.Replace("Create new", "Edit existing") :
                entry.Value.ButtonTitle;

              if (HostingPage == "Question")
              {
                /* don't use EpisodeBtnConfig.EpisodeOfCareID here */
                if (EpisodeBtnConfig.ActionButtonVM.EpisodeID > 0)
                  cmdBtnTemplateVM.ShowThisButton = true;
                else
                  cmdBtnTemplateVM.ShowThisButton = false;
              }
              else
                cmdBtnTemplateVM.ShowThisButton = true;
              break;
            }
          default:
            {
              /* don't use EpisodeBtnConfig.EpisodeOfCareID here */
              if (EpisodeBtnConfig.ActionButtonVM.EpisodeID > 0)
              {
                cmdBtnTemplateVM.ActionBtnCssClass = entry.Value.ButtonCss;
                cmdBtnTemplateVM.ShowThisButton = true;
                cmdBtnTemplateVM.Stage = entry.Key;
                cmdBtnTemplateVM.Title = (cmdBtnTemplateVM.Stage == "New") ?
                entry.Value.ButtonTitle.Replace("Create new", "Edit existing") : entry.Value.ButtonTitle;
                cmdBtnTemplateVM.TextNode = entry.Value.ButtonTitle == "Base" ?
                  "Episode of Care" : entry.Value.ButtonTitle;
                cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;  // use invoked parameter
              }
              break;
            }
        }
        cmdButtonVMList.Add(cmdBtnTemplateVM);
      }

      string viewName = "StageCommandBtn";
      return Task.FromResult<IViewComponentResult>(View(viewName, cmdButtonVMList));
    }
  }
}
