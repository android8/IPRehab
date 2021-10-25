using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class RehabActionViewModel : ICloneable
  {
    public string HostingPage { get; set; }
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
    public bool EnableThisPatient { get; set; }

    public RehabActionViewModel()
    {
      ControllerName = "Question";
      ActionName = "Edit";
    }

    public object Clone()
    {
      return new RehabActionViewModel()
      {
        HostingPage = this.HostingPage,
        ControllerName = this.ControllerName,
        ActionName = this.ActionName,
        PatientID = this.PatientID,
        SearchCriteria = this.SearchCriteria,
        OrderBy = this.OrderBy,
        PageNumber = this.PageNumber,
        EpisodeID = this.EpisodeID,
        EnableThisPatient = this.EnableThisPatient
      };
    }

    public override string ToString()
    {
      return $"HostingPage = {this.HostingPage}, ControllerName = {this.ControllerName},ActionName = {this.ActionName}, PatientID = {this.PatientID}, SearchCriteria = {this.SearchCriteria}, OrderBy = {this.OrderBy}, PageNumber = {this.PageNumber}, EpisodeID = {this.EpisodeID}, EnableThisPatient = {this.EnableThisPatient}";
    }
  }
}
