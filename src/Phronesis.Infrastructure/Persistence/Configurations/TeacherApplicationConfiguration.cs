using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Users;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class TeacherApplicationConfiguration : IEntityTypeConfiguration<TeacherApplication>
{
    public void Configure(EntityTypeBuilder<TeacherApplication> builder)
    {
        builder.ToTable("TeacherApplications");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.AdminNotes)
            .HasMaxLength(2000);

        builder.Property(t => t.InterviewLink)
            .HasMaxLength(1000);

        builder.Property(t => t.InterviewNotes)
            .HasMaxLength(2000);

        builder.HasOne(t => t.TeacherProfile)
            .WithMany()
            .HasForeignKey(t => t.TeacherProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Documents)
            .WithOne(d => d.TeacherApplication)
            .HasForeignKey(d => d.TeacherApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
