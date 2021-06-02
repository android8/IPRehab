using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  //[ModelBinder(typeof(PatientSearchModelBinder))]
  public class PatientSearchModel
  {
    [DisplayName("Rehab Stage")]
    public RehabStageEnum Stage { get; set; }

    [DisplayName("Patient SSN")]
    public String SSN { get; set; }
  }
}
