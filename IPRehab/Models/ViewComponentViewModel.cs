using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class ViewComponenViewModel
  {
    public int ControlCounter { get; set; }
    public string UserID { get; set; }
    public int QuestionID { get; set; }
    public string QuestionKey { get; set; }
    public string StageTitle { get; set; }
    public bool MultipleChoices { get; set; }
    public bool Required { get; set; }
    public bool DisplayStageHeader { get; set; }
    public string StageHeaderBorderCssClass { get; set; }
    public string ContainerCssClass { get; set; }
  }

  public class ViewComponentTemplateModel
  {
    public List<ChoiceAndAnswer> ChoiceAndAnswerList {get;set;}
    public ViewComponenViewModel ViewComponentViewModel {get;set;}

    public ViewComponentTemplateModel()
    {
      ChoiceAndAnswerList = new();
      ViewComponentViewModel = new();
    }
  }
}
