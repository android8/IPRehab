using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
   public class ApiResponseModel
   {
      public object Result { get; set; }   
      public int RecordCount { get; set; }
   }
}
