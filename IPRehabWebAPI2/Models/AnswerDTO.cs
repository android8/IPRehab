﻿using IPRehabModel;

namespace IPRehabWebAPI2.Models
{
  public partial class AnswerDTO
  {
    /// <summary>
    /// record key
    /// </summary>
    public int AnswerID { get; set; }

    public EpisodeOfCareDTO EpisodeOfCare { get; set; }

    public int QuestionIDFK { get; set; }

    //Initial, Interim, Discharge, Followup, Base, IPA 
    public int MeasureID { get; set; }
    public CodeSetDTO MeasureCodeSet { get; set; }
    public CodeSetDTO AnswerCodeSet { get; set; }
    public int? AnswerSequenceNumber { get; set; }
    public string Description { get; set; }
    public string ByUser { get; set; }
  }
}
