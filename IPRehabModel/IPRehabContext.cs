﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace IPRehabModel
{
    public partial class IPRehabContext : IdentityDbContext<ApplicationUser>
    {
        public IPRehabContext()
        {
        }

        public IPRehabContext(DbContextOptions<IPRehabContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblAnswer> TblAnswer { get; set; }
        public virtual DbSet<TblCodeSet> TblCodeSet { get; set; }
        public virtual DbSet<TblEpisodeOfCare> TblEpisodeOfCare { get; set; }
        public virtual DbSet<TblPatient> TblPatient { get; set; }
        public virtual DbSet<TblQuestion> TblQuestion { get; set; }
        public virtual DbSet<TblQuestionInstruction> TblQuestionInstruction { get; set; }
        public virtual DbSet<TblSignature> TblSignature { get; set; }
        public virtual DbSet<TblUser> TblUser { get; set; }
        public virtual DbSet<VCodeSetHierarchy> VCodeSetHierarchy { get; set; }
        public virtual DbSet<VQuestionStandardChoices> VQuestionStandardChoices { get; set; }
        public virtual DbSet<VQuestionStandardChoicesCondensed> VQuestionStandardChoicesCondensed { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("app")
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<TblAnswer>(entity =>
            {
                entity.Property(e => e.EpsideOfCareIdfk).ValueGeneratedNever();

                entity.Property(e => e.Description).IsUnicode(false);

                entity.HasOne(d => d.AnswerCodeSetFkNavigation)
                    .WithMany(p => p.TblAnswer)
                    .HasForeignKey(d => d.AnswerCodeSetFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblAnswer_tblCodeSet");

                entity.HasOne(d => d.EpsideOfCareIdfkNavigation)
                    .WithOne(p => p.TblAnswer)
                    .HasForeignKey<TblAnswer>(d => d.EpsideOfCareIdfk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblAnswer_tblEpisodeOfCare");

                entity.HasOne(d => d.QuestionIdfkNavigation)
                    .WithMany(p => p.TblAnswer)
                    .HasForeignKey(d => d.QuestionIdfk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblAnswer_tblQuestion");
            });

            modelBuilder.Entity<TblCodeSet>(entity =>
            {
                entity.Property(e => e.CodeDescription).IsUnicode(false);

                entity.Property(e => e.CodeValue).IsUnicode(false);

                entity.Property(e => e.Comment).IsUnicode(false);

                entity.HasOne(d => d.CodeSetParentNavigation)
                    .WithMany(p => p.InverseCodeSetParentNavigation)
                    .HasForeignKey(d => d.CodeSetParent)
                    .HasConstraintName("FK_tblCodeSet_tblCodeSet");
            });

            modelBuilder.Entity<TblEpisodeOfCare>(entity =>
            {
                entity.HasKey(e => e.EpisodeOfCareId)
                    .HasName("PK_app.tblEpisodeOfCare");

                entity.Property(e => e.PatientIcnfk).IsUnicode(false);

                entity.HasOne(d => d.PatientIcnfkNavigation)
                    .WithMany(p => p.TblEpisodeOfCare)
                    .HasForeignKey(d => d.PatientIcnfk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_app.tblEpisodeOfCare_app.tblPatient");
            });

            modelBuilder.Entity<TblPatient>(entity =>
            {
                entity.Property(e => e.Icn).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.Ien).IsUnicode(false);

                entity.Property(e => e.Last4Ssn)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.MiddleName).IsUnicode(false);
            });

            modelBuilder.Entity<TblQuestion>(entity =>
            {
                entity.HasKey(e => e.QuestionId)
                    .HasName("PK_app.tblQuestion");

                entity.Property(e => e.GroupTitle).IsUnicode(false);

                entity.Property(e => e.Question).IsUnicode(false);

                entity.Property(e => e.QuestionKey).IsUnicode(false);

                entity.Property(e => e.QuestionTitle).IsUnicode(false);

                entity.HasOne(d => d.AnswerCodeSetFkNavigation)
                    .WithMany(p => p.TblQuestionAnswerCodeSetFkNavigation)
                    .HasForeignKey(d => d.AnswerCodeSetFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_app.tblQuestion_tblCodeSet");

                entity.HasOne(d => d.FormFkNavigation)
                    .WithMany(p => p.TblQuestionFormFkNavigation)
                    .HasForeignKey(d => d.FormFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblQuestion_tblCodeSet_Form");

                entity.HasOne(d => d.FormSectionFkNavigation)
                    .WithMany(p => p.TblQuestionFormSectionFkNavigation)
                    .HasForeignKey(d => d.FormSectionFk)
                    .HasConstraintName("FK_tblQuestion_tblCodeSet_FomSection");
            });

            modelBuilder.Entity<TblQuestionInstruction>(entity =>
            {
                entity.HasKey(e => e.InstructionId)
                    .HasName("PK_tblInstruction");

                entity.Property(e => e.Instruction).IsUnicode(false);

                entity.HasOne(d => d.QuestionIdfkNavigation)
                    .WithMany(p => p.TblQuestionInstruction)
                    .HasForeignKey(d => d.QuestionIdfk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblInstruction_tblQuestion");
            });

            modelBuilder.Entity<TblSignature>(entity =>
            {
                entity.Property(e => e.EpisodeCareIdfk).ValueGeneratedNever();

                entity.Property(e => e.Signature).IsFixedLength(true);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.HasOne(d => d.EpisodeCareIdfkNavigation)
                    .WithOne(p => p.TblSignature)
                    .HasForeignKey<TblSignature>(d => d.EpisodeCareIdfk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblSignature_tblEpisodeOfCare");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);
            });

            modelBuilder.Entity<VCodeSetHierarchy>(entity =>
            {
                entity.ToView("vCodeSetHierarchy", "app");

                entity.Property(e => e.AncestorDescription).IsUnicode(false);

                entity.Property(e => e.AncestorValue).IsUnicode(false);

                entity.Property(e => e.AntiquityDescription).IsUnicode(false);

                entity.Property(e => e.AntiquityValue).IsUnicode(false);

                entity.Property(e => e.ChildDescription).IsUnicode(false);

                entity.Property(e => e.ChildValue).IsUnicode(false);

                entity.Property(e => e.GrandDescription).IsUnicode(false);

                entity.Property(e => e.GrandValue).IsUnicode(false);

                entity.Property(e => e.GreatDescription).IsUnicode(false);

                entity.Property(e => e.GreatValue).IsUnicode(false);

                entity.Property(e => e.Hierarchy).IsUnicode(false);

                entity.Property(e => e.ParentComment).IsUnicode(false);

                entity.Property(e => e.ParentDescription).IsUnicode(false);

                entity.Property(e => e.ParentValue).IsUnicode(false);
            });

            modelBuilder.Entity<VQuestionStandardChoices>(entity =>
            {
                entity.ToView("vQuestionStandardChoices", "app");

                entity.Property(e => e.AncestorDescription).IsUnicode(false);

                entity.Property(e => e.AncestorValue).IsUnicode(false);

                entity.Property(e => e.AntiquityDescription).IsUnicode(false);

                entity.Property(e => e.AntiquityValue).IsUnicode(false);

                entity.Property(e => e.ChildDescription).IsUnicode(false);

                entity.Property(e => e.ChildValue).IsUnicode(false);

                entity.Property(e => e.GrandDescription).IsUnicode(false);

                entity.Property(e => e.GrandValue).IsUnicode(false);

                entity.Property(e => e.GreatDescription).IsUnicode(false);

                entity.Property(e => e.GreatValue).IsUnicode(false);

                entity.Property(e => e.GroupTitle).IsUnicode(false);

                entity.Property(e => e.Hierarchy).IsUnicode(false);

                entity.Property(e => e.ParentDescription).IsUnicode(false);

                entity.Property(e => e.ParentValue).IsUnicode(false);

                entity.Property(e => e.Question).IsUnicode(false);

                entity.Property(e => e.QuestionKey).IsUnicode(false);

                entity.Property(e => e.QuestionTitle).IsUnicode(false);
            });

            modelBuilder.Entity<VQuestionStandardChoicesCondensed>(entity =>
            {
                entity.ToView("vQuestionStandardChoices_Condensed", "app");

                entity.Property(e => e.ChoiceCode).IsUnicode(false);

                entity.Property(e => e.CodeSetComment).IsUnicode(false);

                entity.Property(e => e.Question).IsUnicode(false);

                entity.Property(e => e.QuestionKey).IsUnicode(false);

                entity.Property(e => e.ValidChoice).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}