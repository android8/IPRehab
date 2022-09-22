using IPRehabModel;
using IPRehabWebAPI2.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
    public static class APIAgent
    {
        public static Task<HttpResponseMessage> GetDataAsync(Uri uri)
        {
            using var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromMinutes(5);

            var httpResponseMsg = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;

            //If you prefer to treat HTTP error codes as exceptions, call HttpResponseMessage.EnsureSuccessStatusCode on the response object.
            httpResponseMsg.EnsureSuccessStatusCode();

            return Task.FromResult(httpResponseMsg);
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
            using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
            {
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using var httpResponse = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;

                httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299

                if (httpResponse.Content?.Headers.ContentType.MediaType == "application/json")
                {
                    var contentStream = await httpResponse.Content.ReadAsStreamAsync();
                    return await JsonSerializer.DeserializeAsync<IEnumerable<tblQuestion>>(contentStream, options);
                }
                else
                {
                    Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
                }
            }
            return null;
        }

        public static async Task<HttpResponseMessage> PostDataAsync(Uri uri, PostbackModel postbackModel)
        {
            //method 1
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(uri, postbackModel);
            response.EnsureSuccessStatusCode();
            return response;

            //method 2
            //using (var client = new HttpClient())
            //using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
            //{
            //  var json = JsonSerializer.Serialize(postbackModel);
            //  using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
            //  {
            //    request.Content = stringContent;
            //    using (var response = await client
            //        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            //        .ConfigureAwait(false))
            //    {
            //      response.EnsureSuccessStatusCode();
            //      return response;
            //    }
            //  }
            //}
        }
    }
}
