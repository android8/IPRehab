using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

///
namespace IPRehabWebAPI2.Controllers
{
  /// <summary>
  /// https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-5.0
  /// </summary>
  [ApiController]
  public class ErrorController : ControllerBase
  {
    [HttpGet]
    [Route("/error-local-development")]
    private IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
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
    private IActionResult Error() => Problem();
  }
}
