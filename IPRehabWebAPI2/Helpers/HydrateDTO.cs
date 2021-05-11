using IPRehabModel;
using IPRehabWebAPI2.Models;
using System.Linq;

namespace IPRehabWebAPI2.Helpers
{

   /// <summary>
   /// Hydrate the DTO with selected domain properties
   /// </summary>
   public class HydrateDTO
   {
      //ToDo: should use AutoMapper
      public static QuestionDTO Hydrate(TblQuestion q)
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
   }
}
