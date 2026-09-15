using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Users;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class TeacherProfileConfiguration : IEntityTypeConfiguration<TeacherProfile>
{
    public void Configure(EntityTypeBuilder<TeacherProfile> builder)
    {
        builder.ToTable("TeacherProfiles");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Bio)
            .HasMaxLength(1000);

        builder.Property(t => t.Qualifications)
            .HasMaxLength(1000);

        builder.Property(t => t.TeachingSkills)
            .HasMaxLength(1000);

        builder.Property(t => t.VerificationState)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(t => t.User)
            .WithOne()
            .HasForeignKey<TeacherProfile>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
