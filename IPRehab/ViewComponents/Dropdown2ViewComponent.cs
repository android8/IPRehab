using IPRehab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehab.ViewComponents
{
  public class Dropdown2ViewComponent : ViewComponent
  {
    public Dropdown2ViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(string UserID, int QuestionID, string QuestionKey, string StageTitle, IList<ChoiceAndAnswer> ChoiceAndAnswers)
    {
      ViewData["UserID"] = UserID;
      ViewData["QuestionID"] = QuestionID;
      ViewData["QuestionKey"] = QuestionKey;
      ViewData["StageTitle"] = StageTitle;
      string viewName = "DropDownDefault2";

      return Task.FromResult<IViewComponentResult>(View(viewName, ChoiceAndAnswers));
    }
  }
}
