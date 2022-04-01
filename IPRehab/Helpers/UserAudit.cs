using IPRehab.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public static class UserAudit
  {
    /// <summary>
    /// send audit data to Centurion
    /// </summary>
    public static async Task AuditUserAsync(IConfiguration configuration, string user, RouteData routeData, IPAddress remoteIPAddress)
    {
      //Generate an audit
      UserAuditModel audit = new()
      {
        UserName = user,
        IPAddress = remoteIPAddress.ToString(),
        Application = "Inpatient Rehab Assessment",  //Enter your Product Name
        Controller = routeData.Values["controller"].ToString(),
        Action = routeData.Values["action"].ToString(),
        Context = "VSSC Web Hits",
        DateAccessed = DateTime.UtcNow,
        ProductID = 6480  //Enter your ProductID
      };

      string baseUrl = configuration.GetSection("AppSettings").GetValue<string>("WebStatsAPIUrl");
      bool webStatsEnabled = configuration.GetSection("AppSettings").GetValue<string>("WebStatsEnabled").ToLower() == "true";
      string strApiCall = "Create/";
      string url = $"{baseUrl}/{strApiCall}";

      if (webStatsEnabled)
      {
        using var client = new HttpClient();
        var useraudit = JsonConvert.SerializeObject(audit);
        StringContent content = new(useraudit, Encoding.UTF8, "application/json");
        using var response = await client.PostAsync(url, content);
        var results = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
      }
    }
  }
}
