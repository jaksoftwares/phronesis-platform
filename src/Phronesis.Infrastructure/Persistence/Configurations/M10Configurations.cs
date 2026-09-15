using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Academic;
using Phronesis.Domain.Users;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class GradeSubjectConfiguration : IEntityTypeConfiguration<GradeSubject>
{
    public void Configure(EntityTypeBuilder<GradeSubject> builder)
    {
        builder.ToTable("GradeSubjects");
        builder.HasKey(gs => gs.Id);
        
        builder.HasIndex(gs => new { gs.GradeLevelId, gs.SubjectId }).IsUnique();

        builder.HasOne(gs => gs.GradeLevel)
            .WithMany()
            .HasForeignKey(gs => gs.GradeLevelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gs => gs.Subject)
            .WithMany()
            .HasForeignKey(gs => gs.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SubStrandPrerequisiteConfiguration : IEntityTypeConfiguration<SubStrandPrerequisite>
{
    public void Configure(EntityTypeBuilder<SubStrandPrerequisite> builder)
    {
        builder.ToTable("SubStrandPrerequisites");
        builder.HasKey(sp => sp.Id);

        builder.HasIndex(sp => new { sp.SubStrandId, sp.PrerequisiteId }).IsUnique();

        builder.HasOne(sp => sp.SubStrand)
            .WithMany()
            .HasForeignKey(sp => sp.SubStrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sp => sp.Prerequisite)
            .WithMany()
            .HasForeignKey(sp => sp.PrerequisiteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TeacherCompetenceConfiguration : IEntityTypeConfiguration<TeacherCompetence>
{
    public void Configure(EntityTypeBuilder<TeacherCompetence> builder)
    {
        builder.ToTable("TeacherCompetences");
        builder.HasKey(tc => tc.Id);

        builder.HasIndex(tc => new { tc.TeacherProfileId, tc.SubjectId, tc.GradeLevelId }).IsUnique();

        builder.HasOne(tc => tc.TeacherProfile)
            .WithMany()
            .HasForeignKey(tc => tc.TeacherProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tc => tc.Subject)
            .WithMany()
            .HasForeignKey(tc => tc.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tc => tc.GradeLevel)
            .WithMany()
            .HasForeignKey(tc => tc.GradeLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
