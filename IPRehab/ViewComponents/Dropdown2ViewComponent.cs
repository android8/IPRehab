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
  public class Dropdown2ViewComponent : ViewComponent
  {
    public Dropdown2ViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(int userID, int questionID, string questionKey, IList<ChoiceViewModel> choiceList)
    {
      //List<SelectListItem> listOfSelectListItem = new List<SelectListItem>();

      //foreach (ChoiceViewModel choice in choiceList)
      //{
      //  SelectListItem thisSelectListItem = new SelectListItem()
      //  {
      //    Value = choice.ID.ToString(),
      //    Text = choice.Choice,
      //    Selected = choice.Selected
      //  };
      //  listOfSelectListItem.Add(thisSelectListItem);
      //}

      ViewData["UserID"] = userID;
      ViewData["QuestionID"] = questionID;
      ViewData["QuestionKey"] = questionKey;

      foreach (var c in choiceList)
      {
        if (c.UserAnswerID.HasValue)
        {
          ViewData["UserAnswerID"] = c.UserAnswerID;
          break;
        }
      }

      string viewName = "DropDownDefault2";

      return Task.FromResult<IViewComponentResult>(View(viewName, choiceList));
    }
  }
}
