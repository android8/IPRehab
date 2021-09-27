using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class MDSubmitButtonViewModel
  {
    public List<UserAnswer> OldAnswers { get; set; }
    public List<UserAnswer> NewAnswers { get; set; }
  }

  public class UserAnswer
  {
    public int AnswerID { get; set; }
    public int EpisodeID { get; set; }
    public int QuestionID { get; set; }
    public int StageID { get; set; }
    public int AnswerCodeSetID { get; set; }
    public int AnswerSequenceNumber { get; set; }

    //optional but required for text (date, ICD), number (therapy Hours), or text area type
    public string Description { get; set; }

    public int AnswerByUserID { get; set; }
    public DateTime LastUpdate { get; set; }
  }

}
