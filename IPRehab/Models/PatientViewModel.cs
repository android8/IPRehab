using IPRehabWebAPI2.Models;
using System.Collections.Generic;

namespace IPRehab.Models
{
  public class PatientViewModel
  {
    public PatientDTO Patient { get; set; }
    public List<PatientEpisodeAndCommandVM> EpisodeBtnConfig { get; set; }

    public PatientViewModel()
    {
      Patient = new();
      EpisodeBtnConfig = new();
    }
  }
}