using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IPRehab.Controllers
{
  public class EpisodeController : BaseController
  {
    ILogger<EpisodeController> _logger;
    public EpisodeController(ILogger<EpisodeController> logger, IConfiguration configuration) : base(configuration)
    {
      _logger = logger;
    }
    // GET: EpisodeController
    public ActionResult Index()
    {
      return View();
    }

    // GET: EpisodeController/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }

    // GET: EpisodeController/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: EpisodeController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(IFormCollection collection)
    {
      try
      {
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }

    // GET: EpisodeController/Edit/5
    public ActionResult Edit(int id)
    {
      return View();
    }

    // POST: EpisodeController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
      try
      {
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }

    // GET: EpisodeController/Delete/5
    public ActionResult Delete(int id)
    {
      return View();
    }

    // POST: EpisodeController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
      try
      {
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }
  }
}
