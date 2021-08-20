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
    public static QuestionWithSelectItems Hydrate(QuestionDTO dto)
    {
      QuestionWithSelectItems qws = new()
      {
        Form = dto.Form,
        Section = GetSectionKey(dto),
        Required = dto.Required,
        QuestionID = dto.QuestionID,
        QuestionKey = dto.QuestionKey,
        SectionTitle = dto.QuestionSection,
        Question = dto.Question,
        StageTitle = string.IsNullOrEmpty(dto.GroupTitle) ? string.Empty :
        Regex.IsMatch(dto.GroupTitle, @"^\d") ? dto.GroupTitle.Remove(0, 3) : dto.GroupTitle,
        AnswerCodeSetID = dto.AnswerCodeSetID,
        AnswerCodeCategory = dto.AnswerCodeCategory,
        ChoiceList = SetSelectedChoice(dto.ChoiceList, dto.Answers, dto.AnswerCodeCategory),
        Instructions = dto.QuestionInsructions
      };
      return qws;
    }

    public static List<SectionInfo> GetDistinctSections(List<QuestionWithSelectItems> questions)
    {
      var sections = questions.Select(x => $"{x.SectionTitle} {x.Section}").AsParallel().Distinct()
        .Select(x => new SectionInfo {
          SectionName = x,
          SectionKey = GetLastKeyWord(x),
        }).OrderBy(x=>x.SectionName).ThenBy(x=>x.SectionKey).ToList();
      return sections;
    }

    private static string GetLastKeyWord(string sectionKey) {
      string[] keys = sectionKey.Split(' ');
      string key = keys[^1]; //new C# 8 member access operator last index
      return key;
    }

    private static string GetSectionKey(QuestionDTO question)
    {
      if (question.QuestionKey.StartsWith("Q"))
        return "(Q)";
      if (question.QuestionKey == "AssessmentCompleted")
        return "(Complete)";
      if (question.QuestionKey == "A1005" || question.QuestionKey == "A1010")
        return "(A10*)";
      if (question.QuestionKey.StartsWith("GG") || question.QuestionKey.StartsWith("BB"))
        return $"({question.QuestionKey.Substring(0, 6)})" ;
      else
      {
        return $"({question.QuestionKey.Substring(0, 5).TrimEnd()})";
      }
    }

    private static List<SelectListItem> SetSelectedChoice(List<CodeSetDTO> validChoices, List<AnswerDTO> answers, string answerCodeCategory)
    {
      List<SelectListItem> selectedChoices = new();
      string text = string.Empty;

      if (!validChoices.Any() && answers.Any())
      {
        /* questions with Y/N, check, or free text answer types will have empty validChoices parameter */
        AnswerDTO thisAnswer = answers.First(); /* so just make the the SelectListItem out of the answer if any */
        if (answerCodeCategory == "Date")
        {
          text = ParseString(thisAnswer.Description); 
        }
        else
        {
          text = thisAnswer.Description;
        }

        SelectListItem thisChiceItem = new()
        { 
          Text = text, 
          Value = thisAnswer.AnswerCodeSet.CodeSetID.ToString(), 
          Selected = true };
        selectedChoices.Add(thisChiceItem);

        return selectedChoices;
      }
      else
      {
        foreach (var c in validChoices)
        {
          if (answerCodeCategory == "Date")
          {
            text = ParseString(c.CodeDescription);
          }
          else
          {
            text = c.CodeDescription;
          }
          var isThisChoice = answers.Any(a => a.AnswerCodeSet.CodeSetID == c.CodeSetID);
          SelectListItem thisChiceItem = new () { 
            Text = text, 
            Value = c.CodeSetID.ToString(), 
            Selected = isThisChoice 
          };
          selectedChoices.Add(thisChiceItem);
        }
        return selectedChoices;
      }
    }

    private static string ParseString(string thisString)
    {
      string text = string.Empty;
      char[] parsers = { '/', ' ', '-' };
      string[] dateParts = thisString.Split(parsers);
      for (int i = 0; i < 3; i++)
      {
        text += $"{dateParts[i]}";
        if (i < 2)
          text += "/";
      }
      if(DateTime.TryParse(text, out DateTime aDate))
      {
        text = aDate.ToString("yyyy-MM-dd"); /* HTML 5 browser date input must be in this format */
      }
      return text;
    }
  }
}
