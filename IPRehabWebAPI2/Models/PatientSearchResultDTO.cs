using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  public class PatientSearchResultDTO
  {
    public int TotalCount {get;set;}
    public List<PatientDTO> Patients { get; set; }
  }
}
