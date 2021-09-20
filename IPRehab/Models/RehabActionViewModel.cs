using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class RehabActionViewModel
  {
    public string HostContainer { get; set; }
    public int EpisodeID { get; set; }
    public string PatientID { get; set; }
    public string PatientName { get; set; }
  }
}
