using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Learning;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class SubjectProgressConfiguration : IEntityTypeConfiguration<SubjectProgress>
{
    public void Configure(EntityTypeBuilder<SubjectProgress> builder)
    {
        builder.ToTable("SubjectProgresses");
        builder.HasKey(sp => sp.Id);

        builder.HasIndex(sp => new { sp.LearnerId, sp.SubjectId }).IsUnique();

        builder.HasOne(sp => sp.Learner)
            .WithMany()
            .HasForeignKey(sp => sp.LearnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Subject)
            .WithMany()
            .HasForeignKey(sp => sp.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CertificateCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.CertificateCode).IsUnique();

        builder.HasIndex(c => new { c.LearnerId, c.SubjectId }).IsUnique();

        builder.HasOne(c => c.Learner)
            .WithMany()
            .HasForeignKey(c => c.LearnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Subject)
            .WithMany()
            .HasForeignKey(c => c.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
