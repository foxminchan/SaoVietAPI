﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.RefreshToken", b =>
            {
                b.Property<long>("id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("id"));

                b.Property<DateTime?>("addedDate")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("expiryDate")
                    .HasColumnType("datetime2");

                b.Property<bool>("isRevoked")
                    .HasColumnType("bit");

                b.Property<bool>("isUsed")
                    .HasColumnType("bit");

                b.Property<string>("jwtId")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("token")
                    .HasColumnType("varchar(max)");

                b.Property<string>("userId")
                    .HasColumnType("nvarchar(450)");

                b.HasKey("id");

                b.ToTable("RefreshTokens", (string)null);
            });

            modelBuilder.Entity("Domain.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Attendance", b =>
                {
                    b.Property<string>("classId")
                        .HasColumnType("char(7)");

                    b.Property<string>("lessonId")
                        .HasColumnType("char(10)");

                    b.Property<byte?>("attendance")
                        .HasColumnType("tinyint");

                    b.Property<string>("comment")
                        .HasColumnType("nvarchar(70)");

                    b.Property<DateTime>("date")
                        .HasColumnType("date");

                    b.Property<byte?>("evaluation")
                        .HasColumnType("tinyint");

                    b.HasKey("classId", "lessonId");

                    b.HasIndex("lessonId");

                    b.ToTable("Attendance", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Branch", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("char(5)");

                    b.Property<string>("address")
                        .HasColumnType("nvarchar(80)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("id");

                    b.ToTable("Branches", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Category", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("char(5)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("id");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Class", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("char(7)");

                    b.Property<string>("branchId")
                        .HasColumnType("char(5)");

                    b.Property<DateTime>("endDate")
                        .HasColumnType("date");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<DateTime>("startDate")
                        .HasColumnType("date");

                    b.Property<Guid?>("teacherId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("id");

                    b.HasIndex("branchId");

                    b.HasIndex("teacherId");

                    b.ToTable("Classes", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ClassStudent", b =>
                {
                    b.Property<string>("classId")
                        .HasColumnType("char(7)");

                    b.Property<Guid?>("studentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("classId", "studentId");

                    b.HasIndex("studentId");

                    b.ToTable("ClassStudents", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Course", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("char(5)");

                    b.Property<string>("categoryId")
                        .HasColumnType("char(5)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("id");

                    b.HasIndex("categoryId");

                    b.ToTable("Courses", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Lesson", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("char(10)");

                    b.Property<string>("courseId")
                        .HasColumnType("char(5)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("id");

                    b.HasIndex("courseId");

                    b.ToTable("Lessons", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Student", b =>
                {
                    b.Property<Guid?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("dob")
                        .HasColumnType("date");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("fullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("phone")
                        .HasColumnType("char(10)");

                    b.HasKey("id");

                    b.ToTable("Students", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Teacher", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("customerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("fullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("phone")
                        .HasColumnType("char(10)");

                    b.HasKey("id");

                    b.HasIndex("customerId");

                    b.ToTable("Teachers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Attendance", b =>
                {
                    b.HasOne("Domain.Entities.Class", "class")
                        .WithMany("attendances")
                        .HasForeignKey("classId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Lesson", "lesson")
                        .WithMany("attendances")
                        .HasForeignKey("lessonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("class");

                    b.Navigation("lesson");
                });

            modelBuilder.Entity("Domain.Entities.Class", b =>
                {
                    b.HasOne("Domain.Entities.Branch", "branch")
                        .WithMany("classes")
                        .HasForeignKey("branchId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Domain.Entities.Teacher", "teacher")
                        .WithMany("classes")
                        .HasForeignKey("teacherId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("branch");

                    b.Navigation("teacher");
                });

            modelBuilder.Entity("Domain.Entities.ClassStudent", b =>
                {
                    b.HasOne("Domain.Entities.Class", "class")
                        .WithMany("classStudents")
                        .HasForeignKey("classId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Student", "student")
                        .WithMany("classStudents")
                        .HasForeignKey("studentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("class");

                    b.Navigation("student");
                });

            modelBuilder.Entity("Domain.Entities.Course", b =>
                {
                    b.HasOne("Domain.Entities.Category", "category")
                        .WithMany("courses")
                        .HasForeignKey("categoryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("category");
                });

            modelBuilder.Entity("Domain.Entities.Lesson", b =>
                {
                    b.HasOne("Domain.Entities.Course", "course")
                        .WithMany("lessons")
                        .HasForeignKey("courseId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("course");
                });

            modelBuilder.Entity("Domain.Entities.Teacher", b =>
                {
                    b.HasOne("Domain.Entities.ApplicationUser", "customer")
                        .WithMany("teachers")
                        .HasForeignKey("customerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("customer");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.ApplicationUser", b =>
                {
                    b.Navigation("teachers");
                });

            modelBuilder.Entity("Domain.Entities.Branch", b =>
                {
                    b.Navigation("classes");
                });

            modelBuilder.Entity("Domain.Entities.Category", b =>
                {
                    b.Navigation("courses");
                });

            modelBuilder.Entity("Domain.Entities.Class", b =>
                {
                    b.Navigation("attendances");

                    b.Navigation("classStudents");
                });

            modelBuilder.Entity("Domain.Entities.Course", b =>
                {
                    b.Navigation("lessons");
                });

            modelBuilder.Entity("Domain.Entities.Lesson", b =>
                {
                    b.Navigation("attendances");
                });

            modelBuilder.Entity("Domain.Entities.Student", b =>
                {
                    b.Navigation("classStudents");
                });

            modelBuilder.Entity("Domain.Entities.Teacher", b =>
                {
                    b.Navigation("classes");
                });
#pragma warning restore 612, 618
        }
    }
}
