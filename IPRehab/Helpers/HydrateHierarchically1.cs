using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public class HydrateHierarchically1
  {
    public static QuestionHierarchy HydrateHierarchically1(List<QuestionDTO> questions)
    {
      string currentStage = string.Empty, currentQuestion = string.Empty;
      string currentSection = "Case Detail";

      QuestionHierarchy qh = new();

      var sections = questions.Select(x => x.QuestionSection).AsParallel().Distinct().ToList();
      foreach (var thisSection in sections)
      {
        SectionInfo si = new();
        si.SectionName = thisSection;
        var questionInTheSection = questions.Where(x => x.QuestionSection == thisSection);
        si.SectionKey = GetSectionKey(questionInTheSection.First());

        string headerInstruction = string.Empty;
        var headerInstructionList = questionInTheSection.Where(q => q.QuestionInsructions.Where(i => i.DisplayLocation == "SectionHeader");
        foreach (var ins in headerInstructionList)
        {
          headerInstruction += string.Format("{0} ", ins.Instruction
          }
        thisSection.SectionInstruction = headerInstruction;
        QuestionGroup qg = new();
      }
      qh.SectionNames = sectionNamess.ToList();

      foreach (var q in questions)
      {
        if (currentStage != q.FormName)
        {
          currentStage = q.FormName;
          qh.StageName = q.FormName;
        }

        if (currentSection != q.QuestionSection)
        {
          currentSection = q.QuestionSection;

          List<SectionInfo> sectionInfo = new();
          SectionInfo thisSection = new();
          thisSection.SectionKey = q.QuestionSection;

          string headerInstruction = string.Empty;
          var headerInstructionList = q.QuestionInsructions.Where(i => i.DisplayLocation == "SectionHeader");
          foreach (var ins in headerInstructionList)
          {
            headerInstruction += string.Format("{0} ", ins.Instruction
          }
          thisSection.SectionInstruction = headerInstruction;
          sectionInfo.Add(thisSection);
        }

      }
      return null;
    }

  }
}
