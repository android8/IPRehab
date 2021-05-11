using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PCC_FIT.Models;
using PCC_FIT_Repository_CORELibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PCC_FIT.ViewComponents
{
  public class Checkbox2ViewComponent : ViewComponent
  {
    public Checkbox2ViewComponent()
    {
    }
    public Task<IViewComponentResult> InvokeAsync(int userID, int questionID, string questionKey, IList<ChoiceViewModel> choiceList)
    {
      //List<ChoiceViewModel> CheckboxList = new List<ChoiceViewModel>();

      //foreach (ChoiceViewModel choice in choiceList)
      //{
      //  CheckboxViewModel thisSelectListItem = new CheckboxViewModel()
      //  {
      //    UserID = userID,
      //    QuestionID = questionID,
      //    QuestionKey = questionKey,
      //    UserAnswerID = choice.UserAnswerID,
      //    Value = choice.ID,
      //    Label = choice.Choice,
      //    Selected = choice.Selected
      //  };
      //  CheckboxList.Add(thisSelectListItem);
      //}

      ViewData["UserID"] = userID;
      ViewData["QuestionID"] = questionID;
      ViewData["QuestionKey"] = questionKey;
      return Task.FromResult<IViewComponentResult>(View("MaterialChkboxFlexDirectionRow", choiceList));
    }
  }
}
