using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class PatientListViewModel
  {
    public List<PatientViewModel> Patients { get; set; }
    public int TotalPatients { get; set; }
    public string SearchCriteria { get; set; }

    public PatientListViewModel() {
      Patients = new();
    }
  }
}
