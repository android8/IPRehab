using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehab.ViewComponents
{
  public class RadioButton2ViewComponent : ViewComponent
  {
    public RadioButton2ViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(int UserID, int QuestionID, string QuestionKey, string StageTitle, IList<SelectListItem> ChoiceList)
    {
      ViewData["UserID"] = UserID;
      ViewData["QuestionID"] = QuestionID;
      ViewData["QuestionKey"] = QuestionKey;
      ViewData["StageTitle"] = StageTitle;
      ViewData["CssClass"] = "radio-with-long-text";

      string viewName = "RadioFlexDirectionColumnLongText2";

      return Task.FromResult<IViewComponentResult>(View(viewName, ChoiceList));
    }
  }
}
