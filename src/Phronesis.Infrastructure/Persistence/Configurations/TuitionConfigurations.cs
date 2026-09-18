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
        builder.Property(cs => cs.MeetingLink).HasMaxLength(500);

        builder.HasOne(cs => cs.VirtualClass)
            .WithMany(vc => vc.Sessions)
            .HasForeignKey(cs => cs.VirtualClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
