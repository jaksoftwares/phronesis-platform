using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Users;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class LearnerGuardianConfiguration : IEntityTypeConfiguration<LearnerGuardian>
{
    public void Configure(EntityTypeBuilder<LearnerGuardian> builder)
    {
        builder.ToTable("LearnerGuardians");
        builder.HasKey(lg => new { lg.LearnerProfileId, lg.GuardianProfileId });

        builder.Property(lg => lg.RelationshipType)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(lg => lg.LearnerProfile)
            .WithMany(l => l.Guardians)
            .HasForeignKey(lg => lg.LearnerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lg => lg.GuardianProfile)
            .WithMany(g => g.Learners)
            .HasForeignKey(lg => lg.GuardianProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
