﻿using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
    public static class SerializationGeneric<T> where T : class
    {
        public static HttpResponseMessage Res { get; set; }

        public static async Task<T> DeserializeAsync(string url, JsonSerializerOptions serializerOptions)
        {
            string httpMsgContentReadMethod = "ReadAsStreamAsync";
            T theList = null;

            Res = await APIAgent.GetDataAsync(new Uri(url));

            if (Res.IsSuccessStatusCode)
            {
                switch (httpMsgContentReadMethod)
                {
                    case "ReadAsAsync":
                        theList = await Res.Content.ReadAsAsync<T>();
                        break;

                    //use .Net 5 built-in deserializer
                    case "ReadAsStreamAsync":
                        Stream contentStream = await Res.Content.ReadAsStreamAsync();
                        theList = await JsonSerializer.DeserializeAsync<T>(contentStream, serializerOptions);
                        
                        break;
                }
            }

            return theList;
        }

    public static async Task<Stream> SerializeAsync(T typedObject)
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, typedObject, typedObject.GetType());
        return stream;
    }

    public static async Task<T> DeserializeAsync(Stream stream, JsonSerializerOptions deserializerOptions)
    {
        return await JsonSerializer.DeserializeAsync<T>(stream, deserializerOptions);
    }
}
}
