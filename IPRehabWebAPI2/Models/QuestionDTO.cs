using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  public class QuestionDTO
  {
    public string Form { get; set; }
    public int QuestionID { get; set; }
    public string QuestionKey { get; set; }
    public string QuestionTitle { get; set; }
    public string Question { get; set; }
    public string GroupTitle { get; set; }
    public int AnswerCodeSetID { get; set; }
    public string AnswerCodeCategory { get; set; }
    public int? DisplayOrder { get; set; }
    public List<CodeSetDTO> ChoiceList { get; set; }
  }
}
