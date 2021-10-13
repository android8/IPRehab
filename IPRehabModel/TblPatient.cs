﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IPRehabModel
{
    [Table("tblPatient", Schema = "app")]
    [Index(nameof(LastName), nameof(FirstName), nameof(MiddleName), nameof(Last4SSN), Name = "IX_tblPatient_UniqueName", IsUnique = true)]
    public partial class tblPatient
    {
        [Key]
        [StringLength(10)]
        public string ICN { get; set; }
        [Required]
        [StringLength(10)]
        public string IEN { get; set; }
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(20)]
        public string LastName { get; set; }
        [StringLength(20)]
        public string MiddleName { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [StringLength(4)]
        public string Last4SSN { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdate { get; set; }
    }
}