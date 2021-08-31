using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public static class SerializationGeneric<T> where T : class
  {
    public static async Task<List<T>> SerializeAsync(string url, JsonSerializerOptions _options)
    {
      List<T> theList = new List<T>();
      HttpResponseMessage Res;
      Res = await APIAgent.GetDataAsync(new Uri(url));
      string httpMsgContentReadMethod = "ReadAsStreamAsync";
      System.IO.Stream contentStream = null;
      if (Res.Content is object && Res.Content.Headers.ContentType.MediaType == "application/json")
      {
        switch (httpMsgContentReadMethod)
        {
          case "ReadAsAsync":
            theList = await Res.Content.ReadAsAsync<List<T>>();
            break;

          //use .Net 5 built-in deserializer
          case "ReadAsStreamAsync":
            contentStream = await Res.Content.ReadAsStreamAsync();
            theList = await JsonSerializer.DeserializeAsync<List<T>>(contentStream, _options);
            break;
        }
      }
      return theList;
    }
  }
}
