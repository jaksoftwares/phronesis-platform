using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Users;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class LearnerProfileConfiguration : IEntityTypeConfiguration<LearnerProfile>
{
    public void Configure(EntityTypeBuilder<LearnerProfile> builder)
    {
        builder.ToTable("LearnerProfiles");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.SchoolName)
            .HasMaxLength(150);

        builder.HasOne(l => l.User)
            .WithOne()
            .HasForeignKey<LearnerProfile>(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.GradeLevel)
            .WithMany()
            .HasForeignKey(l => l.GradeLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
