﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace IPRehabModel
{
  public partial class TblEpisodeOfCare
  {
    public TblEpisodeOfCare()
    {
      TblAnswer = new HashSet<TblAnswer>();
    }

    public virtual TblPatient PatientIcnfkNavigation { get; set; }
    public virtual TblSignature TblSignature { get; set; }
    public virtual ICollection<TblAnswer> TblAnswer { get; set; }

    public int EpisodeOfCareId { get; set; }
    public DateTime OnsetDate { get; set; }
    public DateTime AdmissionDate { get; set; }
    public string PatientIcnfk { get; set; }
  }
}