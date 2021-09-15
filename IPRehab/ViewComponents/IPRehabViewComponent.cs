using IPRehab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.ViewComponents
{
  public class IPRehabViewComponent : ViewComponent
  {
    public IPRehabViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(int ControlCounter, QuestionWithSelectItems QWS, string NetworkID)
    {
      ViewComponenViewModel thisVCVM = new();
      thisVCVM.ControlCounter = ControlCounter;
      thisVCVM.UserID = NetworkID;
      thisVCVM.QuestionID = QWS.QuestionID;
      thisVCVM.QuestionKey = QWS.QuestionKey;
      thisVCVM.StageTitle = QWS.StageTitle;
      thisVCVM.MultipleAnswers = QWS.MultipleChoices;

      thisVCVM.DisplayStageHeader = false;
      thisVCVM.DisplayStageHeader = QWS.QuestionKey.Contains("Q43") ||
                                    QWS.QuestionKey.Contains("D0150") ||
                                    QWS.QuestionKey.Contains("K0520") ||
                                    QWS.QuestionKey.Contains("GG0130") ||
                                    QWS.QuestionKey.Contains("GG0170") ||
                                    QWS.QuestionKey.Contains("M0300") ||
                                    QWS.QuestionKey.Contains("N0415") ||
                                    QWS.QuestionKey.Contains("O0110");

      thisVCVM.StageHeaderBorderCssClass = "stageHeaderNoBottomBorder";
      thisVCVM.ContainerCssClass = "flex-start-column-nowrap";

      string viewName = string.Empty;
      switch (QWS.ChoiceList.Count)
      {
        case int n when n > 3:
          viewName = "DropDown";
          if (QWS.QuestionKey.Contains("O0401"))
          {
            /* week1 and week2 therapy with total hours*/
            viewName = "DropDownPT";
          }
          if (QWS.QuestionKey.Contains("A10"))
          {
            viewName = "MaterialChkboxBoxBeforeHeaderEthnicity";
            thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
            thisVCVM.StageHeaderBorderCssClass = "stageHeaderNoLeftBorder";
          }
          break;
        case int n when n >= 2 && n <= 3:
          viewName = "RadioFlexDirectionColumnLongText2";
          if (QWS.Question.Contains("Is this assessment completed and ready for processing") ||
              QWS.QuestionKey == "A1110B" ||
              QWS.QuestionKey == "C131A" ||
              QWS.QuestionKey == "C0300C" ||
              QWS.QuestionKey == "J1750" ||
              QWS.QuestionKey == "J1900" ||
              QWS.QuestionKey == "J2000" ||
              QWS.QuestionKey == "Q8" ||
              QWS.QuestionKey.Contains("Q14") ||
              QWS.QuestionKey == "Q24A" ||
              QWS.QuestionKey.Contains("Q41") ||
              QWS.QuestionKey.Contains("Q42") ||
              QWS.QuestionKey.Contains("Q44C") ||
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
              viewName = "MaterialChkboxBoxAfterHeader";

              if (QWS.QuestionKey.Contains("K0520") ||
                QWS.QuestionKey.Contains("N0415") ||
                QWS.QuestionKey.Contains("O0110"))
              {
                viewName = "MaterialChkboxBoxBeforeHeader";
                thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
                thisVCVM.StageHeaderBorderCssClass = "stageHeaderNoLeftBorder";
              }
              break;
            case "Date":
              viewName = "MaterialInputDate";
              thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
              break;
            case "Number":
              viewName = "MaterialInputNumber";
              break;
            case "TextArea":
              viewName = "MaterialTextArea";
              break;
            case "ICD":
              viewName = "MaterialInputText";
              thisVCVM.ContainerCssClass = "flex-start-row-nowrap";
              break;
            case "FreeText":
              viewName = "MaterialInputText";
              break;
          }
          break;
      }
      return Task.FromResult<IViewComponentResult>(View(viewName, new ViewComponentTemplateModel
      {
        ChoiceAndAnswerList = QWS.ChoicesAnswers,
        ViewComponentViewModel = thisVCVM
      }));
    }
  }
}
