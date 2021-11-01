using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public static class NewtonSoftSerializationGeneric<T> where T : class
  {
    public static string Serialize(T typeObject) => JsonConvert.SerializeObject(typeObject);

    public static T Deserialize<T>(string jsonString) => JsonConvert.DeserializeObject<T>(jsonString);

    /// <summary>
    /// make webapi call through httpClient then deserialize the returned JSON string
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<T> DeserializeAsync(string url)
    {
      HttpResponseMessage Res = await APIAgent.GetDataAsync(new Uri(url));

      if (Res == null || Res.Content is not object)
        return null;
      else
      {
        return Deserialize<T>(await Res.Content.ReadAsStringAsync());
      }
    }

  }
}
