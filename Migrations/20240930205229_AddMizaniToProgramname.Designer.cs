﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyUni.Data;

#nullable disable

namespace MyUni.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240930205229_AddMizaniToProgramname")]
    partial class AddMizaniToProgramname
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MyUni.Models.Entities.EventCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Time")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isFeatured")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("MyEventCard");
                });

            modelBuilder.Entity("MyUni.Models.Entities.EventCard+EventType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("EventCardId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EventCardId");

                    b.ToTable("EventType");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("MyprogramCard");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard+CheckBoxes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ChackBoxName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProgramNamesId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProgramNamesId");

                    b.ToTable("CheckBoxes");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard+Field", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FieldName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProgramCardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProgramCardId");

                    b.ToTable("Field");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard+ProgramNames", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("FieldId")
                        .HasColumnType("int");

                    b.Property<string>("programname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FieldId");

                    b.ToTable("ProgramNames");
                });

            modelBuilder.Entity("MyUni.Models.Entities.Quiz", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Time")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MyQuiz");
                });

            modelBuilder.Entity("MyUni.Models.Entities.Quiz+Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("QuizId")
                        .HasColumnType("int");

                    b.Property<string>("correctanswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("img")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("question")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("MyUni.Models.Entities.Quiz+inccorectanswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("InccorectAnswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("inccorectanswer");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ExchangePrograms")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ForPupil")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("History")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Labs")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MainText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethods")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ScholarshipAndFunding")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StudentsLife")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MyUniCard");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+ArchevitiSavaldebuloSagani", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UniCardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UniCardId");

                    b.ToTable("ArchevitiSavaldebuloSagani");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+ArchevitiSavaldebuloSagnebi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdgilebisRaodenoba")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ArchevitiSavaldebuloSaganiId")
                        .HasColumnType("int");

                    b.Property<string>("Koeficienti")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MinimaluriZgvari")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prioriteti")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SagnisSaxeli")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ArchevitiSavaldebuloSaganiId");

                    b.ToTable("ArchevitiSavaldebuloSagnebi");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Time")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UniCardId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UniCardId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Programname", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdgilebisRaodenoba")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dafinanseba")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fasi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Jobs")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kodi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("KreditebisRaodenoba")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kvalifikacia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mizani")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProgramName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProgramisAgwera")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SectionId")
                        .HasColumnType("int");

                    b.Property<string>("SwavlebisEna")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("Programname");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+SavaldebuloSagnebi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdgilebisRaodenoba")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Koeficienti")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MinimaluriZgvari")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prioriteti")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SagnisSaxeli")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Section2Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Section2Id");

                    b.ToTable("SavaldebuloSagnebi");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UniCardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UniCardId");

                    b.ToTable("Section");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Section2", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UniCardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UniCardId");

                    b.ToTable("Section2");
                });

            modelBuilder.Entity("MyUni.Models.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Coin")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Img")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResetToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResetTokenExpiry")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MyUser");
                });

            modelBuilder.Entity("MyUni.Models.Entities.EventCard+EventType", b =>
                {
                    b.HasOne("MyUni.Models.Entities.EventCard", null)
                        .WithMany("Types")
                        .HasForeignKey("EventCardId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard+CheckBoxes", b =>
                {
                    b.HasOne("MyUni.Models.Entities.ProgramCard+ProgramNames", null)
                        .WithMany("CheckBoxes")
                        .HasForeignKey("ProgramNamesId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard+Field", b =>
                {
                    b.HasOne("MyUni.Models.Entities.ProgramCard", null)
                        .WithMany("Fields")
                        .HasForeignKey("ProgramCardId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard+ProgramNames", b =>
                {
                    b.HasOne("MyUni.Models.Entities.ProgramCard+Field", null)
                        .WithMany("ProgramNames")
                        .HasForeignKey("FieldId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.Quiz+Question", b =>
                {
                    b.HasOne("MyUni.Models.Entities.Quiz", null)
                        .WithMany("Questions")
                        .HasForeignKey("QuizId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.Quiz+inccorectanswer", b =>
                {
                    b.HasOne("MyUni.Models.Entities.Quiz+Question", null)
                        .WithMany("IncorrectAnswers")
                        .HasForeignKey("QuestionId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+ArchevitiSavaldebuloSagani", b =>
                {
                    b.HasOne("MyUni.Models.Entities.UniCard", null)
                        .WithMany("ArchevitiSavaldebuloSaganebi")
                        .HasForeignKey("UniCardId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+ArchevitiSavaldebuloSagnebi", b =>
                {
                    b.HasOne("MyUni.Models.Entities.UniCard+ArchevitiSavaldebuloSagani", null)
                        .WithMany("ArchevitiSavaldebuloSagnebi")
                        .HasForeignKey("ArchevitiSavaldebuloSaganiId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Event", b =>
                {
                    b.HasOne("MyUni.Models.Entities.UniCard", null)
                        .WithMany("Events")
                        .HasForeignKey("UniCardId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Programname", b =>
                {
                    b.HasOne("MyUni.Models.Entities.UniCard+Section", null)
                        .WithMany("ProgramNames")
                        .HasForeignKey("SectionId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+SavaldebuloSagnebi", b =>
                {
                    b.HasOne("MyUni.Models.Entities.UniCard+Section2", null)
                        .WithMany("SavaldebuloSagnebi")
                        .HasForeignKey("Section2Id");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Section", b =>
                {
                    b.HasOne("MyUni.Models.Entities.UniCard", null)
                        .WithMany("Sections")
                        .HasForeignKey("UniCardId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Section2", b =>
                {
                    b.HasOne("MyUni.Models.Entities.UniCard", null)
                        .WithMany("Sections2")
                        .HasForeignKey("UniCardId");
                });

            modelBuilder.Entity("MyUni.Models.Entities.EventCard", b =>
                {
                    b.Navigation("Types");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard", b =>
                {
                    b.Navigation("Fields");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard+Field", b =>
                {
                    b.Navigation("ProgramNames");
                });

            modelBuilder.Entity("MyUni.Models.Entities.ProgramCard+ProgramNames", b =>
                {
                    b.Navigation("CheckBoxes");
                });

            modelBuilder.Entity("MyUni.Models.Entities.Quiz", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("MyUni.Models.Entities.Quiz+Question", b =>
                {
                    b.Navigation("IncorrectAnswers");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard", b =>
                {
                    b.Navigation("ArchevitiSavaldebuloSaganebi");

                    b.Navigation("Events");

                    b.Navigation("Sections");

                    b.Navigation("Sections2");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+ArchevitiSavaldebuloSagani", b =>
                {
                    b.Navigation("ArchevitiSavaldebuloSagnebi");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Section", b =>
                {
                    b.Navigation("ProgramNames");
                });

            modelBuilder.Entity("MyUni.Models.Entities.UniCard+Section2", b =>
                {
                    b.Navigation("SavaldebuloSagnebi");
                });
#pragma warning restore 612, 618
        }
    }
}
