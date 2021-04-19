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
    [Index(nameof(LastName), nameof(FirstName), nameof(MiddleName), nameof(Last4Ssn), Name = "IX_tblPatient_UniqueName", IsUnique = true)]
    public partial class TblPatient
    {
        public TblPatient()
        {
            TblEpisodeOfCare = new HashSet<TblEpisodeOfCare>();
        }

        [Key]
        [Column("ICN")]
        [StringLength(10)]
        public string Icn { get; set; }
        [Required]
        [Column("IEN")]
        [StringLength(10)]
        public string Ien { get; set; }
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
        [Column("Last4SSN")]
        [StringLength(4)]
        public string Last4Ssn { get; set; }

        [InverseProperty("PatientIcnfkNavigation")]
        public virtual ICollection<TblEpisodeOfCare> TblEpisodeOfCare { get; set; }
    }
}