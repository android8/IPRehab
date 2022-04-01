using IPRehab.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IPRehab.Controllers
{
  //ToDo: [Authorize]
  public class HomeController : BaseController
  {
    public HomeController(IWebHostEnvironment environment, ILogger<HomeController> logger, IConfiguration configuration) 
      : base(environment, configuration, logger)
    {
    }

    public IActionResult Splash()
    {
      string vm = $"Sponsored by {base.Office}";
      return View("splash", vm);
    }

    public IActionResult Index(string searchCriteria, int pageNumber, string orderBy)
    {
      RehabActionViewModel vm = new();
      vm.SearchCriteria = searchCriteria;
      vm.PageNumber = pageNumber;
      vm.OrderBy = orderBy;
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }
  }
}
