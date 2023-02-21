using Domain.Entities;
using Infrastructure.Converter;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Teacher>? Teachers { get; set; }
        public DbSet<Branch>? Branches { get; set; }
        public DbSet<Class>? Classes { get; set; }
        public DbSet<Student>? Students { get; set; }
        public DbSet<ClassStudent>? ClassStudents { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Course>? Courses { get; set; }
        public DbSet<Lesson>? Lessons { get; set; }
        public DbSet<Attendance>? Attendances { get; set; }
        public DbSet<RefreshToken>? RefreshTokens { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public IEnumerable<string> GetAllId() => Users.Select(u => u.Id);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.userId)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.token)
                    .HasColumnType("varchar(max)");
                entity.Property(e => e.jwtId)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.isUsed)
                    .HasColumnType("bit");
                entity.Property(e => e.isRevoked)
                    .HasColumnType("bit");
                entity.Property(e => e.addedDate)
                    .HasColumnType("datetime2");
                entity.Property(e => e.expiryDate)
                    .HasColumnType("datetime2");
            });

            builder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teachers");
                entity.HasKey(e => e.id);
                entity.Property(e => e.fullName)
                    .HasColumnType("nvarchar(50)")
                    .IsRequired();
                entity.Property(e => e.email)
                    .HasColumnType("varchar(50)")
                    .IsRequired();
                entity.Property(e => e.phone)
                    .HasColumnType("char(10)");
                entity.HasOne(s => s.customer)
                    .WithMany(g => g.teachers)
                    .HasForeignKey(s => s.customerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branches");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id)
                    .HasColumnType("char(5)");
                entity.Property(e => e.name)
                    .HasColumnType("nvarchar(50)")
                    .IsRequired();
                entity.Property(e => e.address)
                    .HasColumnType("nvarchar(80)");
            });


            builder.Entity<Class>(entity =>
            {
                entity.ToTable("Classes");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id)
                    .HasColumnType("char(7)");
                entity.Property(e => e.name)
                    .HasColumnType("nvarchar(25)")
                    .IsRequired();
                entity.Property(e => e.startDate)
                    .HasConversion<StringConverter>()
                    .HasColumnType("date")
                    .IsRequired();
                entity.Property(e => e.endDate)
                    .HasConversion<StringConverter>()
                    .HasColumnType("date")
                    .IsRequired();
                entity.HasOne(s => s.teacher)
                    .WithMany(g => g.classes)
                    .HasForeignKey(s => s.teacherId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(s => s.branch)
                    .WithMany(g => g.classes)
                    .HasForeignKey(s => s.branchId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(e => e.id);
                entity.Property(e => e.fullName)
                    .HasColumnType("nvarchar(50)")
                    .IsRequired();
                entity.Property(e => e.dob)
                    .HasConversion<StringConverter>()
                    .HasColumnType("date");
                entity.Property(e => e.email)
                    .HasColumnType("varchar(50)")
                    .IsRequired();
                entity.Property(e => e.phone)
                    .HasColumnType("char(10)");
            });

            builder.Entity<ClassStudent>(entity =>
            {
                entity.ToTable("ClassStudents");
                entity.HasKey(e => new { e.classId, e.studentId });
                entity.HasOne(s => s.@class)
                    .WithMany(g => g.classStudents)
                    .HasForeignKey(s => s.classId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(s => s.student)
                    .WithMany(g => g.classStudents)
                    .HasForeignKey(s => s.studentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id)
                    .HasColumnType("char(5)");
                entity.Property(e => e.name)
                    .HasColumnType("nvarchar(25)")
                    .IsRequired();
            });

            builder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id)
                    .HasColumnType("char(5)");
                entity.Property(e => e.name)
                    .HasColumnType("nvarchar(20)")
                    .IsRequired();
                entity.Property(e => e.description)
                    .HasColumnType("nvarchar(max)");
                entity.HasOne(s => s.category)
                    .WithMany(g => g.courses)
                    .HasForeignKey(s => s.categoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Lesson>(entity =>
            {
                entity.ToTable("Lessons");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id)
                    .HasColumnType("char(10)");
                entity.Property(e => e.name)
                    .HasColumnType("nvarchar(50)")
                    .IsRequired();
                entity.Property(e => e.description)
                    .HasColumnType("nvarchar(max)");
                entity.HasOne(s => s.course)
                    .WithMany(g => g.lessons)
                    .HasForeignKey(s => s.courseId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Attendance>(entity =>
            {
                entity.ToTable("Attendance");
                entity.HasKey(e => new { e.classId, e.lessonId });
                entity.Property(e => e.date)
                    .HasConversion<StringConverter>()
                    .HasColumnType("date")
                    .IsRequired();
                entity.Property(e => e.comment)
                    .HasColumnType("nvarchar(70)");
                entity.Property(e => e.evaluation)
                    .HasColumnType("tinyint");
                entity.Property(e => e.attendance)
                    .HasColumnType("tinyint");
                entity.HasOne(s => s.lesson)
                    .WithMany(g => g.attendances)
                    .HasForeignKey(s => s.lessonId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.@class)
                    .WithMany(g => g.attendances)
                    .HasForeignKey(s => s.classId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
