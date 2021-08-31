using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IPRehabWebAPI2.Filters
{
  public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
  {
    public int Order { get; } = int.MaxValue - 10; //an Order of the maximum integer value minus 10 allows other filters to run at the end of the pipeline.

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
      if (context.Exception is HttpResponseException exception)
      {
        context.Result = new ObjectResult(exception.Value)
        {
          StatusCode = exception.Status,
        };
        context.ExceptionHandled = true;
      }
    }
  }
}