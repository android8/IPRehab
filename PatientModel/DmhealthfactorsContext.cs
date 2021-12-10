﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PatientModel
{
    public partial class DmhealthfactorsContext : DbContext
    {
        public DmhealthfactorsContext()
        {
        }

        public DmhealthfactorsContext(DbContextOptions<DmhealthfactorsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FSODPatient> FSODPatient { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<FSODPatient>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FSODPatientDetailFY21Q4", "FSOD");

                entity.Property(e => e.District)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Division).HasMaxLength(356);

                entity.Property(e => e.FSODSSN).HasMaxLength(255);

                entity.Property(e => e.Facility).HasMaxLength(356);

                entity.Property(e => e.FiscalPeriod)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(137)
                    .IsUnicode(false);

                entity.Property(e => e.PTFSSN)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.VISN)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}