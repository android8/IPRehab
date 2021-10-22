using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehab.ViewComponents
{
  public class Input2ViewComponent : ViewComponent
  {
    public Input2ViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(int userID, int questionID, string questionKey, IList<SelectListItem> choiceList, int? thisLength, int? thisUiWidth)
    {
      ViewData["UserID"] = userID;
      ViewData["QuestionID"] = questionID;
      ViewData["QuestionKey"] = questionKey;
      return Task.FromResult<IViewComponentResult>(View("InputFlexDirectionRow", choiceList));
    }
  }
}
