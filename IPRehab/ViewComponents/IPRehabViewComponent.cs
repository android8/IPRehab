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
    public IPRehabViewComponent() { 
    }

    public Task<IViewComponentResult> InvokeAsync(QuestionWithSelectItems QWS, string NetworkID)
    {
      ViewData["UserID"] = NetworkID;
      ViewData["QuestionID"] = QWS.QuestionID;
      ViewData["QuestionKey"] = QWS.QuestionKey;
      ViewData["StageTitle"] = QWS.StageTitle;
      ViewData["CssClass"] = "radio-with-long-text";
      ViewData["MultipleAnswers"] = QWS.MultipleChoices;

      string viewName = string.Empty; 

      switch(QWS.ChoiceList.Count)
      {
        case int n when n > 3:
          viewName = "DropDownDefault2";
          ViewData["DisplayStageHeader"] = QWS.QuestionKey.Contains("GG0130A") || QWS.QuestionKey.Contains("GG0170A");
          break;
        case int n when n >= 2 && n <= 3:
          viewName = "RadioFlexDirectionColumnLongText2";
          break;
        default:
          switch(QWS.AnswerCodeCategory)
          {
            case "Checked":
              viewName = "MaterialChkboxFlexDirectionRow";
              ViewData["DisplayStageHeader"] = QWS.QuestionKey.Contains("K0520A");
              break;
            case "Date":
              viewName = "MaterialInputDate";
              break;
            case "Number":
              viewName = "MaterialInputNumber";
              break;
            case "TextArea":
              viewName = "MaterialTextArea";
              break;
            case "ICD":
            case "FreeText":
              viewName = "MaterialInputText";
              break;
          }
          break;
      }
      return Task.FromResult<IViewComponentResult>(View(viewName, QWS.ChoicesAnswers));
    }
  }
}
