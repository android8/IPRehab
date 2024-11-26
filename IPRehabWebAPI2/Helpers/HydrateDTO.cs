using IPRehabModel;
using IPRehabWebAPI2.Models;
using PatientModel_TreatingSpecialty;
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
        public HydrateDTO()
        {
        }

        //ToDo: should use AutoMapper
        public static QuestionDTO HydrateQuestion(tblQuestion q, string stageName, int stageID, tblQuestionMeasure thisQuestionMeasure)
        {
            QuestionDTO questionDTO = new()
            {
                FormName = stageName,
                StageID = stageID,
                QuestionID = q.QuestionID,
                QuestionKey = q.QuestionKey,
                QuestionSection = q.QuestionSection,
                Question = q.Question
            };

            //if (thisQuestionMeasure == null) //each question may or may not have measure such as Q12, Q23, or AssessmentCompleted so to construct the pseudo measure
            //                                 //use measureCounter and currentQuestion to control multi-measure questions 
            //{
            //    var thisMeasures = q.tblQuestionMeasure.Where(m =>
            //        m.QuestionIDFK == q.QuestionID && m.StageFK == stageID).OrderBy(m => m.MeasureCodeSetIDFK);
            //    if (thisMeasures != null && thisMeasures.Any())
            //    {
            //        switch (thisMeasures.Count())
            //        {
            //            case 1: //only one measure so take First()
            //                {
            //                    questionDTO.Required = thisMeasures.First().Required.HasValue && thisMeasures.First().Required.Value == true;
            //                    questionDTO.MeasureID = thisMeasures.First().Id;
            //                }
            //                break;
            //            case > 1: //increment the measurCounter if not the same question, else reset the measureCounter 
            //                {
            //                    if (currentQuestionID != q.QuestionID)
            //                    {
            //                        currentQuestionID = q.QuestionID;
            //                        measureCounter = 0;
            //                    }

            //                    //temporarily take first from the ordered measures until a better way is found
            //                    var thisMeasure = thisMeasures.ToArray()[measureCounter];
            //                    questionDTO.Required = thisMeasure.Required.HasValue && thisMeasure.Required.Value == true;
            //                    questionDTO.MeasureID = thisMeasure.Id;
            //                    measureCounter++;
            //                }
            //                break;
            //        }
            //    }
            //}
            //else
            //{

            if (thisQuestionMeasure != null)
            {
                questionDTO.Required = thisQuestionMeasure.Required.HasValue && thisQuestionMeasure.Required.Value == true;
                questionDTO.MeasureID = thisQuestionMeasure.Id;
                if (thisQuestionMeasure.MeasureCodeSetIDFKNavigation != null)
                {
                    questionDTO.MeasureDescription = thisQuestionMeasure.MeasureCodeSetIDFKNavigation.CodeDescription; //GetGroupTitle(q, questionStage);
                    questionDTO.MeasureCodeSetID = thisQuestionMeasure.MeasureCodeSetIDFKNavigation.CodeSetID;
                    questionDTO.MeasureCodeValue = thisQuestionMeasure.MeasureCodeSetIDFKNavigation.CodeValue;
                }
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
                PatientIcnFK = thisEpisode.PatientICNFK,
                FacilityID6 = thisEpisode.FacilityID6
            };

            CodeSetDTO measureCodeSet = new();

            if (a.MeasureIDFKNavigation.MeasureCodeSetIDFKNavigation != null)
            {
                measureCodeSet.CodeSetID = a.MeasureIDFKNavigation.MeasureCodeSetIDFKNavigation.CodeSetID;
                measureCodeSet.CodeValue = a.MeasureIDFKNavigation.MeasureCodeSetIDFKNavigation.CodeValue;
                measureCodeSet.CodeDescription = a.MeasureIDFKNavigation.MeasureCodeSetIDFKNavigation.CodeDescription;
                measureCodeSet.Comment = a.MeasureIDFKNavigation.MeasureCodeSetIDFKNavigation.Comment;
            }

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
                MeasureCodeSet = measureCodeSet,
                MeasureID = a.MeasureIDFK,
                AnswerCodeSet = answerCodeSet,
                AnswerSequenceNumber = a.AnswerSequenceNumber,
                Description = a.Description,
                ByUser = a.AnswerByUserID
            };
            return answerDTO;
        }

        /// <summary>
        /// hydrate DTO from vTreatingSpecialtyRecent3Yrs
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static PatientDTOTreatingSpecialty HydrateTreatingSpecialtyPatient(vTreatingSpecialtyRecent3Yrs p)
        {
            return new PatientDTOTreatingSpecialty
            {
                Sta6a = p.bsta6a,
                Name = p.PatientName?.Trim().IndexOf(",") == 0 ? p.PatientName.Trim() : p.PatientName.Replace(",", ", ").Trim(),
                PTFSSN = p.ScrSSNT.Trim(),
                RealSSN = p.RealSSN.Value.ToString().Trim(),
                PatientICN = p.PatientICN?.Trim(),
                DoB = p.DoB.Value,
                Bedsecn = p.bedsecn,
                AdmitDates = new() { p.admitday.Value }
            };
        }

        /// <summary>
        /// hydrate DTO from RptRehabDetails
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static PatientDTOTreatingSpecialty HydrateTreatingSpecialtyPatient(RptRehabDetails p)
        {
            return new PatientDTOTreatingSpecialty
            {
                Sta6a = string.IsNullOrEmpty(p.Bsta6a) ? "NA" : p.Bsta6a,
                Name = string.IsNullOrEmpty(p.PatientName) ? "NA" : p.PatientIcn.Trim().IndexOf(",") == 0 ? p.PatientName.Trim() : p.PatientName.Replace(",", ", ").Trim(),
                PTFSSN = string.IsNullOrEmpty(p.ScrSsnt.Trim()) ? "NA" : p.ScrSsnt.Trim(),
                RealSSN = string.IsNullOrEmpty(p.Realssn) ? "NA" : "XXXXX" + p.Realssn.Trim().Substring(p.Realssn.Length - 4),
                PatientICN = string.IsNullOrEmpty(p.PatientIcn) ? "NA" : p.PatientIcn.Trim(),
                DoB = p.DoB.HasValue ? p.DoB.Value : DateTime.MinValue,
                Bedsecn = p.Bedsecn.HasValue ? p.Bedsecn : -1,
                AdmitDates = p.Admitday.HasValue ? new() { p.Admitday.Value } : new() { DateTime.MinValue }
            };
        }

        public static EpisodeOfCareDTO HydrateEpisodeOfCare(tblEpisodeOfCare e)
        {
            DateTime onsetDate = new(DateTime.MinValue.Ticks);
            bool formIsCompeted = false;
            var completed = e.tblAnswer.Where(a => a.QuestionIDFKNavigation.QuestionKey == "AssessmentCompleted").SingleOrDefault();
            //the Assessment Completed may be entered in any form so it's necessary to enumerate the collection to find if any exist and the CodeDescription is "Yes"
            if (completed?.AnswerCodeSetFKNavigation.CodeDescription == "Yes")
                formIsCompeted = true;

            EpisodeOfCareDTO thisDTO = new()
            {
                EpisodeOfCareID = e.EpisodeOfCareID,

                FacilityID6 = e.FacilityID6,
                AdmissionDate = e.AdmissionDate /* use admit date in episode */,
                OnsetDate = onsetDate,
                PatientIcnFK = e.PatientICNFK,
                FormIsComplete = formIsCompeted
            };

            /* get Q23 for onset date */
            IEnumerable<tblAnswer> keyDates = e.tblAnswer.Where(a =>
               a.EpsideOfCareIDFK == e.EpisodeOfCareID && a.QuestionIDFKNavigation.QuestionKey == "Q23");

            if (keyDates != null && keyDates.Any())
            {
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
                LName = u.lname,
                FName = u.fname,
                AppID = u.appid,
                AcclevID = u.acclevid,
                CPRSnssd = u.cprsnssd,
                Sunsetdat = u.sunsetdat
            };
            return user;
        }

        private static string ParseDateString(string thisString)
        {
            string text = string.Empty;
            char[] parsers = { '/', ' ', '-', '.' };
            string[] dateParts = thisString.Split(parsers);

            int counter = 0;
            foreach (string part in dateParts)
            {
                text += part;
                if (counter <= 1)
                {
                    counter++;
                    text += "/";
                }
            }

            //for (int i = 0; i < 3; i++)
            //{
            //    text += $"{dateParts[i]}";
            //    if (i < 2)
            //        text += "/";
            //}

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