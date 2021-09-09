using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public static class SerializationGeneric<T> where T : class
  {
    public static async Task<T> SerializeAsync(string url, JsonSerializerOptions _options)
    {
      string httpMsgContentReadMethod = "ReadAsStreamAsync";
      System.IO.Stream contentStream = null;
      T theList = null;
      HttpResponseMessage Res;
      Res = await APIAgent.GetDataAsync(new Uri(url));

      if (Res == null)
        return null;
      else
      {
        if (Res.Content is object)
        {
          switch (Res.Content.Headers.ContentType.MediaType)
          {
            case "application/json":
              {
                switch (httpMsgContentReadMethod)
                {
                  case "ReadAsAsync":
                    theList = await Res.Content.ReadAsAsync<T>();
                    break;

                  //use .Net 5 built-in deserializer
                  case "ReadAsStreamAsync":
                    contentStream = await Res.Content.ReadAsStreamAsync();
                    theList = await JsonSerializer.DeserializeAsync<T>(contentStream, _options);
                    break;
                }
                break;
              }
          }
        }
        return theList;
      }
    }

    public static async Task<Stream> SerializeAsync(T typedObject) {
      string json = string.Empty;
      using (var stream = new MemoryStream())
      {
        await JsonSerializer.SerializeAsync(stream, typedObject, typedObject.GetType());
        return stream;
      }
    }

    public static async Task<T> DeserializeAsync(Stream stream, JsonSerializerOptions _options)
    {
      return await JsonSerializer.DeserializeAsync<T>(stream, _options);
    }
  }
}
