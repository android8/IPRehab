using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
    public static class SerializeToByteArrayExtension
    {
        public static byte[] SerializeToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var byteArrayOfThisObj = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            return byteArrayOfThisObj;
        }

        public static async System.Threading.Tasks.Task<T> DeserializeAsync<T>(this byte[] byteArray, JsonSerializerOptions serializerOptions) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            using (var memStream = new MemoryStream())
            {
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(memStream, serializerOptions);
                return obj;
            }
        }
    }
}
