using IPRehabModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  public class QuestionDTO
  {
    public string FormName { get; set; }
    public int StageID { get; set; }
    public int QuestionID { get; set; }
    public bool? Required { get; set; }
    public string QuestionKey { get; set; }
    public string QuestionSection { get; set; }
    public string Question { get; set; }
    public int MeasureID { get; set; }
    public int MeasureCodeSetID { get; set; }
    public string MeasureCodeValue { get; set; }
    public string MeasureDescription { get; set; }
    public int AnswerCodeSetID { get; set; }
    public string AnswerCodeCategory { get; set; }
    public int? DisplayOrder { get; set; }
    /// <summary>
    /// set the Enabled property to false, if this question is dependent on another question's answers and if other question(s) determines to false 
    /// set it to true, if this question is not dependent on another question's answers 
    /// </summary>
    public bool Enabled { get; set; }
    public bool MultipleChoices { get; set; }
    public List<CodeSetDTO> ChoiceList { get; set; }
    public List<QuestionInstructionDTO> QuestionInsructions { get; set; }

    /// <summary>
    /// Each question has primarily a single answer but can be multiple such as ICD, Interrupt Dates
    /// </summary>
    public List<AnswerDTO> Answers { get; set; }

    public QuestionDTO() {
      ChoiceList = new List<CodeSetDTO>();
      QuestionInsructions = new List<QuestionInstructionDTO>();
      Answers = new List<AnswerDTO>();
    }
  }
}
