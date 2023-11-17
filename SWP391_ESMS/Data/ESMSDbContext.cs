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

    public virtual DbSet<ExamFormat> ExamFormats { get; set; }

    public virtual DbSet<ExamPeriod> ExamPeriods { get; set; }

    public virtual DbSet<ExamRoom> ExamRooms { get; set; }

    public virtual DbSet<ExamSession> ExamSessions { get; set; }

    public virtual DbSet<ExamShift> ExamShifts { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<ProctoringRequest> ProctoringRequests { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseLazyLoadingProxies();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigurationSetting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__Configur__54372AFD05EB8AC1");

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
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187A5EA4B88");

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

        modelBuilder.Entity<ExamFormat>(entity =>
        {
            entity.HasKey(e => e.ExamFormatId).HasName("PK__ExamForm__5A9D36C8849EAECB");

            entity.Property(e => e.ExamFormatId)
                .ValueGeneratedNever()
                .HasColumnName("ExamFormatID");
            entity.Property(e => e.ExamFormatCode)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ExamFormatName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasMany(d => d.Courses).WithMany(p => p.ExamFormats)
                .UsingEntity<Dictionary<string, object>>(
                    "ExamFormatCourseAssociation",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK__ExamForma__Cours__60A75C0F"),
                    l => l.HasOne<ExamFormat>().WithMany()
                        .HasForeignKey("ExamFormatId")
                        .HasConstraintName("FK__ExamForma__ExamF__5FB337D6"),
                    j =>
                    {
                        j.HasKey("ExamFormatId", "CourseId").HasName("PK__ExamForm__360FE1D05C5D6EAD");
                        j.ToTable("ExamFormatCourseAssociations");
                        j.IndexerProperty<Guid>("ExamFormatId").HasColumnName("ExamFormatID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<ExamPeriod>(entity =>
        {
            entity.HasKey(e => e.ExamPeriodId).HasName("PK__ExamPeri__C88981B5B24A49C4");

            entity.Property(e => e.ExamPeriodId)
                .ValueGeneratedNever()
                .HasColumnName("ExamPeriodID");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.ExamFormatId).HasColumnName("ExamFormatID");
            entity.Property(e => e.ExamPeriodName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("date");

            entity.HasOne(d => d.ExamFormat).WithMany(p => p.ExamPeriods)
                .HasForeignKey(d => d.ExamFormatId)
                .HasConstraintName("FK__ExamPerio__ExamF__6383C8BA");
        });

        modelBuilder.Entity<ExamRoom>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__ExamRoom__328639190BA03466");

            entity.Property(e => e.RoomId)
                .ValueGeneratedNever()
                .HasColumnName("RoomID");
            entity.Property(e => e.RoomName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ExamSession>(entity =>
        {
            entity.HasKey(e => e.ExamSessionId).HasName("PK__ExamSess__85F7FBB001B51A79");

            entity.Property(e => e.ExamSessionId)
                .ValueGeneratedNever()
                .HasColumnName("ExamSessionID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.ExamDate).HasColumnType("date");
            entity.Property(e => e.ExamPeriodId).HasColumnName("ExamPeriodID");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.ShiftId).HasColumnName("ShiftID");
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Course).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__ExamSessi__Cours__66603565");

            entity.HasOne(d => d.ExamPeriod).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.ExamPeriodId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ExamSessi__ExamP__6754599E");

            entity.HasOne(d => d.Room).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__ExamSessi__RoomI__693CA210");

            entity.HasOne(d => d.Shift).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("FK__ExamSessi__Shift__68487DD7");

            entity.HasOne(d => d.Staff).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__ExamSessi__Staff__6B24EA82");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__ExamSessi__Teach__6A30C649");

            entity.HasMany(d => d.Students).WithMany(p => p.ExamSessions)
                .UsingEntity<Dictionary<string, object>>(
                    "ExamEnrollment",
                    r => r.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .HasConstraintName("FK__ExamEnrol__Stude__6EF57B66"),
                    l => l.HasOne<ExamSession>().WithMany()
                        .HasForeignKey("ExamSessionId")
                        .HasConstraintName("FK__ExamEnrol__ExamS__6E01572D"),
                    j =>
                    {
                        j.HasKey("ExamSessionId", "StudentId").HasName("PK__ExamEnro__06DBA917D655592B");
                        j.ToTable("ExamEnrollments", tb =>
                        {
                            tb.HasTrigger("trg_StudentEnrollmentAfterDelete");
                            tb.HasTrigger("trg_StudentEnrollmentAfterInsert");
                        });
                        j.IndexerProperty<Guid>("ExamSessionId").HasColumnName("ExamSessionID");
                        j.IndexerProperty<Guid>("StudentId").HasColumnName("StudentID");
                    });
        });

        modelBuilder.Entity<ExamShift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__ExamShif__C0A838E16A24F9C1");

            entity.Property(e => e.ShiftId)
                .ValueGeneratedNever()
                .HasColumnName("ShiftID");
            entity.Property(e => e.ShiftName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.MajorId).HasName("PK__Majors__D5B8BFB1FE4EBED3");

            entity.Property(e => e.MajorId)
                .ValueGeneratedNever()
                .HasColumnName("MajorID");
            entity.Property(e => e.MajorName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProctoringRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Proctori__33A8519A9C721ECF");

            entity.Property(e => e.RequestId)
                .ValueGeneratedNever()
                .HasColumnName("RequestID");
            entity.Property(e => e.ExamSessionId).HasColumnName("ExamSessionID");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.RequestType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.ExamSession).WithMany(p => p.ProctoringRequests)
                .HasForeignKey(d => d.ExamSessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Proctorin__ExamS__71D1E811");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ProctoringRequests)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Proctorin__Teach__72C60C4A");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__96D4AAF700A38545");

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
            entity.Property(e => e.StaffRole)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A7955DF973A");

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
                        .HasConstraintName("FK__CourseEnr__Cours__52593CB8"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .HasConstraintName("FK__CourseEnr__Stude__5165187F"),
                    j =>
                    {
                        j.HasKey("StudentId", "CourseId").HasName("PK__CourseEn__5E57FD614A84A4BE");
                        j.ToTable("CourseEnrollments");
                        j.IndexerProperty<Guid>("StudentId").HasColumnName("StudentID");
                        j.IndexerProperty<Guid>("CourseId").HasColumnName("CourseID");
                    });
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF25944B469D6D9");

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
