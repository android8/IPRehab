﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IPRehabModel
{
    [Table("tblAnswer", Schema = "app")]
    [Index(nameof(EpsideOfCareIDFK), nameof(MeasureIDFK), nameof(AnswerCodeSetFK), nameof(AnswerSequenceNumber), Name = "IX_tblAnswer", IsUnique = true)]
    [Index(nameof(AnswerCodeSetFK), Name = "IX_tblAnswer_AnswerCodeSetFK")]
    [Index(nameof(EpsideOfCareIDFK), Name = "IX_tblAnswer_EpisodeOfCareIDFK")]
    [Index(nameof(QuestionIDFK), Name = "IX_tblAnswer_QuestionIDFK")]
    public partial class tblAnswer
    {
        [Key]
        public int AnswerID { get; set; }
        public int EpsideOfCareIDFK { get; set; }
        public int QuestionIDFK { get; set; }
        public int MeasureIDFK { get; set; }
        public int AnswerCodeSetFK { get; set; }
        public int AnswerSequenceNumber { get; set; }
        public string Description { get; set; }
        [Required]
        [StringLength(50)]
        public string AnswerByUserID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdate { get; set; }

        [ForeignKey(nameof(AnswerCodeSetFK))]
        [InverseProperty(nameof(tblCodeSet.tblAnswer))]
        public virtual tblCodeSet AnswerCodeSetFKNavigation { get; set; }
        [ForeignKey(nameof(EpsideOfCareIDFK))]
        [InverseProperty(nameof(tblEpisodeOfCare.tblAnswer))]
        public virtual tblEpisodeOfCare EpsideOfCareIDFKNavigation { get; set; }
        [ForeignKey(nameof(MeasureIDFK))]
        [InverseProperty(nameof(tblQuestionMeasure.tblAnswer))]
        public virtual tblQuestionMeasure MeasureIDFKNavigation { get; set; }
        [ForeignKey(nameof(QuestionIDFK))]
        [InverseProperty(nameof(tblQuestion.tblAnswer))]
        public virtual tblQuestion QuestionIDFKNavigation { get; set; }
    }
}