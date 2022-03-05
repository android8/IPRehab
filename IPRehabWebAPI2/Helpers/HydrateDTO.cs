﻿using IPRehabModel;
using IPRehabWebAPI2.Models;
using PatientModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UserModel;

namespace IPRehabWebAPI2.Helpers
{
  /// <summary>
  /// Hydrate the DTO with selected domain properties
  /// </summary>
  public class HydrateDTO
  {
    //ToDo: should use AutoMapper
    public static QuestionDTO HydrateQuestion(tblQuestion q, string questionStage, int stageID, tblCodeSet measureCodeSet)
    {
      QuestionDTO questionDTO = new();
      questionDTO.FormName = questionStage;
      questionDTO.StageID = stageID;
      questionDTO.QuestionID = q.QuestionID;
      questionDTO.Required = q.tblQuestionMeasure.Where(x =>
          x.QuestionIDFK == q.QuestionID && x.StageFK == stageID).FirstOrDefault()?.Required;
      questionDTO.QuestionKey = q.QuestionKey;
      questionDTO.QuestionSection = q.QuestionSection;
      questionDTO.Question = q.Question;

      //use question measures
      if (measureCodeSet != null)
      {
        questionDTO.Measure = measureCodeSet.CodeDescription; //GetGroupTitle(q, questionStage);
        questionDTO.MeasureCodeValue = measureCodeSet.CodeValue;
      }
      questionDTO.AnswerCodeSetID = q.AnswerCodeSetFK;
      questionDTO.AnswerCodeCategory = q.AnswerCodeSetFKNavigation.CodeValue;
      questionDTO.DisplayOrder = q.Order;
      questionDTO.MultipleChoices = q.MultiChoice.HasValue;
      questionDTO.ChoiceList = q.AnswerCodeSetFKNavigation.InverseCodeSetParentNavigation.OrderBy(x => x.SortOrder)
                        .Select(s => new CodeSetDTO
                        {
                          CodeSetID = s.CodeSetID,
                          CodeSetParent = s.CodeSetParent,
                          CodeValue = s.CodeValue,
                          CodeDescription = s.CodeDescription.Contains(s.CodeValue) && s.CodeValue.Length > 1 ? $"{s.CodeDescription}" : $"{s.CodeValue}. {s.CodeDescription}",
                          Comment = s.Comment
                        }).ToList();
      questionDTO.QuestionInsructions = GetInstruction(q);
      return questionDTO;
    }

    public static AnswerDTO HydrateAnswer(tblAnswer a, tblEpisodeOfCare thisEpisode)
    {
      EpisodeOfCareDTO episode = new()
      {
        EpisodeOfCareID = thisEpisode.EpisodeOfCareID,
        OnsetDate = thisEpisode.OnsetDate,
        AdmissionDate = thisEpisode.AdmissionDate,
        PatientIcnFK = thisEpisode.PatientICNFK
      };

      CodeSetDTO answerCodeSet = new()
      {
        CodeSetID = a.AnswerCodeSetFK,
        CodeValue = a.AnswerCodeSetFKNavigation.CodeValue,
        CodeDescription = a.AnswerCodeSetFKNavigation.CodeDescription,
        Comment = a.AnswerCodeSetFKNavigation.Comment
      };

      AnswerDTO answerDTO = new()
      {
        AnswerID = a.AnswerID,
        EpisodeOfCare = episode,
        QuestionIDFK = a.QuestionIDFK,
        Measure = a.MeasureIDFKNavigation.MeasureCodeSetIDFKNavigation?.CodeDescription,
        MeasureID = a.MeasureIDFK,
        AnswerCodeSet = answerCodeSet,
        AnswerSequenceNumber = a.AnswerSequenceNumber,
        Description = a.Description,
        ByUser = a.AnswerByUserID
      };
      return answerDTO;
    }

    public static UserFacilityGrant HydrateUserFacilityGrant(FSODPatient p)
    {
      UserFacilityGrant grants = new();
      grants.District.Add(p.District);
      grants.Division.Add(p.Division);
      grants.Facility.Add(p.Facility);

      return grants;
    }

