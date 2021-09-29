using System;
using System.Collections.Generic;

namespace IPRehabWebAPI2.Models
{
  public class PostbackModel
  {
    public List<UserAnswer> OldAnswers { get; set; }
    public List<UserAnswer> NewAnswers { get; set; }
    public List<UserAnswer> UpdatedAnswers { get; set; }
  }

  public class UserAnswer
  {
    public string PatientName { get; set; }
    public int AnswerID { get; set; }
    public int EpisodeID { get; set; }
    public int QuestionID { get; set; }
    public string QuestionKey { get; set; }
    public int StageID { get; set; }
    public string StageName { get; set; }
    public int AnswerCodeSetID { get; set; }
    public string AnswerCodeSetDescription { get; set; }
    public int AnswerSequenceNumber { get; set; }

    /// <summary>
    /// optional but required for text (date, ICD), number (therapy Hours), or text area type
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// user network ID
    /// </summary>
    public string AnswerByUserID { get; set; }

    public DateTime LastUpdate { get; set; }
  }
}
