using IPRehab.Helpers;
using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
  //ToDo: [Authorize]
  public class QuestionController : BaseController
  {
    ILogger<QuestionController> _logger;
    public QuestionController(ILogger<QuestionController> logger, IConfiguration configuration) : base(configuration)
    {
      _logger = logger;
    }

    // GET: QuestionController
    public ActionResult Index()
    {
      return View();
    }

    // GET: QuestionController/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }

    // GET: QuestionController/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: QuestionController/Create
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
    /// <summary>
    /// https://www.stevejgordon.co.uk/sending-and-receiving-json-using-httpclient-with-system-net-http-json
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // GET: QuestionController/Edit/5
    public async Task<ActionResult> Edit(string stage, int? id)
    {
      ViewBag.Title = stage == "Followup"? "Follow Up": stage;
      List<QuestionDTO> questions = new List<QuestionDTO>();

      try
      {
        //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
        HttpResponseMessage Res;
        if (string.IsNullOrEmpty(stage))
        {
          Res = await APIAgent.GetDataAsync(new Uri($"{_apiBaseUrl}/api/Question/GetAll"));
          ViewBag.Title = "IRF-PAI Form";
        }
        else
        { 
          Res = await APIAgent.GetDataAsync(new Uri($"{_apiBaseUrl}/api/Question/Get{stage}Stage")); 
        }
        string httpMsgContentReadMethod = "ReadAsStringAsync";
        if (Res.Content is object && Res.Content.Headers.ContentType.MediaType == "application/json")
        {
          List<QuestionWithSelectItems> vm = new List<QuestionWithSelectItems>();
          try
          {
            switch (httpMsgContentReadMethod)
            {
              case "ReadAsStringAsync":
                string questionString = await Res.Content.ReadAsStringAsync();
                questions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuestionDTO>>(questionString);
                break;

              case "ReadAsAsync":
                questions = await Res.Content.ReadAsAsync<List<QuestionDTO>>();
                break;
              case "ReadAsStreamAsync":
                var contentStream = await Res.Content.ReadAsStreamAsync();
                questions = await JsonSerializer.DeserializeAsync<List<QuestionDTO>>(contentStream, _options);
                break;
            }
            foreach (var dto in questions)
            {
              /* convert IEnumerable<QuestionDTO> to IEnumerable<QuestionWithSelectItems>
               * where the ChoiceList property is a list of SelectListItem */
              QuestionWithSelectItems qws = HydrateVM.Hydrate(dto);
              vm.Add(qws);
            }

            //returning the question list to view  
            return View(vm);
          }
          catch (Exception ex) // Could be ArgumentNullException or UnsupportedMediaTypeException
          {
            Console.WriteLine("HTTP Response was invalid or could not be deserialised.");
            Console.WriteLine($"{ex.Message}");
            if (ex.InnerException != null)
            {
              Console.WriteLine($"{ex.InnerException.Message}");
            }
            return null;
          }
        }
        else
        {
          return null;
        }

      }
      catch (Exception ex)
      {
        Console.WriteLine("WebAPI call failure.");
        Console.WriteLine($"{ex.Message}");
        if (ex.InnerException != null)
        {
          Console.WriteLine($"{ex.InnerException.Message}");
        }
        return null;
      }
    }

    // POST: QuestionController/Edit/5
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

    // GET: QuestionController/Delete/5
    public ActionResult Delete(int id)
    {
      return View();
    }

    // POST: QuestionController/Delete/5
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
