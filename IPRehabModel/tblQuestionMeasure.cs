﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IPRehabModel
{
    [Table("tblQuestionMeasure", Schema = "app")]
    [Index(nameof(QuestionIDFK), nameof(StageFK), nameof(MeasureCodeSetIDFK), Name = "IX_tblQuestionMeasure", IsUnique = true)]
    [Index(nameof(QuestionIDFK), Name = "IX_tblQuestionMeasure_QuestionIdFk")]
    [Index(nameof(StageFK), Name = "IX_tblQuestionMeasure_StageFk")]
    public partial class tblQuestionMeasure
    {
        public tblQuestionMeasure()
        {
            tblAnswer = new HashSet<tblAnswer>();
        }

        [Key]
        public int Id { get; set; }
        public int QuestionIDFK { get; set; }
        public int StageFK { get; set; }
        [Required]
        public bool? Required { get; set; }
        public int? MeasureCodeSetIDFK { get; set; }
        [StringLength(500)]
        public string Comment { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdate { get; set; }

        [ForeignKey(nameof(MeasureCodeSetIDFK))]
        [InverseProperty(nameof(tblCodeSet.tblQuestionMeasureMeasureCodeSetIDFKNavigation))]
        public virtual tblCodeSet MeasureCodeSetIDFKNavigation { get; set; }
        [ForeignKey(nameof(QuestionIDFK))]
        [InverseProperty(nameof(tblQuestion.tblQuestionMeasure))]
        public virtual tblQuestion QuestionIDFKNavigation { get; set; }
        [ForeignKey(nameof(StageFK))]
        [InverseProperty(nameof(tblCodeSet.tblQuestionMeasureStageFKNavigation))]
        public virtual tblCodeSet StageFKNavigation { get; set; }
        [InverseProperty("MeasureIDFKNavigation")]
        public virtual ICollection<tblAnswer> tblAnswer { get; set; }
    }
}