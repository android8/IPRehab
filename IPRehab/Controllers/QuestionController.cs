using IPRehab.Helpers;
using IPRehabModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IPRehab.Controllers
{
   public class QuestionController : Controller
   {
      private readonly ILogger<QuestionController> _logger;
      private readonly IConfiguration _configuration;
      private readonly string _apiBaseUrl;

      public QuestionController(ILogger<QuestionController> logger, IConfiguration configuration)
      {
         _logger = logger;
         _configuration = configuration;
         _apiBaseUrl = _configuration.GetValue<string>("WebAPIBaseUrl");
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
         List<TblQuestion> questions = new List<TblQuestion>();
         JsonSerializerOptions options = new JsonSerializerOptions()
         {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true
         };

         //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
         //HttpResponseMessage Res = await APIAgent<IEnumerable<TblQuestion>>.GetDataAsync(_apiBaseUrl, "api/Questions/GetInitialStage");
         
         var ApiResponse = await APIAgent.StreamWithSystemTextJson(
            new Uri($"{_apiBaseUrl}/api/Questions/GetInitialStage"), options);

         //returning the question list to view  
         return View(ApiResponse);
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
