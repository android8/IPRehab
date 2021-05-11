using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PCC_FIT.Models;
using PCC_Fit_Model_CORELibrary;
using PCC_FIT_Repository_CORELibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCC_FIT.ViewComponents
{
  public class SubmitButtonViewComponent : ViewComponent
  {
    public SubmitButtonViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(int facilityID, int fy, bool readOnly, List<QandASetSummaryItem> q_a_summary, bool cloning, int QuestionPageID)
    {
      MDSubmitButtonViewModel vm = new MDSubmitButtonViewModel();
      vm.FacilityID = facilityID;
      vm.FiscalYear = fy;
      string viewName = "MDSubmitButton";
      return Task.FromResult<IViewComponentResult>(View(viewName, vm));
    }
  }
}
