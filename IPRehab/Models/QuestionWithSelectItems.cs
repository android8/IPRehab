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
        /// <summary>
        /// set ReadOnly to true if this question is not editable such as Q12.
        /// </summary>
        public bool? ReadOnly { get; set; }
        public bool? Required { get; set; }
        public bool? KeyQuestion { get; set; }
        public string QuestionKey { get; set; }
        public string SectionTitle { get; set; }
        public string Question { get; set; }
        public string MeasureDescription { get; set; }
        public int MeasureID { get; set; }
        public int StageID { get; set; }
        public bool MultipleChoices { get; set; }
        public int AnswerCodeSetParentID { get; set; }
        public int AnswerCodeSetID { get; set; }
        public int DisplayLocation { get; set; }
        public int? DisplayOrder { get; set; }
        public List<QuestionInstructionDTO> Instructions { get; set; }
        public List<SelectListItem> ChoiceList { get; set; }
        public List<ChoiceAndAnswer> ChoicesAnswers { get; set; }
        public string AnswerCodeCategory { get; set; }

        public QuestionWithSelectItems()
        {
            Instructions = new();
            ChoiceList = new();
            ChoicesAnswers = new();
        }
    }
}
