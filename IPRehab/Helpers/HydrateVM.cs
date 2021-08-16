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
      QuestionWithSelectItems qws = new QuestionWithSelectItems();
      qws.Form = dto.Form;
      qws.Section = GetSectionKey(dto);
      qws.Required = dto.Required;
      qws.QuestionID = dto.QuestionID;
      qws.QuestionKey = dto.QuestionKey;
      qws.QuestionTitle = dto.QuestionTitle;
      qws.Question = dto.Question;
      qws.GroupTitle = string.IsNullOrEmpty(dto.GroupTitle) ? string.Empty: 
        Regex.IsMatch(dto.GroupTitle, @"^\d")? dto.GroupTitle.Remove(0,2): dto.GroupTitle;
      qws.AnswerCodeSetID = dto.AnswerCodeSetID;
      qws.AnswerCodeCategory = dto.AnswerCodeCategory;
      qws.ChoiceList = SetSelectedChoice(dto.ChoiceList, dto.Answers, dto.AnswerCodeCategory);
      qws.Instructions = dto.QuestionInsructions;
      return qws;
    }

    public static List<SectionInfo> GetQuestionSections(List<QuestionWithSelectItems> questions)
    {
      var sections = questions.Select(x => $"{x.QuestionTitle} {x.Section}").AsParallel().Distinct()
        .Select(x => new SectionInfo {
          SectionName = x,
          SectionKey = GetLastKeyWord(x),
        }).OrderBy(x=>x.SectionName).ThenBy(x=>x.SectionKey).ToList();
      return sections;
    }

    private static string GetLastKeyWord(string sectionKey) {
      string[] keys = sectionKey.Split(' ');
      string key = keys[keys.Length - 1];
      return key;
    }

    private static string GetSectionKey(QuestionDTO question)
    {
      string sectionKey = string.Empty;
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
      List<SelectListItem> selectedChoices = new List<SelectListItem>();
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

        SelectListItem thisChiceItem = new SelectListItem { 
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
          SelectListItem thisChiceItem = new SelectListItem { 
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
      DateTime aDate;
      if(DateTime.TryParse(text, out aDate))
      {
        text = aDate.ToString("yyyy-MM-dd"); /* HTML 5 browser date input must be in this format */
      }
      return text;
    }
  }
}
