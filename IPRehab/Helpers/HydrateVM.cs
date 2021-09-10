﻿using IPRehab.Models;
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
    public static QuestionWithSelectItems Hydrate(QuestionDTO questionDTO)
    {
      QuestionWithSelectItems qws = new()
      {
        Form = questionDTO.FormName,
        
        Section = GetSection(questionDTO),

        Required = questionDTO.Required,
        QuestionID = questionDTO.QuestionID,
        KeyQuestion = questionDTO.QuestionKey == "Q12" || questionDTO.QuestionKey == "Q23",
        QuestionKey = questionDTO.QuestionKey,
        SectionTitle = questionDTO.QuestionSection,
        Question = questionDTO.Question,

        StageTitle = string.IsNullOrEmpty(questionDTO.GroupTitle) ?
          string.Empty : Regex.IsMatch(questionDTO.GroupTitle, @"^\d") ? questionDTO.GroupTitle.Remove(0, 3) : questionDTO.GroupTitle,

        AnswerCodeSetID = questionDTO.AnswerCodeSetID,
        AnswerCodeCategory = questionDTO.AnswerCodeCategory,
        MultipleChoices = questionDTO.MultipleChoices,

        ChoiceList = SetSelectedChoice(questionDTO.ChoiceList, questionDTO.Answers, questionDTO.AnswerCodeCategory),

        ChoicesAnswers = SetChoicesAnswers(questionDTO),

        Instructions = questionDTO.QuestionInsructions

      };

      return qws;
    }

    public static QuestionHierarchy HydrateHierarchically(List<QuestionDTO> questions, string stageTitle)
    {
      QuestionHierarchy qh = new();
      qh.StageTitle = stageTitle;
      
      List<QuestionWithSelectItems> qwsList = new();
      foreach(var q in questions)
      {
        QuestionWithSelectItems qws = Hydrate(q);
        qwsList.Add(qws);
      }

      var distinctSections = GetDistinctSections2(qwsList);

      /* Sections */
      foreach (SectionInfo thisSection in distinctSections)
      {
        var questionInTheSection = qwsList.Where(q => q.SectionTitle == thisSection.SectionTitle).ToList();

        var questionWithHeaderInstruction = questionInTheSection.Where(q => q.Instructions.Any(qi => qi.DisplayLocation == "SectionHeader")).FirstOrDefault();

        if (questionWithHeaderInstruction != null)
        {
          foreach (var ins in questionWithHeaderInstruction.Instructions)
          {
            thisSection.SectionInstruction += ins.Instruction;
          }
        }

        var questionWithAggregateInstruction = questionInTheSection.Where(q => q.Instructions.Any(qi => qi.DisplayLocation == "SectionFooter")).FirstOrDefault();

        if (questionWithAggregateInstruction != null)
        {
          foreach (var ins in questionWithAggregateInstruction.Instructions)
          {
            thisSection.AggregateInstruction += ins;
          }

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
        List<string> distinctQuestions = questionInTheSection.Select(x => x.Question).Distinct().ToList();
        foreach (string thisQuestionText in distinctQuestions)
        {
          QuestionGroup questionGroup = new();
          questionGroup.SharedQuestionText = thisQuestionText;

          var questionInTheGroup = questionInTheSection.Where(q => q.Question == thisQuestionText).OrderBy(q=>q.QuestionKey).ToList();
          var groupKey = questionInTheGroup.First().QuestionKey;
          questionGroup.SharedQuestionKey = groupKey;

          var groupInstruction = questionInTheGroup.Where(q=>q.Instructions.Any(qi => qi.DisplayLocation == "QuestionBody")).ToList();
          if (groupInstruction != null)
          {
            foreach (var ins in groupInstruction)
              questionGroup.SharedQuestionInstruction += ins.Instructions;
          }

          questionGroup.Questions = questionInTheGroup;

          thisSection.QuestionGroups.Add(questionGroup); 
        }

        qh.Sections.Add(thisSection);
      }

      return qh;
    }      

    public static List<SectionInfo> GetDistinctSections(List<QuestionWithSelectItems> questions)
    {
      var sections = questions.Select(x => $"{x.SectionTitle} {x.Section}").AsParallel().Distinct()
        .Select(x => new SectionInfo {
          SectionTitle = x,
          SectionKey = GetLastKeyWord(x),
        }).OrderBy(x=>x.SectionTitle).ThenBy(x=>x.SectionKey).ToList();
      return sections;
    }

    public static List<SectionInfo> GetDistinctSections2(List<QuestionWithSelectItems> questions)
    {
      var sections = questions.Select(x => new { x.SectionTitle, x.Section }).AsParallel().Distinct()
        .Select(x => new SectionInfo
        { 
          SectionTitle = x.SectionTitle,
          SectionKey = x.Section,
        }).OrderBy(x=>x.SectionTitle).ToList();

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

    private static string GetLastKeyWord(string sectionKey) {
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
        return $"({question.QuestionKey.Substring(0, 6)})" ;
      else
      {
        return $"({question.QuestionKey.Substring(0, 5).TrimEnd()})";
      }
    }

    private static List<SelectListItem> SetSelectedChoice(List<CodeSetDTO> choices, List<AnswerDTO> answers, string answerCodeCategory)
    {
      List<SelectListItem> selectedChoices = new();
      string text = string.Empty, value = string.Empty ;

      if (!choices.Any())
      {
        if (answers.Any())
        {
          foreach (var thisAnswer in answers)
          {
            /* an empty validChoices parameter only possible for questions with Y/N, check, or free text answer 
             so use the text in the answer to populate the selectedChoices with single SelectListItem */
            text = thisAnswer.Description;
            value = thisAnswer.AnswerCodeSet.CodeSetID.ToString();

            if (answerCodeCategory == "Date")
            {
              text = ParseDateString(thisAnswer.Description);
              value = text;
            }

            SelectListItem thisChiceItem = new()
            {
              Text = text,
              Value = text,
              Selected = true
            };
            selectedChoices.Add(thisChiceItem);
          }
        }
        return selectedChoices;
      }
      else
      {
        foreach (var c in choices)
        {
          text = c.CodeDescription;
          var isSelected = answers.Any(a => a.AnswerCodeSet.CodeSetID == c.CodeSetID);
          SelectListItem thisChiceItem = new () { 
            Text = text, 
            Value = c.CodeSetID.ToString(), 
            Selected = isSelected
          };
          selectedChoices.Add(thisChiceItem);
        }
        return selectedChoices;
      }
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

        ChoiceAndAnswer thisChoiceAndAnswer = new();
        thisChoiceAndAnswer.SelectListItem = thisChice;
        thisChoiceAndAnswer.Answer = thisAnswer;

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

    private static string ParseDateString(string originalString)
    {
      string text = string.Empty;
      char[] delimiter = { '/', ' ', '-' };
      string[] dateParts = originalString.Split(delimiter);
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
      else
      {
        text = originalString;
      }
      return text;
    }
  }
}
