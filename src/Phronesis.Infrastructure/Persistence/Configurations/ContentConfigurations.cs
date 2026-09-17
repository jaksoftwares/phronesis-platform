using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Content;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class EducationalContentConfiguration : IEntityTypeConfiguration<EducationalContent>
{
    public void Configure(EntityTypeBuilder<EducationalContent> builder)
    {
        builder.ToTable("EducationalContents");
        builder.HasKey(ec => ec.Id);

        builder.Property(ec => ec.Title).IsRequired().HasMaxLength(255);
        builder.Property(ec => ec.Description).IsRequired().HasMaxLength(2000);
        builder.Property(ec => ec.Version).IsRequired().HasMaxLength(50);
        builder.Property(ec => ec.ContentType).HasConversion<string>().IsRequired();
        builder.Property(ec => ec.Status).HasConversion<string>().IsRequired();

        // Authorship
        builder.HasOne(ec => ec.Author)
            .WithMany()
            .HasForeignKey(ec => ec.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Required Taxonomy Tags
        builder.HasOne(ec => ec.GradeLevel)
            .WithMany()
            .HasForeignKey(ec => ec.GradeLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ec => ec.Subject)
            .WithMany()
            .HasForeignKey(ec => ec.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional Taxonomy Tags
        builder.HasOne(ec => ec.Strand)
            .WithMany()
            .HasForeignKey(ec => ec.StrandId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ec => ec.SubStrand)
            .WithMany()
            .HasForeignKey(ec => ec.SubStrandId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ec => ec.LearningObjective)
            .WithMany()
            .HasForeignKey(ec => ec.LearningObjectiveId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ContentTagConfiguration : IEntityTypeConfiguration<ContentTag>
{
    public void Configure(EntityTypeBuilder<ContentTag> builder)
    {
        builder.ToTable("ContentTags");
        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.Name).IsRequired().HasMaxLength(100);

        builder.HasIndex(ct => new { ct.EducationalContentId, ct.Name }).IsUnique();

        builder.HasOne(ct => ct.EducationalContent)
            .WithMany(ec => ec.Tags)
            .HasForeignKey(ct => ct.EducationalContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ContentReviewConfiguration : IEntityTypeConfiguration<ContentReview>
{
    public void Configure(EntityTypeBuilder<ContentReview> builder)
    {
        builder.ToTable("ContentReviews");
        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Outcome).HasConversion<string>().IsRequired();
        builder.Property(cr => cr.Feedback).IsRequired().HasMaxLength(2000);

        builder.HasOne(cr => cr.EducationalContent)
            .WithMany(ec => ec.Reviews)
            .HasForeignKey(cr => cr.EducationalContentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cr => cr.Reviewer)
            .WithMany()
            .HasForeignKey(cr => cr.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ContentAttachmentConfiguration : IEntityTypeConfiguration<ContentAttachment>
{
    public void Configure(EntityTypeBuilder<ContentAttachment> builder)
    {
        builder.ToTable("ContentAttachments");
        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.FileName).IsRequired().HasMaxLength(255);
        builder.Property(ca => ca.FileUri).IsRequired().HasMaxLength(1000);
        builder.Property(ca => ca.MimeType).IsRequired().HasMaxLength(100);

        builder.HasOne(ca => ca.EducationalContent)
            .WithMany(ec => ec.Attachments)
            .HasForeignKey(ca => ca.EducationalContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SavedContentConfiguration : IEntityTypeConfiguration<SavedContent>
{
    public void Configure(EntityTypeBuilder<SavedContent> builder)
    {
        builder.ToTable("SavedContents");
        builder.HasKey(sc => sc.Id);

        builder.HasIndex(sc => new { sc.UserId, sc.EducationalContentId }).IsUnique();

        builder.HasOne(sc => sc.User)
            .WithMany()
            .HasForeignKey(sc => sc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sc => sc.EducationalContent)
            .WithMany()
            .HasForeignKey(sc => sc.EducationalContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
