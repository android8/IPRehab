using IPRehabWebAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class PatientViewModel
  {
    public PatientDTO Patient { get; set; }
    public RehabActionViewModel ActionButtonVM { get; set; }
  }
}
