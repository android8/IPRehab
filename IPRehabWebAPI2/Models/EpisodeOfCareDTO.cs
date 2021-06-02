using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  public partial class EpisodeOfCareDTO
  {
    [DisplayName("Episode of Care ID")]
    public int EpisodeOfCareId { get; set; }

    [DisplayName("Onset Date")]
    public DateTime OnsetDate { get; set; }

    [DisplayName("Admission Date")]
    public DateTime AdmissionDate { get; set; }

    [DisplayName("Patient ICN")]
    public string PatientIcnfk { get; set; }
  }
}
