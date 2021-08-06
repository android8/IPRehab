using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IPRehabWebAPI2.Models
{
  public partial class EpisodeOfCareDTO
  {
    [DisplayName("Episode")]
    public int EpisodeOfCareID { get; set; }

    [DisplayFormat(DataFormatString = "{0:d}")]
    [DisplayName("Onset Date")]
    public DateTime OnsetDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:d}")]
    [DisplayName("Admission Date")]
    public DateTime AdmissionDate { get; set; }

    [DisplayName("Patient ICN")]
    public string PatientIcnFK { get; set; }
  }
}
