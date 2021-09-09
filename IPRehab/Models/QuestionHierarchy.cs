using IPRehabWebAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class QuestionHierarchy
  {
    public bool ReadOnly { get; set; }
    public string PatientName { get; set;}
    public RehabActionViewModel ActionButtons { get; set; }
    public string StageTitle { get; set; }
    public string CurrentAction { get; set; }
    public string ModeColorCssClass { get; set; }
    public List<SectionInfo> Sections { get; set; }

    public QuestionHierarchy()
    {
      Sections = new();
    }
  }
}
