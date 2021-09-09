using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class SectionInfo
  {
    public int SectionID { get; set; }
    public string SectionTitle { get; set; }
    public string SectionKey { get; set; }
    public int? DisplayOrder { get; set; }
    public string SectionInstruction { get; set; }
    public string AggregateInstruction { get; set; }
    public string AggregateAfterQuestionKey { get; set; }
    public string AggregateType { get; set; }
    public List<QuestionGroup> QuestionGroups { get; set; }

    public SectionInfo()
    {
      QuestionGroups = new();
    }
  }
}
