using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  public class HttpResponseException : Exception
  {
    public int Status { get; set; } = 500;

    public object Value { get; set; }
  }
}
