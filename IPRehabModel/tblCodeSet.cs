﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IPRehabModel
{
    [Table("tblCodeSet", Schema = "app")]
    [Index("CodeSetParent", "CodeValue", Name = "IX_tblCodeSet", IsUnique = true)]
    public partial class tblCodeSet
    {
        public tblCodeSet()
        {
            InverseCodeSetParentNavigation = new HashSet<tblCodeSet>();
            tblAnswer = new HashSet<tblAnswer>();
            tblQuestion = new HashSet<tblQuestion>();
            tblQuestionInstructionDisplayLocationFKNavigation = new HashSet<tblQuestionInstruction>();
            tblQuestionInstructionStageCodeSetIDFKNavigation = new HashSet<tblQuestionInstruction>();
            tblQuestionMeasureMeasureCodeSetIDFKNavigation = new HashSet<tblQuestionMeasure>();
            tblQuestionMeasureStageFKNavigation = new HashSet<tblQuestionMeasure>();
        }

        [Key]
        public int CodeSetID { get; set; }
        public int? CodeSetParent { get; set; }
        [Required]
        [StringLength(30)]
        [Unicode(false)]
        public string CodeValue { get; set; }
        [Required]
        [StringLength(400)]
        [Unicode(false)]
        public string CodeDescription { get; set; }
        public int? HierarchyType { get; set; }
        public bool? Active { get; set; }
        public int? FyConstraint { get; set; }
        public int? SortOrder { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string Comment { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdate { get; set; }

        [ForeignKey("CodeSetParent")]
        [InverseProperty("InverseCodeSetParentNavigation")]
        public virtual tblCodeSet CodeSetParentNavigation { get; set; }
        [InverseProperty("CodeSetParentNavigation")]
        public virtual ICollection<tblCodeSet> InverseCodeSetParentNavigation { get; set; }
        [InverseProperty("AnswerCodeSetFKNavigation")]
        public virtual ICollection<tblAnswer> tblAnswer { get; set; }
        [InverseProperty("AnswerCodeSetFKNavigation")]
        public virtual ICollection<tblQuestion> tblQuestion { get; set; }
        [InverseProperty("DisplayLocationFKNavigation")]
        public virtual ICollection<tblQuestionInstruction> tblQuestionInstructionDisplayLocationFKNavigation { get; set; }
        [InverseProperty("StageCodeSetIDFKNavigation")]
        public virtual ICollection<tblQuestionInstruction> tblQuestionInstructionStageCodeSetIDFKNavigation { get; set; }
        [InverseProperty("MeasureCodeSetIDFKNavigation")]
        public virtual ICollection<tblQuestionMeasure> tblQuestionMeasureMeasureCodeSetIDFKNavigation { get; set; }
        [InverseProperty("StageFKNavigation")]
        public virtual ICollection<tblQuestionMeasure> tblQuestionMeasureStageFKNavigation { get; set; }
    }
}