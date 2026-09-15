using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Identity;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");

        builder.HasKey(us => us.Id);

        builder.Property(us => us.RefreshToken)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(us => us.RefreshToken)
            .IsUnique();

        builder.Property(us => us.DeviceInfo)
            .HasMaxLength(200);

        builder.Property(us => us.IpAddress)
            .HasMaxLength(50);

        builder.HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
