using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class SectionInfo
  {
    public int SectionID { get; set; }
    public string SectionName { get; set; }
    public string SectionKey { get; set; }
    public bool HasAnswer { get; set; }
  }
}
