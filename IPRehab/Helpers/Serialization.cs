using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
   public class JsonSerialization
   {
      JsonSerializerOptions options = new()
      {
         ReferenceHandler = ReferenceHandler.Preserve,
         WriteIndented = true
      };

      public string Serialize(object obj)
      {
         string responseJson = JsonSerializer.Serialize(obj, options);
         return responseJson;
      }
   }
}
