﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IPRehabModel
{
    [Table("tblBranching", Schema = "app")]
    [Index(nameof(BranchingName), Name = "IX_tblBranching_Name")]
    [Index(nameof(FromQuestionID), nameof(ToQuestionID), nameof(BranchingName), Name = "IX_tblBranching_To_And_From", IsUnique = true)]
    public partial class tblBranching
    {
        [Key]
        public int BranchingID { get; set; }
        [Required]
        [StringLength(255)]
        public string BranchingName { get; set; }
        public int FromQuestionID { get; set; }
        public int ToQuestionID { get; set; }
        [StringLength(250)]
        public string Condition { get; set; }

        [ForeignKey(nameof(FromQuestionID))]
        [InverseProperty(nameof(tblQuestion.tblBranchingFromQuestion))]
        public virtual tblQuestion FromQuestion { get; set; }
        [ForeignKey(nameof(ToQuestionID))]
        [InverseProperty(nameof(tblQuestion.tblBranchingToQuestion))]
        public virtual tblQuestion ToQuestion { get; set; }
    }
}