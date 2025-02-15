﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UnstopAPI.Data;

#nullable disable

namespace UnstopAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240613104219_ApplicationTable")]
    partial class ApplicationTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("UnstopAPI.Models.Application", b =>
                {
                    b.Property<int>("ApplicationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ApplicationId"));

                    b.Property<DateTime>("AppliedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("JobId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Resume")
                        .HasColumnType("bytea");

                    b.Property<string>("ResumeContentType")
                        .HasColumnType("text");

                    b.Property<string>("ResumeFileName")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("ApplicationId");

                    b.HasIndex("JobId");

                    b.HasIndex("UserId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("UnstopAPI.Models.Candidate", b =>
                {
                    b.Property<int>("CandidateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CandidateId"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .HasColumnType("text");

                    b.Property<bool>("IsProfileCompleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LinkedInProfile")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("CandidateId");

                    b.HasIndex("UserId");

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("UnstopAPI.Models.Company", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CompanyId"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("CompanyName")
                        .HasColumnType("text");

                    b.Property<int>("FoundedYear")
                        .HasColumnType("integer");

                    b.Property<string>("Industry")
                        .HasColumnType("text");

                    b.Property<byte[]>("Logo")
                        .HasColumnType("bytea");

                    b.Property<string>("LogoContentType")
                        .HasColumnType("text");

                    b.Property<string>("LogoFileName")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("WebsiteUrl")
                        .HasColumnType("text");

                    b.HasKey("CompanyId");

                    b.HasIndex("UserId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("UnstopAPI.Models.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("JobId"));

                    b.Property<string>("Company")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("JobTiming")
                        .HasColumnType("text");

                    b.Property<string>("JobType")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<int>("MaxExperience")
                        .HasColumnType("integer");

                    b.Property<decimal?>("MaxSalary")
                        .HasColumnType("numeric");

                    b.Property<int>("MinExperience")
                        .HasColumnType("integer");

                    b.Property<decimal?>("MinSalary")
                        .HasColumnType("numeric");

                    b.Property<int>("Occupancy")
                        .HasColumnType("integer");

                    b.Property<string>("Requirements")
                        .HasColumnType("text");

                    b.Property<string>("Responsibilities")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("WorkingDays")
                        .HasColumnType("integer");

                    b.HasKey("JobId");

                    b.HasIndex("UserId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("UnstopAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UnstopAPI.Models.Application", b =>
                {
                    b.HasOne("UnstopAPI.Models.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UnstopAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UnstopAPI.Models.Candidate", b =>
                {
                    b.HasOne("UnstopAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UnstopAPI.Models.Company", b =>
                {
                    b.HasOne("UnstopAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UnstopAPI.Models.Job", b =>
                {
                    b.HasOne("UnstopAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
