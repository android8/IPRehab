using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class PatientListViewModel
  {
    public string PageTitle { get; set; }
    public List<PatientViewModel> Patients { get; set; }
    public int TotalPatients { get; set; }
    public string SearchCriteria { get; set; }
    public int PageNumber { get; set; }
    public string OrderBy { get; set; }
    
    public PatientListViewModel() {
      Patients = new();
    }
  }
}
