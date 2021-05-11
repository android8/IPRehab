using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PCC_FIT.Models;
using PCC_FIT_Repository_CORELibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCC_FIT.ViewComponents
{
  public class RadioButton2ViewComponent : ViewComponent
  {
    public RadioButton2ViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(int userID, int questionID, string questionKey, IList<ChoiceViewModel> choiceList, string question, string radioGroupName)
    {
      ViewData["UserID"] = userID;
      ViewData["RadioGroupName"] = radioGroupName;
      ViewData["QuestionID"] = questionID;
      ViewData["QuestionKey"] = questionKey;
      //List<SelectListItem> listOfSelectListItem = new List<SelectListItem>();

      //foreach (ChoiceViewModel choice in choiceList)
      //{
      //  SelectListItem thisSelectListItem = new SelectListItem()
      //  {
      //    Value = choice.ID.ToString(),
      //    Text = choice.Choice,
      //    Selected = choice.Selected,
      //  };
      //  listOfSelectListItem.Add(thisSelectListItem);
      //}

      string viewName = "RadioDefault2";

      if (question == "Level of FIT Engagement")
      {
        viewName = "RadioFlexDirectionColumnLongText2";
      }

      //if (question == "Stages of Transformation")
      //{
      //  viewName = "RadioFlexDirectionColumn2";
      //}
      return Task.FromResult<IViewComponentResult>(View(viewName, choiceList));
    }
  }
}
