using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IPRehab.Models
{
  public class QuestionWithSelectItems
  {
    public string Form { get; set; }
    public string Section { get; set; }
    public int QuestionID { get; set; }
    public bool? Required { get; set; }
    public string QuestionKey { get; set; }
    public string SectionTitle { get; set; }
    public string Question { get; set; }
    public string StageTitle { get; set; }
    public int AnswerCodeSetParentID { get; set; }
    public int AnswerCodeSetID { get; set; }
    public int DisplayLocation { get; set; }
    public int? DisplayOrder { get; set; }
    public List<QuestionInstructionDTO> Instructions { get; set; }
    public List<SelectListItem> ChoiceList { get; set; }
    public string AnswerCodeCategory { get; set; }

    public QuestionWithSelectItems() {
      ChoiceList = new List<SelectListItem>();
      Instructions = new List<QuestionInstructionDTO>();
    }
  }
}
