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

            string admissionDate = EpisodeBtnConfig.AdmissionDate.ToString("MM/dd/yyyy");

            Dictionary<string, CommandBtnConfig> commandBtnConfig = EpisodeCommandButtonSettings.CommandBtnConfigDictionary;

            List<StageCommandViewComponentTemplateModel> cmdButtonVMList = new();

            /* iterate configuration dicitionary of the 5 command buttons per episode */
            foreach (KeyValuePair<string, CommandBtnConfig> button in commandBtnConfig)
            {
                StageCommandViewComponentTemplateModel cmdBtnTemplateVM = new();

                string HostingPage = EpisodeBtnConfig.ActionButtonVM.HostingPage;
                switch (button.Key)
                {
                    case "Patient":
                        {
                            switch (HostingPage)
                            {
                                case "Patient":
                                    {
                                        cmdBtnTemplateVM.ShowThisButton = false;
                                        break;
                                    }
                                case "Question":
                                    {
                                        cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                                        cmdBtnTemplateVM.ShowThisButton = true;
                                        cmdBtnTemplateVM.TextNode = "Patient List";
                                        RehabActionViewModel clonedPatientActionVM = EpisodeBtnConfig.ActionButtonVM.Clone() as RehabActionViewModel;
                                        clonedPatientActionVM.ControllerName = "Patient";
                                        clonedPatientActionVM.ActionName = "IndexTreatingSpecailty";
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
                            cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                            cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;  // use invoked parameter as is
                            cmdBtnTemplateVM.Stage = button.Key;
                            cmdBtnTemplateVM.TextNode = (button.Value.ButtonTitle == "Base") ? $"Episode of Care" :
                              $"{button.Value.ButtonTitle} for {admissionDate}";
                            cmdBtnTemplateVM.Title = (cmdBtnTemplateVM.Stage == "New") ?
                              button.Value.ButtonTitle.Replace("New", $"Edit episode") :
                              $"{button.Value.ButtonTitle} for {admissionDate}";

                            /* don't use EpisodeBtnConfig.EpisodeOfCareID here */
                            if (EpisodeBtnConfig.ActionButtonVM.EpisodeID > 0)
                                cmdBtnTemplateVM.ShowThisButton = false;
                            else
                                cmdBtnTemplateVM.ShowThisButton = true;
                            break;
                        }
                    default:
                        {
                            /* don't use EpisodeBtnConfig.EpisodeOfCareID here */
                            if (EpisodeBtnConfig.ActionButtonVM.EpisodeID > 0)
                            {
                                cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                                cmdBtnTemplateVM.ShowThisButton = true;
                                cmdBtnTemplateVM.Stage = button.Key;
                                cmdBtnTemplateVM.Title = (cmdBtnTemplateVM.Stage == "New") ?
                                button.Value.ButtonTitle.Replace("Create new", $"Edit existing {admissionDate}") : button.Value.ButtonTitle;
                                cmdBtnTemplateVM.TextNode = button.Value.ButtonTitle == "Base" ?
                                  $"Episode of Care {admissionDate}" : button.Value.ButtonTitle;
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
