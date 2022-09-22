using IPRehab.Helpers;
using IPRehab.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

///
namespace IPRehab.Controllers
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-5.0
    /// </summary>
    [ApiController]
    public class ErrorController : Controller
    {
        [HttpGet]
        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [HttpGet]
        [Route("/error")]
        //private IActionResult Error() => Problem();
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            ErrorViewModel errorViewModdel = new() {
                ExceptionCategory = context.Error.GetType()?.Name,
                Message = context.Error.Message,
                InnerExceptionMessage =context.Error.InnerException?.Message,
                ErrorPartial = new()
                {
                    ExceptionCategory = context.Error.GetType()?.Name,
                    Message = context.Error.Message,
                    InnerExceptionMessage = context.Error.InnerException?.Message,
                }
            };

            return View("Error", errorViewModdel);

            //PartialView("_ErrorPartial", 
            //    new ErrorViewModelHelper()
            //    .Create("Web API content is not an object or mededia type is not applicaiton/json", 
            //        string.Empty, string.Empty));
        }
    }
}
