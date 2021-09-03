using IPRehabModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public static class APIAgent
  {
    public static async Task<HttpResponseMessage> GetDataAsync(Uri uri)
    {
      using var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
      client.DefaultRequestHeaders.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

      var httpResponseMsg = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

      //httpResponseMsg.EnsureSuccessStatusCode();
      return httpResponseMsg;
    }

    public static async Task<IEnumerable<tblQuestion>> ReadAsAsyncWithSystemTextJson(Uri uri, JsonSerializerOptions options, string contentReadMethod)
    {
      using var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
      client.DefaultRequestHeaders.Clear();
      //Define request data format  
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      using var httpResponse = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

      httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299

      switch (contentReadMethod)
      {
        case "ReadAsAsync":
          return await httpResponse.Content.ReadAsAsync<IEnumerable<tblQuestion>>();
        case "ReadAsStreamAsync":
          var contentStream = await httpResponse.Content.ReadAsStreamAsync();
          return await JsonSerializer.DeserializeAsync<IEnumerable<tblQuestion>>(contentStream, options);
      }

      return null;
    }

    public static async Task<IEnumerable<tblQuestion>> StreamWithSystemTextJson(Uri uri, JsonSerializerOptions options)
    {
      using var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
      client.DefaultRequestHeaders.Clear();
      //Define request data format  
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      using var httpResponse = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

      httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299

      if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
      {
        var contentStream = await httpResponse.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<IEnumerable<tblQuestion>>(contentStream, options);
      }
      else
      {
        Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
      }

      return null;
    }
  }
}
