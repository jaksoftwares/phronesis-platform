using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Learning;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.ToTable("Assessments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title).IsRequired().HasMaxLength(255);
        builder.Property(a => a.Description).HasMaxLength(1000);

        builder.HasOne(a => a.Subject)
            .WithMany()
            .HasForeignKey(a => a.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Questions)
            .WithOne(q => q.Assessment)
            .HasForeignKey(q => q.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Text).IsRequired().HasMaxLength(2000);

        builder.HasMany(q => q.Options)
            .WithOne(qo => qo.Question)
            .HasForeignKey(qo => qo.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");
        builder.HasKey(qo => qo.Id);

        builder.Property(qo => qo.Text).IsRequired().HasMaxLength(1000);
    }
}

public class AssessmentAttemptConfiguration : IEntityTypeConfiguration<AssessmentAttempt>
{
    public void Configure(EntityTypeBuilder<AssessmentAttempt> builder)
    {
        builder.ToTable("AssessmentAttempts");
        builder.HasKey(aa => aa.Id);

        builder.HasOne(aa => aa.Learner)
            .WithMany()
            .HasForeignKey(aa => aa.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(aa => aa.Assessment)
            .WithMany()
            .HasForeignKey(aa => aa.AssessmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(aa => aa.Answers)
            .WithOne(a => a.Attempt)
            .HasForeignKey(a => a.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
{
    public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
    {
        builder.ToTable("AttemptAnswers");
        builder.HasKey(aa => aa.Id);

        builder.HasOne(aa => aa.Question)
            .WithMany()
            .HasForeignKey(aa => aa.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(aa => aa.SelectedOption)
            .WithMany()
            .HasForeignKey(aa => aa.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
