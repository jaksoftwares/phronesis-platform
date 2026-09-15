using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Organization;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class StaffProfileConfiguration : IEntityTypeConfiguration<StaffProfile>
{
    public void Configure(EntityTypeBuilder<StaffProfile> builder)
    {
        builder.ToTable("StaffProfiles");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.EmployeeId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.EmployeeId)
            .IsUnique();

        builder.Property(s => s.Department)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.JobTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(s => s.User)
            .WithOne(u => u.StaffProfile)
            .HasForeignKey<StaffProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
