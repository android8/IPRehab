using IPRehab.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace IPRehab.Controllers
{
    //ToDo: [Authorize]
    public class HomeController : BaseController
  {
    public HomeController(IWebHostEnvironment environment, IMemoryCache meoryCache, IConfiguration configuration) 
      : base(environment, meoryCache, configuration)
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
