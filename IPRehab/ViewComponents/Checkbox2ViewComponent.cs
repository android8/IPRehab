using IPRehab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehab.ViewComponents
{
  public class Checkbox2ViewComponent : ViewComponent
  {
    public Checkbox2ViewComponent()
    {
    }
    public Task<IViewComponentResult> InvokeAsync(int UserID, int QuestionID, string QuestionKey, string StageTitle, IList<SelectListItem> ChoiceList)
    {
      ViewData["UserID"] = UserID;
      ViewData["QuestionID"] = QuestionID;
      ViewData["QuestionKey"] = QuestionKey;
      ViewData["StageTitle"] = StageTitle;
      return Task.FromResult<IViewComponentResult>(View("MaterialChkboxFlexDirectionRow", ChoiceList));
    }
  }
}
