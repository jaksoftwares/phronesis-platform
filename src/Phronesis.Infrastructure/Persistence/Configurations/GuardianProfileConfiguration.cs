using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Users;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class GuardianProfileConfiguration : IEntityTypeConfiguration<GuardianProfile>
{
    public void Configure(EntityTypeBuilder<GuardianProfile> builder)
    {
        builder.ToTable("GuardianProfiles");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(g => g.User)
            .WithOne()
            .HasForeignKey<GuardianProfile>(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
