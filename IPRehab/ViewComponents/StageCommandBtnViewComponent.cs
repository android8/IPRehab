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
                    //Patient button
                    case "Patient":
                        {
                            switch (HostingPage)
                            {
                                //do not show patient button in patient listing
                                case "Patient":
                                    {
                                        cmdBtnTemplateVM.ShowThisButton = false;
                                        break;
                                    }
                                //show patient button with name as "Patient List" in questions page
                                case "Question":
                                    {
                                        cmdBtnTemplateVM.ShowThisButton = true;
                                        cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                                        cmdBtnTemplateVM.TextNode = "Patient List";
                                        RehabActionViewModel clonedPatientActionVM = EpisodeBtnConfig.ActionButtonVM.Clone() as RehabActionViewModel;
                                        clonedPatientActionVM.ControllerName = "Patient";
                                        clonedPatientActionVM.ActionName = "IndexTreatingSpecailty";
                                        clonedPatientActionVM.PatientID = string.Empty;
                                        clonedPatientActionVM.EpisodeID = EpisodeBtnConfig.EpisodeOfCareID;
                                        cmdBtnTemplateVM.ActionVM = clonedPatientActionVM;  //use cloned
                                        break;
                                    }
                            }
                            break;
                        }

                    //New button
                    case "New":
                        {
                            /* don't use EpisodeBtnConfig.EpisodeOfCareID here */
                            if (EpisodeBtnConfig.EpisodeOfCareID > 0)
                                cmdBtnTemplateVM.ShowThisButton = false;
                            else
                            {
                                cmdBtnTemplateVM.ShowThisButton = true;
                                cmdBtnTemplateVM.ActionVM.EpisodeID = EpisodeBtnConfig.EpisodeOfCareID;
                                cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                                cmdBtnTemplateVM.ActionVM = EpisodeBtnConfig.ActionButtonVM;  // use invoked parameter as is
                                cmdBtnTemplateVM.Stage = button.Key;
                                cmdBtnTemplateVM.TextNode = (button.Value.ButtonTitle == "Base") ? $"Episode of Care" :
                                  $"{button.Value.ButtonTitle} for {admissionDate}";
                                cmdBtnTemplateVM.Title = (cmdBtnTemplateVM.Stage == "New") ?
                                  button.Value.ButtonTitle.Replace("New", $"Edit episode") :
                                  $"{button.Value.ButtonTitle} for {admissionDate}";
                            }
                            break;
                        }

                    //other buttons
                    default:
                        {
                            /* don't use EpisodeBtnConfig.EpisodeOfCareID here */
                            if (EpisodeBtnConfig.EpisodeOfCareID > 0)
                            {
                                cmdBtnTemplateVM.ShowThisButton = true;
                                cmdBtnTemplateVM.ActionVM.EpisodeID = EpisodeBtnConfig.EpisodeOfCareID;
                                cmdBtnTemplateVM.ActionBtnCssClass = button.Value.ButtonCss;
                                cmdBtnTemplateVM.Stage = button.Key;
                                cmdBtnTemplateVM.Title = (cmdBtnTemplateVM.Stage == "New") ?
                                button.Value.ButtonTitle.Replace("Create new", $"Edit existing {admissionDate}") : button.Value.ButtonTitle;
                                cmdBtnTemplateVM.TextNode = button.Value.ButtonTitle == "Base" ?
                                  $"Episode of Care" : button.Value.ButtonTitle;
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
