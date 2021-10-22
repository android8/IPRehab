using System.Collections.Generic;

namespace IPRehab.Models
{
  public class InputViewComponentTemplateModel
  {
    public List<ChoiceAndAnswer> ChoiceAndAnswerList {get;set;}
    public InputViewComponenViewModel ViewComponentViewModel {get;set;}

    public InputViewComponentTemplateModel()
    {
      ChoiceAndAnswerList = new();
      ViewComponentViewModel = new();
    }
  }
}
