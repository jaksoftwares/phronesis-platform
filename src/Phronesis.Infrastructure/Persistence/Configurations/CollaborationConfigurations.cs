using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Collaboration;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class ClassResourceConfiguration : IEntityTypeConfiguration<ClassResource>
{
    public void Configure(EntityTypeBuilder<ClassResource> builder)
    {
        builder.ToTable("ClassResources");
        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Title).IsRequired().HasMaxLength(200);
        builder.Property(cr => cr.Description).HasMaxLength(1000);
        builder.Property(cr => cr.FileUrl).IsRequired().HasMaxLength(1000);
        builder.Property(cr => cr.MimeType).IsRequired().HasMaxLength(100);

        builder.HasOne(cr => cr.VirtualClass)
            .WithMany()
            .HasForeignKey(cr => cr.VirtualClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.Teacher)
            .WithMany()
            .HasForeignKey(cr => cr.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(cr => cr.ClassSession)
            .WithMany()
            .HasForeignKey(cr => cr.ClassSessionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ClassDiscussionConfiguration : IEntityTypeConfiguration<ClassDiscussion>
{
    public void Configure(EntityTypeBuilder<ClassDiscussion> builder)
    {
        builder.ToTable("ClassDiscussions");
        builder.HasKey(cd => cd.Id);

        builder.Property(cd => cd.Title).IsRequired().HasMaxLength(200);
        builder.Property(cd => cd.Content).IsRequired().HasMaxLength(4000);

        builder.HasOne(cd => cd.VirtualClass)
            .WithMany()
            .HasForeignKey(cd => cd.VirtualClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cd => cd.Author)
            .WithMany()
            .HasForeignKey(cd => cd.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DiscussionReplyConfiguration : IEntityTypeConfiguration<DiscussionReply>
{
    public void Configure(EntityTypeBuilder<DiscussionReply> builder)
    {
        builder.ToTable("DiscussionReplies");
        builder.HasKey(dr => dr.Id);

        builder.Property(dr => dr.Content).IsRequired().HasMaxLength(4000);

        builder.HasOne(dr => dr.ClassDiscussion)
            .WithMany(cd => cd.Replies)
            .HasForeignKey(dr => dr.ClassDiscussionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dr => dr.Author)
            .WithMany()
            .HasForeignKey(dr => dr.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