    public static PatientDTO HydratePatient(FSODPatient p)
    {
      return new PatientDTO
      {
        VISN = p.VISN,
        Facility = p.Facility,
        District = p.District,
        Division = p.Division,
        ADMParent_Key = p.ADMParent_Key,
        Sta6aKey = p.Sta6aKey,
        Bedsecn = p.bedsecn,
        Name = p.Name,

        PTFSSN = p.PTFSSN,
        FSODSSN = p.FSODSSN,

        FiscalPeriod = p.FiscalPeriod,
        FiscalPeriodInt = p.FiscalPeriodInt
      };
    }

    public static EpisodeOfCareDTO HydrateEpisodeOfCare(tblEpisodeOfCare e)
    {
      DateTime admissionDate = new(DateTime.MinValue.Ticks);
      DateTime onsetDate = new(DateTime.MinValue.Ticks);
      bool formIsCompeted = false;
      var completed = e.tblAnswer.Where(a => a.QuestionIDFKNavigation.QuestionKey == "AssessmentCompleted");
      foreach(var complete in completed)
      {
        //the Assessment Completed may be entered in any form so it's necessary to enumerate the collection to find if any exist and the CodeDescription is "Yes"
        if (complete?.AnswerCodeSetFKNavigation.CodeDescription == "Yes")
        {
          formIsCompeted = true;
          break;
        }
      }

      EpisodeOfCareDTO thisDTO = new EpisodeOfCareDTO
      {
        EpisodeOfCareID = e.EpisodeOfCareID,
        AdmissionDate = admissionDate,
        OnsetDate = onsetDate,
        PatientIcnFK = e.PatientICNFK,
        FormIsComplete = formIsCompeted
      };

      /* check if Q12 and Q23 have answers and, if yes, trump the episode dates */
      IEnumerable<tblAnswer> keyDates = e.tblAnswer.Where(a =>
         a.QuestionIDFKNavigation.QuestionKey == "Q12" || a.QuestionIDFKNavigation.QuestionKey == "Q23")
        .OrderBy(a => a.QuestionIDFKNavigation.QuestionKey);

      if (keyDates.Any())
      {
        /* there is only one admission date and one onset date, so first must be Q12 admission date*/
        if (DateTime.TryParse(ParseDateString(keyDates.First().Description), out admissionDate))
          thisDTO.AdmissionDate = admissionDate;

        /* the Last() must be onset date */
        if (DateTime.TryParse(ParseDateString(keyDates.Last().Description), out onsetDate))
          thisDTO.OnsetDate = onsetDate;
      }

      return thisDTO;
    }

    public static MastUserDTO HydrateUser(uspVSSCMain_SelectAccessInformationFromNSSDResult u)
    {
      MastUserDTO user = new()
      {
        UserID = u.UserID,
        UserIdentity = u.UserID,
        NTDomain = u.NTDomain,
        NTUserName = u.NTUserName,
        VISN = u.VISN,
        Facility = u.Facility,
        LName = u.LName,
        FName = u.FName,
        AppID = u.AppID,
        AcclevID = u.AcclevID,
        CPRSnssd = u.CPRSnssd,
        Sunsetdat = u.Sunsetdat
      };
      return user;
    }

    private static string ParseDateString(string thisString)
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
      if (DateTime.TryParse(text, out DateTime aDate))
      {
        text = aDate.ToString("yyyy-MM-dd"); /* HTML 5 browser date input must be in this format */
      }
      return text;
    }

    private static List<QuestionInstructionDTO> GetInstruction(tblQuestion q)
    {
      if (q.tblQuestionInstruction.Any(i => i.QuestionIDFK == q.QuestionID))
      {
        return q.tblQuestionInstruction.Where(i => i.QuestionIDFK == q.QuestionID)
          .OrderBy(i => i.Order)
          .Select(i => new QuestionInstructionDTO
          {
            InstructionId = i.InstructionID,
            QuestionIDFK = q.QuestionID,
            Instruction = i.Instruction,
            DisplayLocation = i.DisplayLocationFKNavigation.CodeValue
          }).ToList();
      }
      else
      {
        return null;
      }
    }
  }
}
