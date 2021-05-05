using IPRehabModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
   public static class APIAgent
   {
      public static async Task<HttpResponseMessage> GetDataAsync(string baseUrl, string endpoint)
      {
         using var client = new HttpClient();
         client.DefaultRequestHeaders.Clear();
         //Define request data format  
         client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
         using var httpResponseMsg = await client.GetAsync(new Uri(uriString: $"{baseUrl}{endpoint}"), HttpCompletionOption.ResponseHeadersRead);
         httpResponseMsg.EnsureSuccessStatusCode();
         return httpResponseMsg;
      }

      public static async Task<IEnumerable<TblQuestion>> StreamWithSystemTextJson(Uri uri, JsonSerializerOptions options)
      {
         using var client = new HttpClient();
         client.DefaultRequestHeaders.Clear();
         //Define request data format  
         client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         using var httpResponse = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

         httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299

         if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
         {
            //var contentStream = await httpResponse.Content.ReadAsStreamAsync();
            try
            {
               //return await JsonSerializer.DeserializeAsync<IEnumerable<TblQuestion>>(contentStream, options);
               return await httpResponse.Content.ReadAsAsync<IEnumerable<TblQuestion>>();
            }
            catch (JsonException ex) // Invalid JSON
            {
               Console.WriteLine("Invalid JSON.");
               Console.WriteLine(ex.Message);
            }
         }
         else
         {
            Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
         }

         return null;
      }
   }
}
