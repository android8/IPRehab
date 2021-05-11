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
  public class UserFacilityAffiliationViewComponent : ViewComponent
  {
    public UserFacilityAffiliationViewComponent()
    {
    }

    public Task<IViewComponentResult> InvokeAsync(IEnumerable<VUserAnswerFacilitySummary2> userFacilityAffiliation)
    {
      string viewName = "UserFacilityAffiliation";
      return Task.FromResult<IViewComponentResult>(View(viewName, userFacilityAffiliation));
    }
  }
}
