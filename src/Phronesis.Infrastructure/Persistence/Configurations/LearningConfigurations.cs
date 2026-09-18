using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Learning;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class LearnerEnrollmentConfiguration : IEntityTypeConfiguration<LearnerEnrollment>
{
    public void Configure(EntityTypeBuilder<LearnerEnrollment> builder)
    {
        builder.ToTable("LearnerEnrollments");
        builder.HasKey(le => le.Id);

        builder.HasIndex(le => new { le.LearnerId, le.SubjectId }).IsUnique();

        builder.HasOne(le => le.Learner)
            .WithMany()
            .HasForeignKey(le => le.LearnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(le => le.Subject)
            .WithMany()
            .HasForeignKey(le => le.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ContentEngagementConfiguration : IEntityTypeConfiguration<ContentEngagement>
{
    public void Configure(EntityTypeBuilder<ContentEngagement> builder)
    {
        builder.ToTable("ContentEngagements");
        builder.HasKey(ce => ce.Id);

        builder.HasIndex(ce => new { ce.LearnerId, ce.EducationalContentId }).IsUnique();

        builder.HasOne(ce => ce.Learner)
            .WithMany()
            .HasForeignKey(ce => ce.LearnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ce => ce.EducationalContent)
            .WithMany()
            .HasForeignKey(ce => ce.EducationalContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
