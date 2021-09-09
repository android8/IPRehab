using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IPRehab.Models
{
  public class ChoiceAndAnswer
  {
    public SelectListItem SelectListItem { get; set; }
    public AnswerDTO Answer{ get; set; }
  }
}