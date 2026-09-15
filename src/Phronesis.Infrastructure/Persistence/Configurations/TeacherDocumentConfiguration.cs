using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Users;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class TeacherDocumentConfiguration : IEntityTypeConfiguration<TeacherDocument>
{
    public void Configure(EntityTypeBuilder<TeacherDocument> builder)
    {
        builder.ToTable("TeacherDocuments");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.DocumentType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.FileUri)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(t => t.VerificationStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.RejectionReason)
            .HasMaxLength(1000);
    }
}
