﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IPRehabModel
{
    [Table("tblEpisodeOfCare", Schema = "app")]
    public partial class tblEpisodeOfCare
    {
        public tblEpisodeOfCare()
        {
            tblAnswer = new HashSet<tblAnswer>();
        }

        [Key]
        public int EpisodeOfCareID { get; set; }
        [Column(TypeName = "date")]
        public DateTime OnsetDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime AdmissionDate { get; set; }
        [Required]
        [StringLength(10)]
        public string PatientICNFK { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdate { get; set; }

        [ForeignKey(nameof(PatientICNFK))]
        [InverseProperty(nameof(tblPatient.tblEpisodeOfCare))]
        public virtual tblPatient PatientICNFKNavigation { get; set; }
        [InverseProperty("EpisodeCareIDFKNavigation")]
        public virtual tblSignature tblSignature { get; set; }
        [InverseProperty("EpsideOfCareIDFKNavigation")]
        public virtual ICollection<tblAnswer> tblAnswer { get; set; }
    }
}