using IPRehab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public class ErrorViewModelHelper
  {
    public ErrorPartialViewModel Create(string category, string message, string innerExceptionMessage)
    {
      return new ErrorPartialViewModel()
      {
        ExceptionCategory = category,
        Message = message,
        InnerExceptionMessage = innerExceptionMessage
      };
    }
  }
}
