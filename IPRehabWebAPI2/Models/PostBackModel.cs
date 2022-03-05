using System;
using System.Collections.Generic;

namespace IPRehabWebAPI2.Models
{
  public class PostbackModel
  {
    public int EpisodeID { get; set; }
    public List<UserAnswer> OldAnswers { get; set; }
    public List<UserAnswer> NewAnswers { get; set; }
    public List<UserAnswer> UpdatedAnswers { get; set; }
  }

  public class UserAnswer
  {
    /// <summary>
    /// not peristable, inspection only
    /// </summary>
    public string PatientName { get; set; }

    /// <summary>
    /// peristable, inspection only
    /// </summary>
    public string PatientID { get; set; }

    /// <summary>
    /// peristable
    /// </summary>
    public int AnswerID { get; set; }

    /// <summary>
    /// peristable, foreign key to episode identifiers 
    /// </summary>
    public int EpisodeID { get; set; }

    /// <summary>
    /// peristable, episode onset date
    /// </summary>
    public DateTime OnsetDate { get; set; }

    /// <summary>
    /// peristable, e[ospde admission date
    /// </summary>
    public DateTime AdmissionDate { get; set; }
    
    /// <summary>
    /// peristable, foreign key to question identifier 
    /// </summary>
    public int QuestionID { get; set; }

    /// <summary>
    /// not peristable, for inspection only
    /// </summary>
    public string QuestionKey { get; set; }

    /// <summary>
    /// peristable, foreign key to stage identifier for inspection only
    /// </summary>
    public int MeasureID { get; set; }

    /// <summary>
    /// not peristable, for inspection only
    /// </summary>
    public string MeasureName { get; set; }

    /// <summary>
    /// peristable, foreign key to codeset id  
    /// </summary>
    public int AnswerCodeSetID { get; set; }

    /// <summary>
    /// not peristable, for inspection only to ensure the the AnswerCodeSet is correct  
    /// </summary>
    public string AnswerCodeSetDescription { get; set; }

    /// <summary>
    /// peristable, unique identifier for multi-answer questions
    /// </summary>
    public int AnswerSequenceNumber { get; set; }

    /// <summary>
    /// persistable, only required for text (date, ICD), number (therapy Hours, ), or text area type
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// persistable, user network ID
    /// </summary>
    public string AnswerByUserID { get; set; }

    /// <summary>
    /// persistable, date of the answer
    /// </summary>
    public DateTime LastUpdate { get; set; }
  }
}
