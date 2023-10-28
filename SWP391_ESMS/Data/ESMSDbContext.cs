using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Models.Domain;

namespace SWP391_ESMS.Data;

public partial class ESMSDbContext : DbContext
{
    public ESMSDbContext()
    {
    }

    public ESMSDbContext(DbContextOptions<ESMSDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConfigurationSetting> ConfigurationSettings { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<ExamRoom> ExamRooms { get; set; }

    public virtual DbSet<ExamSession> ExamSessions { get; set; }

    public virtual DbSet<ExamShift> ExamShifts { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseLazyLoadingProxies();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigurationSetting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__Configur__54372AFDB8267C36");

            entity.Property(e => e.SettingId)
                .ValueGeneratedNever()
                .HasColumnName("SettingID");
            entity.Property(e => e.SettingDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SettingName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SettingValue).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187FE47DA8E");

            entity.Property(e => e.CourseId)
                .ValueGeneratedNever()
                .HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MajorId).HasColumnName("MajorID");

            entity.HasOne(d => d.Major).WithMany(p => p.Courses)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__Courses__MajorID__4BAC3F29");
        });

        modelBuilder.Entity<ExamRoom>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__ExamRoom__3286391993DDE3C0");

            entity.Property(e => e.RoomId)
                .ValueGeneratedNever()
                .HasColumnName("RoomID");
            entity.Property(e => e.RoomName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ExamSession>(entity =>
        {
            entity.HasKey(e => e.ExamSessionId).HasName("PK__ExamSess__85F7FBB0A95AE512");

            entity.Property(e => e.ExamSessionId)
                .ValueGeneratedNever()
                .HasColumnName("ExamSessionID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.ExamDate).HasColumnType("date");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.ShiftId).HasColumnName("ShiftID");
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Course).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__ExamSessi__Cours__5DCAEF64");

            entity.HasOne(d => d.Room).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__ExamSessi__RoomI__5EBF139D");

            entity.HasOne(d => d.Shift).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("FK__ExamSessi__Shift__619B8048");

            entity.HasOne(d => d.Staff).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__ExamSessi__Staff__60A75C0F");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__ExamSessi__Teach__5FB337D6");

            entity.HasMany(d => d.Students).WithMany(p => p.ExamSessions)
                .UsingEntity<Dictionary<string, object>>(
                    "ExamEnrollment",
                    r => r.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ExamEnrol__Stude__656C112C"),
                    l => l.HasOne<ExamSession>().WithMany()
                        .HasForeignKey("ExamSessionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ExamEnrol__ExamS__6477ECF3"),
                    j =>
                    {
                        j.HasKey("ExamSessionId", "StudentId").HasName("PK__ExamEnro__06DBA917A0E204D4");
                        j.ToTable("ExamEnrollments");
                        j.IndexerProperty<Guid>("ExamSessionId").HasColumnName("ExamSessionID");
                        j.IndexerProperty<Guid>("StudentId").HasColumnName("StudentID");
                    });
        });

        modelBuilder.Entity<ExamShift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__ExamShif__C0A838E12CA7FD5D");

            entity.Property(e => e.ShiftId)
                .ValueGeneratedNever()
                .HasColumnName("ShiftID");
            entity.Property(e => e.ShiftName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.MajorId).HasName("PK__Majors__D5B8BFB1261CF639");

            entity.Property(e => e.MajorId)
                .ValueGeneratedNever()
                .HasColumnName("MajorID");
            entity.Property(e => e.MajorName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__96D4AAF743EFCD1A");

            entity.Property(e => e.StaffId)
                .ValueGeneratedNever()
                .HasColumnName("StaffID");
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HomeAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicture).HasMaxLength(255);
            entity.Property(e => e.StaffRole)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79AAB225DA");

            entity.Property(e => e.StudentId)
                .ValueGeneratedNever()
                .HasColumnName("StudentID");
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HomeAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicture).HasMaxLength(255);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Major).WithMany(p => p.Students)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__Students__MajorI__4E88ABD4");

            entity.HasMany(d => d.Courses).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseEnrollment",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseEnr__Cours__52593CB8"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CourseEnr__Stude__5165187F"),
                    j =>
                    {
                        j.HasKey("StudentId", "CourseId").HasName("PK__CourseEn__5E57FD61DC0F28A2");
                        j.ToTable("CourseEnrollments");
                        j.IndexerProperty<Guid>("StudentId").HasColumnName("StudentID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF25944066FA3E2");

            entity.Property(e => e.TeacherId)
                .ValueGeneratedNever()
                .HasColumnName("TeacherID");
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HomeAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MajorId).HasColumnName("MajorID");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicture).HasMaxLength(255);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Major).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK__Teachers__MajorI__571DF1D5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
