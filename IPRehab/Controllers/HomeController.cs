using IPRehab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IPRehab.Controllers
{
  //ToDo: [Authorize]
  public class HomeController : BaseController
  {
    public HomeController(ILogger<HomeController> logger, IConfiguration configuration) 
      : base(configuration, logger)
    {
    }

    public IActionResult Splash()
    {
      string vm = _office;
      return View("splash", vm);
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }
  }
}
