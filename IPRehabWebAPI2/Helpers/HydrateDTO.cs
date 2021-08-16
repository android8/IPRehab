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
    public static QuestionDTO HydrateQuestion(TblQuestion q, string questionStage)
    {
      QuestionDTO questionDTO = new QuestionDTO();
      questionDTO.Form = q.FormFkNavigation.CodeDescription;
      questionDTO.QuestionID = q.QuestionId;
      questionDTO.Required = q.TblQuestionStage.Where(x =>
          x.QuestionIdFk == q.QuestionId && x.StageFkNavigation.CodeValue.ToUpper() == questionStage).SingleOrDefault()?.Required;
      questionDTO.QuestionKey = q.QuestionKey;
      questionDTO.QuestionTitle = q.QuestionTitle;
      questionDTO.Question = q.Question;
      //use TblQuestionStage.StageGroupTitle if it is not null or empty else use TblQuestion.GroupTitle
      questionDTO.GroupTitle = GetGroupTitle(q, questionStage);
      questionDTO.AnswerCodeSetID = q.AnswerCodeSetFk;
      questionDTO.AnswerCodeCategory = q.AnswerCodeSetFkNavigation.CodeValue;
      questionDTO.DisplayOrder = q.Order;
      questionDTO.ChoiceList = q.AnswerCodeSetFkNavigation.InverseCodeSetParentNavigation.OrderBy(x => x.SortOrder)
                        .Select(s => new CodeSetDTO
                        {
                          CodeSetID = s.CodeSetId,
                          CodeSetParent = s.CodeSetParent,
                          CodeValue = s.CodeValue,
                          CodeDescription = s.CodeDescription.Contains(s.CodeValue) && s.CodeValue.Length > 1 ? $"{s.CodeDescription}" : $"{s.CodeValue}. {s.CodeDescription}",
                          Comment = s.Comment
                        }).ToList();
      questionDTO.QuestionInsructions = GetInstruction(q);
      return questionDTO;
    }

    public static AnswerDTO HydrateAnswer(TblAnswer a) {
      EpisodeOfCareDTO episode = new()
      {
        EpisodeOfCareID = a.EpsideOfCareIdfk,
        OnsetDate = a.EpsideOfCareIdfkNavigation.OnsetDate,
        AdmissionDate = a.EpsideOfCareIdfkNavigation.AdmissionDate,
        PatientIcnFK = a.EpsideOfCareIdfkNavigation.PatientIcnfk
      };

      CodeSetDTO answerCodeSet = new()
      {
        CodeSetID = a.AnswerCodeSetFkNavigation.CodeSetId,
        CodeValue = a.AnswerCodeSetFkNavigation.CodeValue,
        CodeDescription = a.AnswerCodeSetFkNavigation.CodeDescription,
        Comment = a.AnswerCodeSetFkNavigation.Comment
      };

      AnswerDTO answerDTO = new() {
        EpisodeOfCare = episode,
        QuestionIdFK = a.QuestionIdfk,
        CareStage = a.StageIdFkNavigation.CodeDescription,
        AnswerCodeSet = answerCodeSet,
        AnswerSequenceNumber = a.AnswerSequenceNumber,
        Description = a.Description,
        ByUser = a.AnswerByUserId
      };
      return answerDTO;
    }

    private static string GetGroupTitle(TblQuestion q, string questionStage)
    {
      if (string.IsNullOrEmpty(questionStage) || !q.TblQuestionStage.Any(x =>
        x.QuestionIdFk == q.QuestionId && x.StageFkNavigation.CodeValue == questionStage))
        return q.GroupTitle;
      else
      {
        var stagedQuestions = q.TblQuestionStage.Where(x => x.QuestionIdFk == q.QuestionId && x.StageFkNavigation.CodeValue == questionStage);
        var groupTitle = string.IsNullOrEmpty(stagedQuestions.First().StageGroupTitle) ? q.GroupTitle : stagedQuestions.First().StageGroupTitle;
        return groupTitle;
      }
    }

    public static UserFacilityGrant HydrateUserFacilityGrant(FSODPatientDetailFY21Q2 p)
    {
      UserFacilityGrant grants = new UserFacilityGrant();
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
        PTFSSN = string.IsNullOrEmpty(p.PTFSSN) ? string.Empty : $"{p.PTFSSN}",
        FSODSSN = string.IsNullOrEmpty(p.FSODSSN) ? string.Empty : $"{p.PTFSSN}",
        FiscalPeriod = p.FiscalPeriod,
        FiscalPeriodInt = p.FiscalPeriodInt
      };
    }

    public static EpisodeOfCareDTO HydrateEpisodeOfCare(TblEpisodeOfCare e)
    {
      /* always used Q12 and Q23 answers to determine episode dates */
      DateTime admissionDate = new DateTime(DateTime.MinValue.Ticks);
      var firstAdmissionDate = e.TblAnswer.Where(a => a.EpsideOfCareIdfk == e.EpisodeOfCareId && a.QuestionIdfkNavigation.QuestionKey == "Q12");
      if (firstAdmissionDate.Any())
      {
        DateTime.TryParse(ParseString(firstAdmissionDate.First().Description), out admissionDate);
      }
      if (admissionDate.Ticks == DateTime.MinValue.Ticks)
        admissionDate = e.AdmissionDate;

      DateTime onsetDate = new DateTime(DateTime.MinValue.Ticks);
      var firstOnsetDate = e.TblAnswer.Where(a => a.EpsideOfCareIdfk == e.EpisodeOfCareId && a.QuestionIdfkNavigation.QuestionKey == "Q23");
      if (firstOnsetDate.Any())
      {
        DateTime.TryParse(ParseString(firstOnsetDate.First().Description), out onsetDate);
      }
      if (onsetDate.Ticks == DateTime.MinValue.Ticks)
        onsetDate = e.OnsetDate;

      return new EpisodeOfCareDTO
      {
        EpisodeOfCareID = e.EpisodeOfCareId,
        AdmissionDate = admissionDate,
        OnsetDate =  onsetDate,
        PatientIcnFK = e.PatientIcnfk
      };
    }

    public static MastUserDTO HydrateUser(uspVSSCMain_SelectAccessInformationFromNSSDResult u)
    {
      MastUserDTO user = new MastUserDTO
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
      if (DateTime.TryParse(text, out aDate))
      {
        text = aDate.ToString("yyyy-MM-dd"); /* HTML 5 browser date input must be in this format */
      }
      return text;
    }

    private static List<QuestionInstructionDTO> GetInstruction(TblQuestion q)
    {
      if (q.TblQuestionInstruction.Any(i => i.QuestionIdfk == q.QuestionId))
      {
        return q.TblQuestionInstruction.Where(i => i.QuestionIdfk == q.QuestionId)
          .Select(i=> new QuestionInstructionDTO {
            InstructionId = i.InstructionId,
            QuestionIdfk = q.QuestionId,
            Instruction =i.Instruction,
            DisplayLocation = i.DisplayLocation
        }).ToList();
      }
      else
      {
        return null;
      }
    }
  }
}
