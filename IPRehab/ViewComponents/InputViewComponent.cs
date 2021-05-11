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
  public class InputViewComponent : ViewComponent
  {
    public InputViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(int userID, int questionID, string questionKey, IList<ChoiceViewModel> choiceList, int? thisLength, int? thisUiWidth)
    {
      //List<NumberInputViewModel> numberInputs = new List<NumberInputViewModel>();
      //foreach (ChoiceViewModel choice in choiceList)
      //{
      //  NumberInputViewModel thisInput = new NumberInputViewModel();
      //  thisInput.ID = choice.ID;
      //  thisInput.ValueKey = choice.ValueKey;
      //  thisInput.Name = questionKey + "-" + choice.ID;
      //  thisInput.Label = choice.Choice;
      //  thisInput.Value = choice.ID; /* this is always the foreign key to tblCodeSet */
      //  thisInput.OtherDescription = choice.OtherDescription; /* use this to describe the code set value*/
      //  thisInput.Max = 100;
      //  thisInput.Min = 0;
      //  numberInputs.Add(thisInput);
      //}

      ViewData["UserID"] = userID;
      ViewData["QuestionID"] = questionID;
      ViewData["QuestionKey"] = questionKey;
      return Task.FromResult<IViewComponentResult>(View("InputFlexDirectionRow", choiceList));
    }
  }
}
