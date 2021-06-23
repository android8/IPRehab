using IPRehab.Models;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IPRehab.Helpers
{
  public class HydrateVM
  {
    public static QuestionWithSelectItems Hydrate(QuestionDTO dto)
    {
      QuestionWithSelectItems qws = new QuestionWithSelectItems();
      qws.Form = dto.Form;
      qws.QuestionID = dto.QuestionID;
      qws.QuestionKey = dto.QuestionKey;
      qws.QuestionTitle = dto.QuestionTitle;
      qws.Question = dto.Question;
      qws.GroupTitle = string.IsNullOrEmpty(dto.GroupTitle) ? string.Empty: 
        Regex.IsMatch(dto.GroupTitle, @"^\d")? dto.GroupTitle.Remove(0,2): dto.GroupTitle;
      qws.AnswerCodeSetID = dto.AnswerCodeSetID;
      qws.AnswerCodeCategory = dto.AnswerCodeCategory;
      qws.ChoiceList = new List<SelectListItem>();
      foreach (var c in dto.ChoiceList)
      {
        bool selecteThis = false; //ToDo: get the answer ID to determine if this item should be selected
        SelectListItem selectItem = new SelectListItem { Text = c.CodeDescription, Value = c.CodeSetID.ToString(), Selected = selecteThis };
        qws.ChoiceList.Add(selectItem);
      }
      return qws;
    }
  }
}
