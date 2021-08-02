using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class ErrorPartialViewModel
  {
    public string ExceptionCategory { get; set; }
    public string Message { get; set; }
    public string InnerExceptionMessage { get; set; }
  }
}
