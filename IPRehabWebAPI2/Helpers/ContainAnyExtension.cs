using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Helpers
{
  public static class ContainAnyExtension
  {
    public static bool ContainsAny(this string str, params string[] values)
    {
      if (!string.IsNullOrEmpty(str) || values.Length > 0)
      {
        foreach (string value in values)
        {
          if (str.Contains(value))
            return true;
        }
      }

      return false;
    }
  }
}
