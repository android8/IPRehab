using System.Collections.Generic;

namespace IPRehab.Models
{
  public class QuestionGroup
  {
    /// <summary>
    /// Each group of question will only show the text once
    /// </summary>
    public string SharedQuestionText { get; set; }

    public string SharedQuestionInstruction { get; set; }

    public string SharedQuestionKey { get; set; }

    public string InputType { get; set; }
    /// <summary>
    /// questions in the group have one shared question text, but may have many form input controls model bound to QuestionWithSelectItems
    /// </summary>
    public List<QuestionWithSelectItems> Questions { get; set; }

    public QuestionGroup()
    {
      Questions = new();
    }
  }
}
