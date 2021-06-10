using IPRehabModel;
using IPRehabWebAPI2.Models;
using PatientModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IPRehabWebAPI2.Helpers
{

  /// <summary>
  /// Hydrate the DTO with selected domain properties
  /// </summary>
  public class HydrateDTO
  {
    //ToDo: should use AutoMapper
    public static QuestionDTO HydrateQuestion(TblQuestion q)
    {
      return new QuestionDTO
      {
        QuestionID = q.QuestionId,
        QuestionKey = q.QuestionKey,
        QuestionTitle = q.QuestionTitle,
        Question = q.Question,
        GroupTitle = q.GroupTitle,
        AnswerCodeSetID = q.AnswerCodeSetFk,
        AnswerCodeCategory = q.AnswerCodeSetFkNavigation.CodeValue,
        DisplayOrder = q.Order,
        ChoiceList = q.AnswerCodeSetFkNavigation.InverseCodeSetParentNavigation
                        .Select(s => new CodeSetDTO
                        {
                          CodeSetID = s.CodeSetId,
                          CodeSetParent = s.CodeSetParent,
                          CodeValue = s.CodeValue,
                          CodeDescription = s.CodeDescription,
                          Comment = s.Comment
                        }).ToList()
      };
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
        PTFSSN = string.IsNullOrEmpty(p.PTFSSN) ? string.Empty :  $"*{p.PTFSSN.Substring(p.PTFSSN.Length - 4, 4)}",
        FSODSSN = string.IsNullOrEmpty(p.FSODSSN) ? string.Empty : $"*{p.PTFSSN.Substring(p.FSODSSN.Length - 4, 4)}",
        FiscalPeriod = p.FiscalPeriod,
        FiscalPeriodInt = p.FiscalPeriodInt
      };
    }

    public static EpisodeOfCareDTO HydrateEpisodeOfCare(TblEpisodeOfCare e)
    {
      return new EpisodeOfCareDTO
      {
        EpisodeOfCareId = e.EpisodeOfCareId,
        OnsetDate = e.OnsetDate,
        AdmissionDate = e.AdmissionDate,
        PatientIcnfk = "TBD"
      };
    }
  }
}
