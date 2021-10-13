using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class RehabActionViewModel
  {
    public string HostContainer { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public int EpisodeID { get; set; }
    /// <summary>
    /// encryption should be used, without it and protect PHI/PII, set the value when no existing episode for this patient
    /// </summary>
    public string PatientID { get; set; }
    //public string PatientName { get; set; }
    public string SearchCriteria { get; set; }
    public int PageNumber { get; set; }
    public string OrderBy { get; set; }
    public bool enableThisPatient { get; set; }

    public RehabActionViewModel() {
      ControllerName = "Question";
      ActionName = "Edit";
    }
  }
}
