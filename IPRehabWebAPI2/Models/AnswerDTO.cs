using IPRehabModel;

namespace IPRehabWebAPI2.Models
{
  public partial class AnswerDTO
  {
    public EpisodeOfCareDTO EpisodeOfCare { get; set; }
    public int QuestionIdFK { get; set; }

    //Initial, Interim, Discharge, Followup, Base, IPA 
    public string CareStage { get; set; }

    public TblCodeSet AnswerCodeSet { get; set; }
    public int AnswerSequenceNumber { get; set; }
    public string Description { get; set; }
    public string ByUser { get; set; }
  }
}
