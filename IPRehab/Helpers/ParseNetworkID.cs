using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public static class ParseNetworkID
  {
    public static string CleanUserName(string networkID)
    {
      bool isZEROAccount = networkID.Substring(networkID.Length-1) == "0"; 
      if (isZEROAccount)
        networkID = networkID.Substring(0, networkID.Length - 1); //if ZERO account drop the last 0

      if (networkID.Contains('\\') || networkID.Contains("%2F") || networkID.Contains("//"))
      {
        String[] separator = { "\\", "%2F", "//" };
        var networkNameWithDomain = networkID.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        if (networkNameWithDomain.Length > 0)
          networkID = networkNameWithDomain[1];
        else
          networkID = networkNameWithDomain[0];
      }
      return networkID;
    }

  }
}
