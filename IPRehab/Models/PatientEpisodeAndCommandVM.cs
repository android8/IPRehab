using IPRehabWebAPI2.Models;

namespace IPRehab.Models
{
  public class PatientEpisodeAndCommandVM: EpisodeOfCareDTO
  {
    public RehabActionViewModel ActionButtonVM { get; set; }

    public PatientEpisodeAndCommandVM()
    {
      ActionButtonVM = new();
    }
  }
}
