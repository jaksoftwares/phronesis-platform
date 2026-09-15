using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Academic;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class CurriculumConfiguration : IEntityTypeConfiguration<Curriculum>
{
    public void Configure(EntityTypeBuilder<Curriculum> builder)
    {
        builder.ToTable("Curricula");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Version).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Description).HasMaxLength(1000);
    }
}

public class EducationLevelConfiguration : IEntityTypeConfiguration<EducationLevel>
{
    public void Configure(EntityTypeBuilder<EducationLevel> builder)
    {
        builder.ToTable("EducationLevels");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);

        builder.HasOne(e => e.Curriculum)
            .WithMany()
            .HasForeignKey(e => e.CurriculumId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects");
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Code).HasMaxLength(50);
        builder.Property(s => s.Description).HasMaxLength(1000);
    }
}

public class StrandConfiguration : IEntityTypeConfiguration<Strand>
{
    public void Configure(EntityTypeBuilder<Strand> builder)
    {
        builder.ToTable("Strands");
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);

        builder.HasOne(s => s.Subject)
            .WithMany()
            .HasForeignKey(s => s.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SubStrandConfiguration : IEntityTypeConfiguration<SubStrand>
{
    public void Configure(EntityTypeBuilder<SubStrand> builder)
    {
        builder.ToTable("SubStrands");
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);

        builder.HasOne(s => s.Strand)
            .WithMany()
            .HasForeignKey(s => s.StrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LearningObjectiveConfiguration : IEntityTypeConfiguration<LearningObjective>
{
    public void Configure(EntityTypeBuilder<LearningObjective> builder)
    {
        builder.ToTable("LearningObjectives");
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Description).IsRequired().HasMaxLength(1000);

        builder.HasOne(l => l.SubStrand)
            .WithMany()
            .HasForeignKey(l => l.SubStrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
