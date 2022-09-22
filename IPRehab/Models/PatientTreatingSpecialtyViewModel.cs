using IPRehabWebAPI2.Models;
using System.Collections.Generic;

namespace IPRehab.Models
{
  public class PatientTreatingSpecialtyViewModel
    {
    public PatientDTOTreatingSpecialty Patient { get; set; }
    public List<PatientEpisodeAndCommandVM> EpisodeBtnConfig { get; set; }

    public PatientTreatingSpecialtyViewModel()
    {
      Patient = new();
      EpisodeBtnConfig = new();
    }
  }
}