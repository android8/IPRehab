﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace IPRehabModel
{
  public partial class TblAnswer
  {
    [Key]
    public int AnswerId { get; set; }
    public int EpsideOfCareIdfk { get; set; }
    public int QuestionIdfk { get; set; }
    public int StageIdfk { get; set; }
    public int AnswerCodeSetfk { get; set; }
    public int AnswerSequenceNumber { get; set; }
    public string Description { get; set; }
    public string AnswerByUserId { get; set; }

    public virtual TblCodeSet StageIdFkNavigation { get; set; }
    public virtual TblCodeSet AnswerCodeSetFkNavigation { get; set; }
    public virtual TblEpisodeOfCare EpsideOfCareIdfkNavigation { get; set; }
    public virtual TblQuestion QuestionIdfkNavigation { get; set; }
  }
}