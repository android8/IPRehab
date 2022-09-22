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
    [Index("QuestionIDFK", Name = "IX_tblAnswer_QuestionIDFK")]
    public partial class tblAnswer
    {
        [Key]
        public int AnswerID { get; set; }
        public int EpsideOfCareIDFK { get; set; }
        public int QuestionIDFK { get; set; }
        public int MeasureIDFK { get; set; }
        public int AnswerCodeSetFK { get; set; }
        public int AnswerSequenceNumber { get; set; }
        [Unicode(false)]
        public string Description { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string AnswerByUserID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdate { get; set; }

        [ForeignKey("AnswerCodeSetFK")]
        [InverseProperty("tblAnswer")]
        public virtual tblCodeSet AnswerCodeSetFKNavigation { get; set; }
        [ForeignKey("EpsideOfCareIDFK")]
        [InverseProperty("tblAnswer")]
        public virtual tblEpisodeOfCare EpsideOfCareIDFKNavigation { get; set; }
        [ForeignKey("MeasureIDFK")]
        [InverseProperty("tblAnswer")]
        public virtual tblQuestionMeasure MeasureIDFKNavigation { get; set; }
        [ForeignKey("QuestionIDFK")]
        [InverseProperty("tblAnswer")]
        public virtual tblQuestion QuestionIDFKNavigation { get; set; }
    }
}