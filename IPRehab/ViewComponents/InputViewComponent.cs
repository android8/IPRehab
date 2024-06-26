﻿using IPRehab.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPRehab.ViewComponents
{
    public class InputViewComponent : ViewComponent
    {
        public InputViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(int EpisodeID, int ControlCounter, QuestionWithSelectItems QWS, string NetworkID)
        {
            InputViewComponenViewModel thisVCVM = new();
            thisVCVM.ControlCounter = ControlCounter;
            thisVCVM.UserID = NetworkID;
            thisVCVM.EpisodeID = EpisodeID;
            thisVCVM.QuestionID = QWS.QuestionID;
            thisVCVM.QuestionKey = QWS.QuestionKey;
            thisVCVM.MeasureID = QWS.MeasureID.ToString();
            thisVCVM.MeasureDescription = QWS.MeasureDescription;
            thisVCVM.MeasureTitleNormalized = QWS.MeasureDescription;

            if (QWS.MeasureDescription != string.Empty)
            {
                string source = QWS.MeasureDescription;
                if (source.IndexOf(" ") != -1)
                {
                    thisVCVM.MeasureDescription = source.Replace(" ", "_");
                }
                else
                {
                    /* separate camel case in Measure type with "_" to create id property in the DOM */
                    string midCaps = string.Concat(Regex.Matches(source, "[A-Z]").OfType<Match>().Select(match => match.Value));
                    if (midCaps.Length > 1)
                    {
                        string middleCapLeter = midCaps.Substring(1, 1);
                        string underlineSeparated = source.Replace(middleCapLeter, $"_{middleCapLeter}");
                        thisVCVM.MeasureDescription = underlineSeparated;
                    }
                }
            }

            thisVCVM.StageID = QWS.StageID;
            thisVCVM.MultipleChoices = QWS.MultipleChoices;
            thisVCVM.Required = QWS.Required.Value;
            thisVCVM.ReadOnly = QWS.ReadOnly.Value;
            thisVCVM.DisplayMeasureHeader = (!QWS.QuestionKey.StartsWith("Q") || QWS.QuestionKey.StartsWith("Q43")) && (QWS.MeasureDescription != string.Empty);
            thisVCVM.MeasureHeaderBorderCssClass = "measureHeaderNoBottomBorder";
            thisVCVM.ContainerCssClass = "flex-start-column-nowrap";

            string viewName = string.Empty;
            switch (QWS.ChoiceList.Count)
            {
                case int n when n > 3:
                    viewName = "DropDown";
                    if (QWS.QuestionKey.Contains("O0401", System.StringComparison.OrdinalIgnoreCase) || QWS.QuestionKey.Contains("O0402", System.StringComparison.OrdinalIgnoreCase))
                    {
                        viewName = "DropDownPT";

                        if (QWS.QuestionKey.Contains("O0401", System.StringComparison.OrdinalIgnoreCase))
                        {
                            /* default codeset id 430 for wk1 therapy, will change by javascript with thearpy type changes, */
                            thisVCVM.TherapyHoursCodeSetID = 430;
                            thisVCVM.TherapyHoursCodeSetValue = "PHY-Individual-Minutes";
                        }
                        else
                        {
                            /* default codeset id 442 for wk2 threapy, will change by javascript with thearpy type changes, */
                            thisVCVM.TherapyHoursCodeSetID = 442;
                            thisVCVM.TherapyHoursCodeSetValue = "PHY-Individual-Minutes";
                        }
                    }

                    if (QWS.QuestionKey.Contains("A10", System.StringComparison.OrdinalIgnoreCase))
                    {
                        viewName = "MaterialChkboxBoxBeforeHeaderEthnicity";
                        thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
                        thisVCVM.MeasureHeaderBorderCssClass = "measureHeaderNoLeftBorder";
                    }
                    break;
                case int n when n >= 2 && n <= 3:
                    viewName = "RadioFlexDirectionColumnLongText2";
                    if (QWS.Question.Contains("Is this assessment completed and ready for processing", System.StringComparison.OrdinalIgnoreCase) ||
                        QWS.QuestionKey == "A1110B" ||
                        QWS.QuestionKey == "C131A" ||
                        QWS.QuestionKey == "C0300C" ||
                        QWS.QuestionKey == "J1750" || QWS.QuestionKey == "J1750" || QWS.QuestionKey == "J1900" || QWS.QuestionKey == "J2000" || QWS.QuestionKey == "Q8" ||
                        QWS.QuestionKey.Contains("Q14", System.StringComparison.OrdinalIgnoreCase) || QWS.QuestionKey == "Q24A" || QWS.QuestionKey.Contains("Q41", System.StringComparison.OrdinalIgnoreCase) ||
                        QWS.QuestionKey.Contains("Q42", System.StringComparison.OrdinalIgnoreCase) || QWS.QuestionKey.Contains("Q44C", System.StringComparison.OrdinalIgnoreCase) ||
                        QWS.QuestionKey == "GG0170RR" ||
                        QWS.QuestionKey == "GG0170SS")
                    {
                        thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
                    }
                    break;
                default:
                    switch (QWS.AnswerCodeCategory)
                    {
                        case "Checked":
                            viewName = "MaterialChkboxBoxBeforeHeader";
                            thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
                            thisVCVM.MeasureHeaderBorderCssClass = "measureHeaderNoLeftBorder";

                            //if (QWS.QuestionKey.Contains("N0415", System.StringComparison.OrdinalIgnoreCase) ||
                            //  QWS.QuestionKey.Contains("O0110", System.StringComparison.OrdinalIgnoreCase))
                            //{
                            //  viewName = "MaterialChkboxBoxBeforeHeader";
                            //  thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
                            //  thisVCVM.MeasureHeaderBorderCssClass = "measureHeaderNoLeftBorder";
                            //}
                            break;
                        case "Date":
                            viewName = "MaterialInputDate";
                            thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
                            break;
                        case "Number":
                            thisVCVM.MeasureHeaderBorderCssClass = "measureHeaderNoBottomBorder";
                            viewName = "MDNumberAfterHeader";
                            break;
                        case "TextArea":
                            viewName = "MaterialTextArea";
                            break;
                        case "ICD":
                        case "FreeText":
                            viewName = "MaterialInputText";
                            if (QWS.QuestionKey.Contains("M0300", System.StringComparison.OrdinalIgnoreCase))
                            {
                                thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
                                thisVCVM.MeasureHeaderBorderCssClass = "measureHeaderNoLeftBorder";
                                viewName = "MDNumberAfterHeader";
                            }
                            break;
                    }
                    break;
            }
            return Task.FromResult<IViewComponentResult>(View(viewName, new InputViewComponentTemplateModel
            {
                ChoiceAndAnswerList = QWS.ChoicesAnswers,
                ViewComponentViewModel = thisVCVM
            }));
        }
    }
}
