using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Tuition;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class VirtualClassConfiguration : IEntityTypeConfiguration<VirtualClass>
{
    public void Configure(EntityTypeBuilder<VirtualClass> builder)
    {
        builder.ToTable("VirtualClasses");
        builder.HasKey(vc => vc.Id);

        builder.Property(vc => vc.Name).IsRequired().HasMaxLength(200);
        builder.Property(vc => vc.Description).HasMaxLength(1000);
        builder.Property(vc => vc.Price).HasColumnType("decimal(18,2)");

        builder.HasOne(vc => vc.Teacher)
            .WithMany()
            .HasForeignKey(vc => vc.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(vc => vc.Subject)
            .WithMany()
            .HasForeignKey(vc => vc.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ClassEnrollmentConfiguration : IEntityTypeConfiguration<ClassEnrollment>
{
    public void Configure(EntityTypeBuilder<ClassEnrollment> builder)
    {
        builder.ToTable("ClassEnrollments");
        builder.HasKey(ce => ce.Id);

        // A learner can only be enrolled in a specific class once
        builder.HasIndex(ce => new { ce.LearnerId, ce.VirtualClassId }).IsUnique();

        builder.HasOne(ce => ce.Learner)
            .WithMany()
            .HasForeignKey(ce => ce.LearnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ce => ce.VirtualClass)
            .WithMany(vc => vc.Enrollments)
            .HasForeignKey(ce => ce.VirtualClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    public void Configure(EntityTypeBuilder<ClassSession> builder)
    {
        builder.ToTable("ClassSessions");
        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.Title).IsRequired().HasMaxLength(200);
        builder.Property(cs => cs.MeetingId).HasMaxLength(100);
        builder.Property(cs => cs.MeetingPassword).HasMaxLength(100);
        builder.Property(cs => cs.MeetingLink).HasMaxLength(1000);
        builder.Property(cs => cs.HostUrl).HasMaxLength(1000);
        builder.Property(cs => cs.RecordingUrl).HasMaxLength(1000);
        builder.Property(cs => cs.TeacherNotes).HasMaxLength(4000);

        builder.HasOne(cs => cs.VirtualClass)
            .WithMany(vc => vc.Sessions)
            .HasForeignKey(cs => cs.VirtualClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TeacherAvailabilityConfiguration : IEntityTypeConfiguration<TeacherAvailability>
{
    public void Configure(EntityTypeBuilder<TeacherAvailability> builder)
    {
        builder.ToTable("TeacherAvailabilities");
        builder.HasKey(ta => ta.Id);

        builder.HasOne(ta => ta.Teacher)
            .WithMany()
            .HasForeignKey(ta => ta.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BookingRequestConfiguration : IEntityTypeConfiguration<BookingRequest>
{
    public void Configure(EntityTypeBuilder<BookingRequest> builder)
    {
        builder.ToTable("BookingRequests");
        builder.HasKey(br => br.Id);

        builder.Property(br => br.TeacherNotes).HasMaxLength(1000);

        builder.HasOne(br => br.Learner)
            .WithMany()
            .HasForeignKey(br => br.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(br => br.Teacher)
            .WithMany()
            .HasForeignKey(br => br.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(br => br.Subject)
            .WithMany()
            .HasForeignKey(br => br.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ClassAttendanceConfiguration : IEntityTypeConfiguration<ClassAttendance>
{
    public void Configure(EntityTypeBuilder<ClassAttendance> builder)
    {
        builder.ToTable("ClassAttendances");
        builder.HasKey(ca => ca.Id);

        builder.HasOne(ca => ca.ClassSession)
            .WithMany(cs => cs.Attendances)
            .HasForeignKey(ca => ca.ClassSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SessionFeedbackConfiguration : IEntityTypeConfiguration<SessionFeedback>
{
    public void Configure(EntityTypeBuilder<SessionFeedback> builder)
    {
        builder.ToTable("SessionFeedbacks");
        builder.HasKey(sf => sf.Id);

        builder.Property(sf => sf.WhatWentWell).HasMaxLength(1000);
        builder.Property(sf => sf.AreasForImprovement).HasMaxLength(1000);
        builder.Property(sf => sf.Complaints).HasMaxLength(1000);

        builder.HasOne(sf => sf.ClassSession)
            .WithMany(cs => cs.Feedbacks)
            .HasForeignKey(sf => sf.ClassSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
