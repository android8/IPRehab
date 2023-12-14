using IPRehab.Helpers;
using IPRehab.Models;
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
                    case "Patient": //Patient button
                        {
                            if (HostingPage == "Patient")    //do not show patient button in patient listing
                                cmdBtnTemplateVM.ShowThisButton = false;
                            else if (HostingPage == "Question")
                                cmdBtnTemplateVM.ShowThisButton = true;

                            if (cmdBtnTemplateVM.ShowThisButton)
                            {
                                RehabActionViewModel clonedPatientActionVM = EpisodeBtnConfig.ActionButtonVM.Clone() as RehabActionViewModel;   //use cloned
                                clonedPatientActionVM.EpisodeID = EpisodeBtnConfig.EpisodeOfCareID;
                                clonedPatientActionVM.ControllerName = "Patient";
                                clonedPatientActionVM.ActionName = "IndexTreatingSpecailty";
                                clonedPatientActionVM.PatientID = string.Empty;
                                cmdBtnTemplateVM.ActionVM = clonedPatientActionVM;
                                cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                                cmdBtnTemplateVM.TextNode = "Patient List";
                            }
                            break;
                        }
                    case "New": //New button
                        {
                            if (EpisodeBtnConfig.ActionButtonVM.EpisodeID > 0 || HostingPage == "Question")
                                cmdBtnTemplateVM.ShowThisButton = false;
                            else
                            {
                                cmdBtnTemplateVM.ShowThisButton = true;
                            }

                            if (cmdBtnTemplateVM.ShowThisButton)
                            {
                                cmdBtnTemplateVM.ActionVM.EpisodeID = -1;
                                cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                                cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;  // use invoked parameter as is
                                cmdBtnTemplateVM.Stage = button.Key;
                                cmdBtnTemplateVM.TextNode = (button.Value.ButtonTitle == "Base") ?
                                    $"Episode of Care" : $"{button.Value.ButtonTitle} for {admissionDate}";
                                cmdBtnTemplateVM.Title = (cmdBtnTemplateVM.Stage == "New") ?
                                  button.Value.ButtonTitle.Replace("New", $"Edit episode") : $"{button.Value.ButtonTitle} for {admissionDate}";
                            }
                            break;
                        }

                    default:    //other buttons
                        {
                            if (EpisodeBtnConfig.ActionButtonVM.EpisodeID < 0 && HostingPage == "Patient")
                                cmdBtnTemplateVM.ShowThisButton = false;
                            else
                                cmdBtnTemplateVM.ShowThisButton = true;

                            if (cmdBtnTemplateVM.ShowThisButton)
                            {
                                cmdBtnTemplateVM.ActionVM.EpisodeID = EpisodeBtnConfig.EpisodeOfCareID;
                                cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                                cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;  // use invoked parameter
                                cmdBtnTemplateVM.Stage = button.Key;
                                cmdBtnTemplateVM.TextNode = button.Value.ButtonTitle == "Base" ?
                                  $"Episode of Care" : button.Value.ButtonTitle;
                                cmdBtnTemplateVM.Title = (cmdBtnTemplateVM.Stage == "New") ?
                                button.Value.ButtonTitle.Replace("Create new", $"Edit existing {admissionDate}") : button.Value.ButtonTitle;
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
