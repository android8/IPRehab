﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PatientModel_TreatingSpecialty
{
    public partial class DMTreatingSpecialtyContext : DbContext
    {
        public DMTreatingSpecialtyContext()
        {
        }

        public DMTreatingSpecialtyContext(DbContextOptions<DMTreatingSpecialtyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FactTsallCdw> FactTsallCdw { get; set; }
        public virtual DbSet<FactTsallCdw2yrs> FactTsallCdw2yrs { get; set; }
        public virtual DbSet<FactTsallDayCdw2yrs> FactTsallDayCdw2yrs { get; set; }
        public virtual DbSet<FactTsallRecent> FactTsallRecent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FactTsallCdw>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FactTSAll_CDW");

                entity.Property(e => e.Admission).HasColumnName("admission");

                entity.Property(e => e.AdmitWardLocationSid).HasColumnName("AdmitWardLocationSID");

                entity.Property(e => e.Admitdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("admitdatetime");

                entity.Property(e => e.Admitday)
                    .HasColumnType("datetime")
                    .HasColumnName("admitday");

                entity.Property(e => e.Adtime).HasColumnName("adtime");

                entity.Property(e => e.Amlos)
                    .HasColumnType("numeric(6, 1)")
                    .HasColumnName("amlos");

                entity.Property(e => e.AtimeKey).HasColumnName("atime_key");

                entity.Property(e => e.Attendingphysicianstaffsid).HasColumnName("attendingphysicianstaffsid");

                entity.Property(e => e.BedDateId).HasColumnName("BedDateID");

                entity.Property(e => e.BedDay).HasColumnName("bed_day");

                entity.Property(e => e.Beddate)
                    .HasColumnType("datetime")
                    .HasColumnName("beddate");

                entity.Property(e => e.Bedsecn).HasColumnName("bedsecn");

                entity.Property(e => e.Big).HasColumnName("big");

                entity.Property(e => e.BigMoveId).HasColumnName("BigMoveID");

                entity.Property(e => e.Bsindatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("bsindatetime");

                entity.Property(e => e.Bsinday)
                    .HasColumnType("datetime")
                    .HasColumnName("bsinday");

                entity.Property(e => e.Bsindaynew)
                    .HasColumnType("datetime")
                    .HasColumnName("bsindaynew");

                entity.Property(e => e.Bsoutday)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutday");

                entity.Property(e => e.Bsoutdaynew)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutdaynew");

                entity.Property(e => e.Bsoutdaytime)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutdaytime");

                entity.Property(e => e.Bsoutime).HasColumnName("bsoutime");

                entity.Property(e => e.Bssq).HasColumnName("bssq");

                entity.Property(e => e.Bsta6a)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("bsta6a");

                entity.Property(e => e.CLos).HasColumnName("C_LOS");

                entity.Property(e => e.CenDay).HasColumnName("cen_day");

                entity.Property(e => e.CenKey).HasColumnName("cen_key");

                entity.Property(e => e.Discharge).HasColumnName("discharge");

                entity.Property(e => e.Dischargedatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("dischargedatetime");

                entity.Property(e => e.Disday)
                    .HasColumnType("datetime")
                    .HasColumnName("disday");

                entity.Property(e => e.Distime).HasColumnName("distime");

                entity.Property(e => e.Distype).HasColumnName("distype");

                entity.Property(e => e.Drgb).HasColumnName("drgb");

                entity.Property(e => e.Drgkey).HasColumnName("DRGkey");

                entity.Property(e => e.DtimeKey).HasColumnName("dtime_key");

                entity.Property(e => e.Dxb2)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb2");

                entity.Property(e => e.Dxb3)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb3");

                entity.Property(e => e.Dxb4)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb4");

                entity.Property(e => e.Dxb5)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb5");

                entity.Property(e => e.Dxlsb)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxlsb");

                entity.Property(e => e.EAge).HasColumnName("E_Age");

                entity.Property(e => e.EEnrstat).HasColumnName("E_ENRSTAT");

                entity.Property(e => e.EScper).HasColumnName("E_SCPer");

                entity.Property(e => e.EZipcode)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("E_zipcode");

                entity.Property(e => e.Ethnicitykey).HasColumnName("ethnicitykey");

                entity.Property(e => e.Gainingwardlocationsid).HasColumnName("gainingwardlocationsid");

                entity.Property(e => e.Gmlos)
                    .HasColumnType("numeric(6, 1)")
                    .HasColumnName("gmlos");

                entity.Property(e => e.Los).HasColumnName("los");

                entity.Property(e => e.Lsb).HasColumnName("lsb");

                entity.Property(e => e.Lsbdim)
                    .HasColumnType("decimal(17, 7)")
                    .HasColumnName("lsbdim");

                entity.Property(e => e.Lvb).HasColumnName("lvb");

                entity.Property(e => e.Movement).HasColumnName("movement");

                entity.Property(e => e.ObsKey).HasColumnName("obs_key");

                entity.Property(e => e.OefoifFlag)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("oefoif_flag");

                entity.Property(e => e.Oefoifflag1).HasColumnName("oefoifflag");

                entity.Property(e => e.Passb).HasColumnName("passb");

                entity.Property(e => e.PcpAdmparentkey).HasColumnName("PCP_admparentkey");

                entity.Property(e => e.PcpSta6akey).HasColumnName("PCP_sta6akey");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PrAsihdays)
                    .HasColumnType("decimal(21, 11)")
                    .HasColumnName("pr_asihdays");

                entity.Property(e => e.PrLeave).HasColumnName("pr_leave");

                entity.Property(e => e.PrPass).HasColumnName("pr_pass");

                entity.Property(e => e.Prevlos).HasColumnName("prevlos");

                entity.Property(e => e.PrimaryphysicianstaffSid).HasColumnName("primaryphysicianstaffSID");

                entity.Property(e => e.ProviderSid).HasColumnName("providerSID");

                entity.Property(e => e.Racekey).HasColumnName("racekey");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Realssn).HasColumnName("REALSSN");

                entity.Property(e => e.ScrSsnt)
                    .HasMaxLength(38)
                    .IsUnicode(false)
                    .HasColumnName("ScrSSNT");

                entity.Property(e => e.Scrnum).HasColumnName("scrnum");

                entity.Property(e => e.Scrssn).HasColumnName("scrssn");

                entity.Property(e => e.SigenderKey).HasColumnName("SIGenderKey");

                entity.Property(e => e.Source)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("source");

                entity.Property(e => e.SourceKey).HasColumnName("source_key");

                entity.Property(e => e.Specialtytransferdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("specialtytransferdatetime");

                entity.Property(e => e.Srtkey).HasColumnName("srtkey");

                entity.Property(e => e.Statyp).HasColumnName("statyp");

                entity.Property(e => e.Svcconb).HasColumnName("svcconb");

                entity.Property(e => e.Transin).HasColumnName("transin");

                entity.Property(e => e.Transout).HasColumnName("transout");
            });

            modelBuilder.Entity<FactTsallCdw2yrs>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FactTSAll_CDW_2yrs");

                entity.Property(e => e.Admission).HasColumnName("admission");

                entity.Property(e => e.Admitdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("admitdatetime");

                entity.Property(e => e.Admitday)
                    .HasColumnType("datetime")
                    .HasColumnName("admitday");

                entity.Property(e => e.AdmitwardlocationSid).HasColumnName("admitwardlocationSID");

                entity.Property(e => e.Adtime).HasColumnName("adtime");

                entity.Property(e => e.Asihdays).HasColumnName("ASIHdays");

                entity.Property(e => e.Attendingphysicianstaffsid).HasColumnName("attendingphysicianstaffsid");

                entity.Property(e => e.Bdoc).HasColumnName("bdoc");

                entity.Property(e => e.Bedsecn).HasColumnName("bedsecn");

                entity.Property(e => e.Big).HasColumnName("big");

                entity.Property(e => e.BigMoveId)
                    .HasColumnType("numeric(11, 0)")
                    .HasColumnName("BigMoveID");

                entity.Property(e => e.BsinDayId).HasColumnName("BSInDayID");

                entity.Property(e => e.Bsindatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("bsindatetime");

                entity.Property(e => e.Bsinday)
                    .HasColumnType("datetime")
                    .HasColumnName("bsinday");

                entity.Property(e => e.Bsindaynew)
                    .HasColumnType("datetime")
                    .HasColumnName("bsindaynew");

                entity.Property(e => e.Bsintime).HasColumnName("bsintime");

                entity.Property(e => e.BsoutDayId).HasColumnName("BSOutDayID");

                entity.Property(e => e.Bsoutdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutdatetime");

                entity.Property(e => e.Bsoutday)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutday");

                entity.Property(e => e.Bsoutdaynew)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutdaynew");

                entity.Property(e => e.Bsoutime).HasColumnName("bsoutime");

                entity.Property(e => e.Bssq).HasColumnName("bssq");

                entity.Property(e => e.Bsta6a)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("bsta6a");

                entity.Property(e => e.CenKey).HasColumnName("Cen_Key");

                entity.Property(e => e.Discharge).HasColumnName("discharge");

                entity.Property(e => e.Dischargedatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("dischargedatetime");

                entity.Property(e => e.DischargewardlocationSid).HasColumnName("dischargewardlocationSID");

                entity.Property(e => e.Disday)
                    .HasColumnType("datetime")
                    .HasColumnName("disday");

                entity.Property(e => e.Distime).HasColumnName("distime");

                entity.Property(e => e.Distype).HasColumnName("distype");

                entity.Property(e => e.Drgb).HasColumnName("drgb");

                entity.Property(e => e.Drgkey).HasColumnName("DRGKey");

                entity.Property(e => e.Dxb2)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb2");

                entity.Property(e => e.Dxb3)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb3");

                entity.Property(e => e.Dxb4)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb4");

                entity.Property(e => e.Dxb5)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb5");

                entity.Property(e => e.Dxlsb)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxlsb");

                entity.Property(e => e.EAge).HasColumnName("e_Age");

                entity.Property(e => e.EEnrstat).HasColumnName("e_enrstat");

                entity.Property(e => e.EScper).HasColumnName("E_SCPer");

                entity.Property(e => e.EZipCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("e_zipCode");

                entity.Property(e => e.Ethnicitykey).HasColumnName("ethnicitykey");

                entity.Property(e => e.Gainingwardlocationsid).HasColumnName("gainingwardlocationsid");

                entity.Property(e => e.Icd9key)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ICD9Key");

                entity.Property(e => e.Inpatientfeebasissid)
                    .HasMaxLength(20)
                    .HasColumnName("inpatientfeebasissid");

                entity.Property(e => e.Inpatientsid)
                    .HasMaxLength(20)
                    .HasColumnName("inpatientsid");

                entity.Property(e => e.Los).HasColumnName("los");

                entity.Property(e => e.Lsb).HasColumnName("lsb");

                entity.Property(e => e.Lvb).HasColumnName("lvb");

                entity.Property(e => e.Movement).HasColumnName("movement");

                entity.Property(e => e.MovementId).HasColumnName("movementID");

                entity.Property(e => e.OefoifFlag1)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("oefoif_flag");

                entity.Property(e => e.Oefoifflag).HasColumnName("oefoifflag");

                entity.Property(e => e.Passb).HasColumnName("passb");

                entity.Property(e => e.PcpAdmparentkey).HasColumnName("PCP_admparentkey");

                entity.Property(e => e.PcpSta6akey).HasColumnName("pcp_sta6akey");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PrimaryphysicianstaffSid).HasColumnName("primaryphysicianstaffSID");

                entity.Property(e => e.ProviderSid).HasColumnName("providerSID");

                entity.Property(e => e.Racekey).HasColumnName("racekey");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.RealSsn).HasColumnName("RealSSN");

                entity.Property(e => e.ScrSsn).HasColumnName("scrSSN");

                entity.Property(e => e.ScrSsnt)
                    .HasMaxLength(38)
                    .IsUnicode(false)
                    .HasColumnName("ScrSSNT");

                entity.Property(e => e.Scrnum).HasColumnName("scrnum");

                entity.Property(e => e.SigenderKey).HasColumnName("SIGenderKey");

                entity.Property(e => e.Source)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("source");

                entity.Property(e => e.SourceKey)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("source_key");

                entity.Property(e => e.SpecialtyTransferDateTime).HasColumnType("datetime");

                entity.Property(e => e.Srtkey).HasColumnName("srtkey");

                entity.Property(e => e.Statyp).HasColumnName("statyp");

                entity.Property(e => e.Svcconb)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("svcconb");
            });

            modelBuilder.Entity<FactTsallDayCdw2yrs>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FactTSAll_Day_CDW_2yrs");

                entity.Property(e => e.Admission).HasColumnName("admission");

                entity.Property(e => e.Admitdatetime).HasColumnType("datetime");

                entity.Property(e => e.Admitday)
                    .HasColumnType("datetime")
                    .HasColumnName("admitday");

                entity.Property(e => e.Admitwardlocationsid).HasColumnName("admitwardlocationsid");

                entity.Property(e => e.Adtime).HasColumnName("adtime");

                entity.Property(e => e.Amlos)
                    .HasColumnType("decimal(4, 1)")
                    .HasColumnName("amlos");

                entity.Property(e => e.Asihdays).HasColumnName("asihdays");

                entity.Property(e => e.AtimeKey).HasColumnName("atime_key");

                entity.Property(e => e.Attendingphysicianstaffsid).HasColumnName("attendingphysicianstaffsid");

                entity.Property(e => e.BedDateId).HasColumnName("BedDateID");

                entity.Property(e => e.BedDay).HasColumnName("bed_day");

                entity.Property(e => e.Beddate)
                    .HasColumnType("date")
                    .HasColumnName("beddate");

                entity.Property(e => e.Bedsecn).HasColumnName("bedsecn");

                entity.Property(e => e.Big).HasColumnName("big");

                entity.Property(e => e.BigMoveId)
                    .HasColumnType("numeric(11, 0)")
                    .HasColumnName("BigMoveID");

                entity.Property(e => e.Bsindatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("bsindatetime");

                entity.Property(e => e.Bsinday)
                    .HasColumnType("datetime")
                    .HasColumnName("bsinday");

                entity.Property(e => e.Bsindaynew)
                    .HasColumnType("datetime")
                    .HasColumnName("bsindaynew");

                entity.Property(e => e.Bsoutdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutdatetime");

                entity.Property(e => e.Bsoutday)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutday");

                entity.Property(e => e.Bsoutdaynew)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutdaynew");

                entity.Property(e => e.Bsoutime).HasColumnName("bsoutime");

                entity.Property(e => e.Bssq).HasColumnName("bssq");

                entity.Property(e => e.Bsta6a)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("bsta6a");

                entity.Property(e => e.CLos).HasColumnName("C_LOS");

                entity.Property(e => e.CenDay).HasColumnName("cen_day");

                entity.Property(e => e.CenKey).HasColumnName("cen_key");

                entity.Property(e => e.Discharge).HasColumnName("discharge");

                entity.Property(e => e.Dischargedatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("dischargedatetime");

                entity.Property(e => e.Dischargewardlocationsid).HasColumnName("dischargewardlocationsid");

                entity.Property(e => e.Disday)
                    .HasColumnType("datetime")
                    .HasColumnName("disday");

                entity.Property(e => e.Distime).HasColumnName("distime");

                entity.Property(e => e.Distype).HasColumnName("distype");

                entity.Property(e => e.Drgb).HasColumnName("drgb");

                entity.Property(e => e.Drgkey).HasColumnName("DRGkey");

                entity.Property(e => e.DtimeKey).HasColumnName("dtime_key");

                entity.Property(e => e.Dxb2)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb2");

                entity.Property(e => e.Dxb3)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb3");

                entity.Property(e => e.Dxb4)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb4");

                entity.Property(e => e.Dxb5)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb5");

                entity.Property(e => e.Dxlsb)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxlsb");

                entity.Property(e => e.EAge).HasColumnName("E_Age");

                entity.Property(e => e.EEnrstat).HasColumnName("E_ENRSTAT");

                entity.Property(e => e.EScper).HasColumnName("E_SCPer");

                entity.Property(e => e.EZipCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("E_ZipCode");

                entity.Property(e => e.Ethnicitykey).HasColumnName("ethnicitykey");

                entity.Property(e => e.GainingWardlocationSid).HasColumnName("GainingWardlocationSID");

                entity.Property(e => e.Gmlos)
                    .HasColumnType("decimal(4, 1)")
                    .HasColumnName("gmlos");

                entity.Property(e => e.Inpatientsid)
                    .HasMaxLength(20)
                    .HasColumnName("inpatientsid");

                entity.Property(e => e.Los).HasColumnName("los");

                entity.Property(e => e.Lsb).HasColumnName("lsb");

                entity.Property(e => e.Lsbdim)
                    .HasColumnType("numeric(17, 7)")
                    .HasColumnName("lsbdim");

                entity.Property(e => e.Lvb).HasColumnName("lvb");

                entity.Property(e => e.Movement).HasColumnName("movement");

                entity.Property(e => e.ObsKey).HasColumnName("obs_key");

                entity.Property(e => e.OefoifFlag)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("oefoif_flag");

                entity.Property(e => e.Oefoifflag1).HasColumnName("oefoifflag");

                entity.Property(e => e.Passb).HasColumnName("passb");

                entity.Property(e => e.PcpAdmparentkey).HasColumnName("PCP_admparentkey");

                entity.Property(e => e.PcpSta6akey).HasColumnName("pcp_sta6akey");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PrAsihdays)
                    .HasColumnType("decimal(19, 11)")
                    .HasColumnName("pr_asihdays");

                entity.Property(e => e.PrLeave)
                    .HasColumnType("decimal(19, 11)")
                    .HasColumnName("pr_leave");

                entity.Property(e => e.PrPass)
                    .HasColumnType("decimal(19, 11)")
                    .HasColumnName("pr_pass");

                entity.Property(e => e.Prevlos).HasColumnName("prevlos");

                entity.Property(e => e.PrimaryphysicianstaffSid).HasColumnName("primaryphysicianstaffSID");

                entity.Property(e => e.ProviderSid).HasColumnName("providerSID");

                entity.Property(e => e.Racekey).HasColumnName("racekey");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Realssn).HasColumnName("REALSSN");

                entity.Property(e => e.ScrSsnt)
                    .HasMaxLength(38)
                    .IsUnicode(false)
                    .HasColumnName("ScrSSNT");

                entity.Property(e => e.Scrnum).HasColumnName("scrnum");

                entity.Property(e => e.Scrssn).HasColumnName("scrssn");

                entity.Property(e => e.Sigenderkey).HasColumnName("SIGenderkey");

                entity.Property(e => e.Source)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("source");

                entity.Property(e => e.SourceKey).HasColumnName("source_key");

                entity.Property(e => e.Specialtytransferdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("specialtytransferdatetime");

                entity.Property(e => e.Srtkey).HasColumnName("srtkey");

                entity.Property(e => e.Statyp).HasColumnName("statyp");

                entity.Property(e => e.Svcconb)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("svcconb");

                entity.Property(e => e.Transin).HasColumnName("transin");

                entity.Property(e => e.Transout).HasColumnName("transout");
            });

            modelBuilder.Entity<FactTsallRecent>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FactTSAll_recent");

                entity.Property(e => e.Admission).HasColumnName("admission");

                entity.Property(e => e.Admitdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("admitdatetime");

                entity.Property(e => e.Admitday)
                    .HasColumnType("datetime")
                    .HasColumnName("admitday");

                entity.Property(e => e.Adtime).HasColumnName("adtime");

                entity.Property(e => e.Attendingphysicianstaffsid).HasColumnName("attendingphysicianstaffsid");

                entity.Property(e => e.Bdoc).HasColumnName("bdoc");

                entity.Property(e => e.Bedsecn).HasColumnName("bedsecn");

                entity.Property(e => e.Big).HasColumnName("big");

                entity.Property(e => e.BigMoveId)
                    .HasColumnType("numeric(11, 0)")
                    .HasColumnName("BigMoveID");

                entity.Property(e => e.BsinDayId).HasColumnName("BSInDayID");

                entity.Property(e => e.Bsinday)
                    .HasColumnType("datetime")
                    .HasColumnName("bsinday");

                entity.Property(e => e.Bsindaynew)
                    .HasColumnType("datetime")
                    .HasColumnName("bsindaynew");

                entity.Property(e => e.BsoutDayId).HasColumnName("BSOutDayID");

                entity.Property(e => e.Bsoutday)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutday");

                entity.Property(e => e.Bsoutdaynew)
                    .HasColumnType("datetime")
                    .HasColumnName("bsoutdaynew");

                entity.Property(e => e.Bsoutime).HasColumnName("bsoutime");

                entity.Property(e => e.Bssq).HasColumnName("bssq");

                entity.Property(e => e.Bsta6a)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("bsta6a");

                entity.Property(e => e.CenKey).HasColumnName("cen_key");

                entity.Property(e => e.Discharge).HasColumnName("discharge");

                entity.Property(e => e.Dischargedatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("dischargedatetime");

                entity.Property(e => e.Disday)
                    .HasColumnType("datetime")
                    .HasColumnName("disday");

                entity.Property(e => e.Distime).HasColumnName("distime");

                entity.Property(e => e.Distype).HasColumnName("distype");

                entity.Property(e => e.Drgb).HasColumnName("drgb");

                entity.Property(e => e.Drgkey).HasColumnName("DRGkey");

                entity.Property(e => e.Dxb2)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb2");

                entity.Property(e => e.Dxb3)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb3");

                entity.Property(e => e.Dxb4)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb4");

                entity.Property(e => e.Dxb5)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxb5");

                entity.Property(e => e.Dxlsb)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("dxlsb");

                entity.Property(e => e.EAge).HasColumnName("E_Age");

                entity.Property(e => e.EEnrstat).HasColumnName("E_ENRSTAT");

                entity.Property(e => e.EScper).HasColumnName("E_SCPer");

                entity.Property(e => e.EZipCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("E_ZipCode");

                entity.Property(e => e.Gainingwardlocationsid).HasColumnName("gainingwardlocationsid");

                entity.Property(e => e.Icd9key)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ICD9Key");

                entity.Property(e => e.Los).HasColumnName("los");

                entity.Property(e => e.Lsb).HasColumnName("lsb");

                entity.Property(e => e.Lvb).HasColumnName("lvb");

                entity.Property(e => e.Movement).HasColumnName("movement");

                entity.Property(e => e.OefoifFlag)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("oefoif_flag");

                entity.Property(e => e.Oefoifflag1).HasColumnName("oefoifflag");

                entity.Property(e => e.Passb).HasColumnName("passb");

                entity.Property(e => e.PrimaryphysicianstaffSid).HasColumnName("primaryphysicianstaffSID");

                entity.Property(e => e.ProviderSid).HasColumnName("providerSID");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Realssn).HasColumnName("REALSSN");

                entity.Property(e => e.Sci)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("sci");

                entity.Property(e => e.ScrSsnt)
                    .HasMaxLength(38)
                    .IsUnicode(false)
                    .HasColumnName("ScrSSNT");

                entity.Property(e => e.Scrnum).HasColumnName("scrnum");

                entity.Property(e => e.Scrssn).HasColumnName("scrssn");

                entity.Property(e => e.Source)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("source");

                entity.Property(e => e.SourceKey)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("source_key");

                entity.Property(e => e.Specialtytransferdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("specialtytransferdatetime");

                entity.Property(e => e.Srtkey).HasColumnName("srtkey");

                entity.Property(e => e.Statyp).HasColumnName("statyp");

                entity.Property(e => e.Svcconb)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("svcconb");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}