using IPRehabModel;
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
    public static QuestionDTO HydrateQuestion(tblQuestion q, string questionStage)
    {
      QuestionDTO questionDTO = new();
      questionDTO.FormName = questionStage;
      questionDTO.QuestionID = q.QuestionID;
      questionDTO.Required = q.tblQuestionStage.Where(x =>
          x.QuestionIDFK == q.QuestionID && x.StageFKNavigation.CodeValue.ToUpper() == questionStage).SingleOrDefault()?.Required;
      questionDTO.QuestionKey = q.QuestionKey;
      questionDTO.QuestionSection = q.QuestionSection;
      questionDTO.Question = q.Question;
      //use tblQuestionStage.StageGroupTitle if it is not null or empty else use tblQuestion.GroupTitle
      questionDTO.GroupTitle = GetGroupTitle(q, questionStage);
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

    public static AnswerDTO HydrateAnswer(tblAnswer a)
    {
      EpisodeOfCareDTO episode = new()
      {
        EpisodeOfCareID = a.EpsideOfCareIDFK,
        OnsetDate = a.EpsideOfCareIDFKNavigation.OnsetDate,
        AdmissionDate = a.EpsideOfCareIDFKNavigation.AdmissionDate,
        PatientIcnFK = a.EpsideOfCareIDFKNavigation.PatientICNFK
      };

      CodeSetDTO answerCodeSet = new()
      {
        CodeSetID = a.AnswerCodeSetFKNavigation.CodeSetID,
        CodeValue = a.AnswerCodeSetFKNavigation.CodeValue,
        CodeDescription = a.AnswerCodeSetFKNavigation.CodeDescription,
        Comment = a.AnswerCodeSetFKNavigation.Comment
      };

      AnswerDTO answerDTO = new()
      {
        EpisodeOfCare = episode,
        QuestionIDFK = a.QuestionIDFK,
        CareStage = a.StageIDFKNavigation.CodeDescription,
        AnswerCodeSet = answerCodeSet,
        AnswerSequenceNumber = a.AnswerSequenceNumber,
        Description = a.Description,
        ByUser = a.AnswerByUserID
      };
      return answerDTO;
    }

    private static string GetGroupTitle(tblQuestion q, string questionStage)
    {
      var alternateTitle = q.tblQuestionStage.Where(x => x.QuestionIDFK == q.QuestionID &&
          (x.StageFKNavigation.CodeValue.ToUpper() == questionStage));
      if (!alternateTitle.Any())
        return q.GroupTitle;
      else
      {
        if (!string.IsNullOrEmpty(alternateTitle.First().StageGroupTitle))
          return alternateTitle.First().StageGroupTitle;
        else
          return q.GroupTitle;
      }
    }

    public static UserFacilityGrant HydrateUserFacilityGrant(FSODPatientDetailFY21Q2 p)
    {
      UserFacilityGrant grants = new();
      grants.District.Add(p.District);
      grants.Division.Add(p.Division);
      grants.Facility.Add(p.Facility);

      return grants;
    }

    public static PatientDTO HydratePatient(FSODPatientDetailFY21Q2 p)
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
        //PTFSSN = string.IsNullOrEmpty(p.PTFSSN) ? string.Empty :  $"*{p.PTFSSN.Substring(p.PTFSSN.Length - 4, 4)}",
        //FSODSSN = string.IsNullOrEmpty(p.FSODSSN) ? string.Empty : $"*{p.PTFSSN.Substring(p.FSODSSN.Length - 4, 4)}",
        PTFSSN = string.IsNullOrEmpty(p.PTFSSN) ? string.Empty : $"{p.PTFSSN.Substring(p.PTFSSN.Length-4)}",
        FSODSSN = string.IsNullOrEmpty(p.FSODSSN) ? string.Empty : $"{p.PTFSSN.Substring(p.PTFSSN.Length - 4)}",
        FiscalPeriod = p.FiscalPeriod,
        FiscalPeriodInt = p.FiscalPeriodInt
      };
    }

    public static EpisodeOfCareDTO HydrateEpisodeOfCare(tblEpisodeOfCare e)
    {
      DateTime admissionDate = new(DateTime.MinValue.Ticks);
      DateTime onsetDate = new(DateTime.MinValue.Ticks);

      /* check if Q12 and Q23 have answers and, it yes, trump the episode dates */
      IEnumerable<tblAnswer> keyDates = e.tblAnswer.Where(a =>
        a.EpsideOfCareIDFK == e.EpisodeOfCareID && (a.QuestionIDFKNavigation.QuestionKey == "Q12" || a.QuestionIDFKNavigation.QuestionKey == "Q23"))
        .OrderBy(a => a.QuestionIDFKNavigation.Order).ThenBy(a => a.QuestionIDFKNavigation.QuestionKey);
      if (keyDates.Any())
      {
        /* there is only one admission date and one onset date, so first must be Q12 admission date*/
        if (DateTime.TryParse(ParseDateString(keyDates.First().Description), out admissionDate))
        {
          if (admissionDate.Ticks == DateTime.MinValue.Ticks)
          {
            admissionDate = e.AdmissionDate;
          }
        }

        /* the Last() must be onset date */
        if (DateTime.TryParse(ParseDateString(keyDates.Last().Description), out onsetDate))
        {
          if (onsetDate.Ticks == DateTime.MinValue.Ticks)
          {
            onsetDate = e.OnsetDate;
          }
        }
      }

      return new EpisodeOfCareDTO
      {
        EpisodeOfCareID = e.EpisodeOfCareID,
        AdmissionDate = admissionDate,
        OnsetDate = onsetDate,
        PatientIcnFK = e.PatientICNFK
      };
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
