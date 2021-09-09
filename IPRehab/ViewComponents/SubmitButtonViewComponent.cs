using IPRehab.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehab.ViewComponents
{
  public class SubmitButtonViewComponent : ViewComponent
  {
    public SubmitButtonViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(int facilityID, int fy)
    {
      MDSubmitButtonViewModel vm = new MDSubmitButtonViewModel();
      vm.FacilityID = facilityID;
      vm.FiscalYear = fy;
      string viewName = "MDSubmitButton";
      return Task.FromResult<IViewComponentResult>(View(viewName, vm));
    }
  }
}
