using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IPRehab.Helpers
{
  public class HydrateVM
  {
    public static QuestionWithSelectItems Hydrate(QuestionDTO questionDto)
    {
      QuestionWithSelectItems qws = new()
      {
        Form = questionDto.FormName,

        Section = GetSection(questionDto),

        Required = questionDto.Required,
        QuestionID = questionDto.QuestionID,

        /* turn on key question */
        KeyQuestion = questionDto.QuestionKey == "Q12" || questionDto.QuestionKey == "Q23",

        /* do not show key for AssessmentCompleted */
        QuestionKey = questionDto.QuestionKey,

        SectionTitle = questionDto.QuestionSection,
        Question = questionDto.Question,

        StageID = questionDto.StageID,

        AnswerCodeSetID = questionDto.AnswerCodeSetID,
        AnswerCodeCategory = questionDto.AnswerCodeCategory,
        MultipleChoices = questionDto.MultipleChoices,

        ChoiceList = SetSelectedChoice(questionDto),

        ChoicesAnswers = SetChoicesAnswers(questionDto),

        Instructions = questionDto.QuestionInsructions,
        MeasureDescription = string.IsNullOrEmpty(questionDto.MeasureDescription) ?
          string.Empty : Regex.IsMatch(questionDto.MeasureDescription, @"^\d") ? questionDto.MeasureDescription.Remove(0, 3) : questionDto.MeasureDescription,
        MeasureID = questionDto.MeasureID
      };

      return qws;
    }

    public static QuestionHierarchy HydrateHierarchically(List<QuestionDTO> questions)
    {
      QuestionHierarchy qh = new();

      List<QuestionWithSelectItems> qwsList = new();
      foreach (var questionDto in questions)
      {
        QuestionWithSelectItems qws = Hydrate(questionDto);
        qwsList.Add(qws);
      }

      /* ToDo: find questionDTO.QuestionID in ToQuestionID of branching point list 
        then check if the FromQuestionID conatins any value. If no value, disable this questionDTO
       */

      var distinctSections = GetDistinctSections2(qwsList);

      /* Sections */
      foreach (SectionInfo thisSection in distinctSections)
      {
        List<QuestionWithSelectItems> questionInTheSection = qwsList.Where(q => q.SectionTitle == thisSection.SectionTitle).ToList();
        
        QuestionWithSelectItems questionWithHeaderInstruction   = questionInTheSection.Where(q => q.Instructions != null && q.Instructions.Any(i => i.DisplayLocation == "SectionHeader")).FirstOrDefault();

        if (questionWithHeaderInstruction != null)
        {
          foreach (var ins in questionWithHeaderInstruction.Instructions)
          {
            thisSection.SectionInstruction += $" {ins.Instruction}";
          }
          thisSection.SectionInstruction = thisSection.SectionInstruction.Trim();
        }

        QuestionWithSelectItems questionWithAggregateInstruction = questionInTheSection.Where(q => q.Instructions != null && q.Instructions.Any(i => i.DisplayLocation == "SectionFooter")).FirstOrDefault();

        if (questionWithAggregateInstruction != null)
        {
          foreach (var ins in questionWithAggregateInstruction.Instructions)
          {
            thisSection.AggregateInstruction += $" {ins.Instruction}";
          }
          thisSection.AggregateInstruction = thisSection.AggregateInstruction.Trim();

          thisSection.AggregateAfterQuestionKey = questionWithAggregateInstruction.QuestionKey;
          switch (thisSection.SectionTitle)
          {
            case "Mobility (3-day assessment period)":
              thisSection.AggregateType = "Mobility Aggregate Score";
              break;
            case "Self-Care (3-day assessment period)":
              thisSection.AggregateType = "Self Care Aggregate Score";
              break;
          }
        }

        /* Question Groups */
        /* GG0170SS is sub question of GG0170S */
        var qGG0170SS = qwsList.Where(q => q.QuestionKey == "GG0170SS");

        List<string> distinctQuestions = questionInTheSection.Select(x => x.Question).Distinct().ToList();
        foreach (string thisQuestionText in distinctQuestions)
        {
          QuestionGroup questionGroup = new();
          List<QuestionWithSelectItems> questionInTheGroup = new();
          questionGroup.SharedQuestionText = thisQuestionText;

          switch (thisQuestionText)
          {
            case "Indicate the type of wheelchair or scooter used.":
              if (qGG0170SS != null)
              {
                questionInTheGroup = questionInTheSection.Where(q => q.Question == thisQuestionText)
                  .Except(qGG0170SS)
                  .OrderBy(q => q.QuestionKey).ToList();
              }
              else
              {
                questionInTheGroup = questionInTheSection.Where(q => q.Question == thisQuestionText)
                  .OrderBy(q => q.QuestionKey).ToList();
              }
              break;
            default:
              questionInTheGroup = questionInTheSection.Where(q => q.Question == thisQuestionText)
                .OrderBy(q => q.QuestionKey).ToList();
              break;
          }

          var groupKey = questionInTheGroup.First().QuestionKey;
          questionGroup.SharedQuestionKey = groupKey;

          var groupInstruction = questionInTheGroup.Where(q => q.Instructions != null && q.Instructions.Any(i => i.DisplayLocation == "QuestionBody")).ToList();
          if (groupInstruction != null)
          {
            foreach (var q in groupInstruction)
            {
              foreach (var ins in q.Instructions)
              {
                questionGroup.SharedQuestionInstruction += ins.Instruction;
              }
            }
          }

          questionGroup.Questions = questionInTheGroup;

          thisSection.QuestionGroups.Add(questionGroup);

          /* add GG0170SS to the bottom of the section following GG0170S */
          if (thisSection.SectionTitle == "Mobility (3-day assessment period)" && questionGroup.SharedQuestionKey == "GG0170S")
          {
            QuestionGroup GG0170SSBreakoutGroup = new();
            GG0170SSBreakoutGroup.SharedQuestionText = qGG0170SS.FirstOrDefault().Question;
            GG0170SSBreakoutGroup.SharedQuestionKey = qGG0170SS.FirstOrDefault().QuestionKey;
            List<QuestionWithSelectItems> questionInBreakoutGroup = new();
            foreach (var item in qGG0170SS)
            {
              questionInBreakoutGroup.Add(item);
            }
            GG0170SSBreakoutGroup.Questions = questionInBreakoutGroup;

            groupInstruction = questionInBreakoutGroup.Where(q => q.Instructions != null && q.Instructions.Any(qi => qi.DisplayLocation == "QuestionBody")).ToList();
            if (groupInstruction != null)
            {
              foreach (var q in groupInstruction)
              {
                foreach (var ins in q.Instructions)
                {
                  GG0170SSBreakoutGroup.SharedQuestionInstruction += ins.Instruction;
                }
              }
            }

            thisSection.QuestionGroups.Add(GG0170SSBreakoutGroup);
          }
        }

        qh.Sections.Add(thisSection);
      }

      return qh;
    }

    public static List<SectionInfo> GetDistinctSections(List<QuestionWithSelectItems> questions)
    {
      var sections = questions.Select(x => $"{x.SectionTitle} {x.Section}").AsParallel().Distinct()
        .Select(x => new SectionInfo
        {
          SectionTitle = x,
          SectionKey = GetLastKeyWord(x),
        }).OrderBy(x => x.SectionTitle).ThenBy(x => x.SectionKey).ToList();
      return sections;
    }

    public static List<SectionInfo> GetDistinctSections2(List<QuestionWithSelectItems> questions)
    {
      var sections = questions.Select(x => new { x.SectionTitle, x.Section }).AsParallel().Distinct()
        .Select(x => new SectionInfo
        {
          SectionTitle = x.SectionTitle,
          SectionKey = x.Section,
        }).OrderBy(x => x.SectionKey.StartsWith("Q") ? x.SectionKey[..1] : x.SectionKey).ToList();

      var caseDetail = sections.Where(s => s.SectionTitle == "Case Detail");
      var complete = sections.Where(s => s.SectionTitle == "Complete");
      sections = sections.Except(caseDetail).Except(complete).ToList();

      if (caseDetail.Any())
      {
        /* hoist case detail section to the top of the list */
        sections.InsertRange(0, caseDetail);
      }
      if (complete.Any())
      {
        sections.Add(complete.First());
      }
      return sections;
    }

    private static string GetLastKeyWord(string sectionKey)
    {
      string[] keys = sectionKey.Split(' ');
      string key = keys[^1]; //new C# 8 member access operator last index
      return key;
    }

    private static string GetSection(QuestionDTO question)
    {
      if (question.QuestionKey.StartsWith("Q"))
        return "(Q)";
      if (question.QuestionKey == "AssessmentCompleted")
        return "(Complete)";
      if (question.QuestionKey == "A1005" || question.QuestionKey == "A1010")
        return "(A10*)";
      if (question.QuestionKey.StartsWith("GG") || question.QuestionKey.StartsWith("BB"))
        return $"({question.QuestionKey[..6]})";
      else
      {
        return $"({question.QuestionKey[..5]})";
      }
    }

    private static List<SelectListItem> SetSelectedChoice(QuestionDTO questionDTO)
    {
      List<SelectListItem> selectedChoices = new();
      string text = string.Empty;

      /* text(153), date(92), checkbox, textarea, and number have only one item in the choices list */
      foreach (var c in questionDTO.ChoiceList)
      {
        SelectListItem thisChiceItem = new()
        {
          Text = c.CodeDescription,
          Value = c.CodeSetID.ToString(),
          Selected = questionDTO.Answers.Any(a => a.AnswerCodeSet.CodeSetID == c.CodeSetID && a.MeasureCodeSet.CodeDescription == questionDTO.MeasureDescription)
        };
        selectedChoices.Add(thisChiceItem);
      }
      return selectedChoices;
    }

    private static List<ChoiceAndAnswer> SetChoicesAnswers(QuestionDTO questionDTO)
    {
      List<ChoiceAndAnswer> choiceAndAnswers = new();
      string text = string.Empty, value = string.Empty;

      if (questionDTO.ChoiceList.Count == 0)
      {
        var thisAnswer = questionDTO.Answers.SingleOrDefault(a => a.AnswerCodeSet.CodeSetID == questionDTO.AnswerCodeSetID);

        /* make choice list with only one codeset id */
        SelectListItem thisChice = new()
        {
          Text = questionDTO.AnswerCodeCategory,
          Value = questionDTO.AnswerCodeSetID.ToString(),
          Selected = (thisAnswer != null)
        };

        ChoiceAndAnswer thisChoiceAndAnswer = new()
        {
          SelectListItem = thisChice,
          Answer = thisAnswer
        };

        choiceAndAnswers.Add(thisChoiceAndAnswer);
      }
      else
      {
        foreach (var c in questionDTO.ChoiceList)
        {
          var thisAnswer = questionDTO.Answers.SingleOrDefault(a => a.AnswerCodeSet.CodeSetID == c.CodeSetID);

          SelectListItem thisChice = new()
          {
            Text = c.CodeDescription,
            Value = c.CodeSetID.ToString(),
            Selected = (thisAnswer != null)
          };

          ChoiceAndAnswer thisChoiceAndAnswer = new();
          thisChoiceAndAnswer.SelectListItem = thisChice;
          thisChoiceAndAnswer.Answer = thisAnswer;

          choiceAndAnswers.Add(thisChoiceAndAnswer);
        }
      }
      return choiceAndAnswers;
    }

    //private static string ParseDateString(string originalString)
    //{
    //  string text = string.Empty;
    //  char[] delimiter = { '/', ' ', '-' };
    //  string[] dateParts = originalString.Split(delimiter);
    //  for (int i = 0; i < 3; i++)
    //  {
    //    text += $"{dateParts[i]}";
    //    if (i < 2)
    //      text += "/";
    //  }
    //  if(DateTime.TryParse(text, out DateTime aDate))
    //  {
    //    text = aDate.ToString("yyyy-MM-dd"); /* HTML 5 browser date input must be in this format */
    //  }
    //  else
    //  {
    //    text = originalString;
    //  }
    //  return text;
    //}
  }
}
