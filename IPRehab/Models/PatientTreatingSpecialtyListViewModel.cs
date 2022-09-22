using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class PatientTreatingSpecialtyListViewModel
    {
    public string PageTitle { get; set; }
    public string PageSysTitle { get; set; }
    public List<PatientTreatingSpecialtyViewModel> Patients { get; set; }
    public int TotalPatients { get; set; }
    public string SearchCriteria { get; set; }
    public int PageNumber { get; set; }
    public string OrderBy { get; set; }
    
    public PatientTreatingSpecialtyListViewModel() {
      Patients = new();
    }
  }
}
