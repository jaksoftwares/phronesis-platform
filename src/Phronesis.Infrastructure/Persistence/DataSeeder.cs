using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Academic;

namespace Phronesis.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IApplicationDbContext context)
    {
        // If Curriculum or GradeLevels already exist, we skip seeding to avoid duplicates.
        // Seed scripts should only set up an empty database.
        if (await context.Curricula.AnyAsync() || await context.GradeLevels.AnyAsync())
        {
            return;
        }

        // 1. Curriculum
        var cbcCurriculum = new Curriculum("Standard CBC", "1.0", "Kenyan Competency-Based Curriculum for Junior and Senior School");
        context.Curricula.Add(cbcCurriculum);
        await context.SaveChangesAsync(CancellationToken.None);

        // 2. Education Levels
        var juniorSchool = new EducationLevel(cbcCurriculum.Id, "Junior School", "Grades 7 to 9", 1);
        var seniorSchool = new EducationLevel(cbcCurriculum.Id, "Senior School", "Grades 10 to 12", 2);
        context.EducationLevels.AddRange(juniorSchool, seniorSchool);
        await context.SaveChangesAsync(CancellationToken.None);

        // 3. Grade Levels

        var grade7 = new GradeLevel("Grade 7", "Junior School Year 1", 7);
        grade7.SetEducationLevel(juniorSchool.Id);
        
        var grade8 = new GradeLevel("Grade 8", "Junior School Year 2", 8);
        grade8.SetEducationLevel(juniorSchool.Id);
        
        var grade9 = new GradeLevel("Grade 9", "Junior School Year 3", 9);
        grade9.SetEducationLevel(juniorSchool.Id);

        var grade10 = new GradeLevel("Grade 10", "Senior School Year 1", 10);
        grade10.SetEducationLevel(seniorSchool.Id);
        
        var grade11 = new GradeLevel("Grade 11", "Senior School Year 2", 11);
        grade11.SetEducationLevel(seniorSchool.Id);
        
        var grade12 = new GradeLevel("Grade 12", "Senior School Year 3", 12);
        grade12.SetEducationLevel(seniorSchool.Id);

        context.GradeLevels.AddRange(grade7, grade8, grade9, grade10, grade11, grade12);
        await context.SaveChangesAsync(CancellationToken.None);

        // 4. Subjects
        var math = new Subject("Mathematics", "MAT", "Numbers, algebra, geometry, measurement, statistics, probability");
        var english = new Subject("English", "ENG", "Listening & speaking, reading, writing, grammar, literature");
        var kiswahili = new Subject("Kiswahili", "SWA", "Kusikiliza, kuzungumza, kusoma, kuandika, sarufi, fasihi");
        var science = new Subject("Integrated Science", "SCI", "Biology, chemistry, physics, health, environment");
        var socialStudies = new Subject("Social Studies", "SST", "History, geography, citizenship, society, environment");
        var re = new Subject("Religious Education", "RE", "CRE / IRE / HRE");
        var preTech = new Subject("Pre-Technical Studies", "PTS", "ICT, design, technology, entrepreneurship, technical skills");
        var agriculture = new Subject("Agriculture", "AGR", "Crop production, livestock, soil, environment, agribusiness");
        var arts = new Subject("Creative Arts & Sports", "CAS", "Visual arts, performing arts, music, physical education, sports");
        
        // Senior School Subjects
        var coreMath = new Subject("Core Mathematics", "CMAT", "Mathematics for STEM");
        var csl = new Subject("Community Service Learning", "CSL", "Community-based projects, service and citizenship");

        context.Subjects.AddRange(math, english, kiswahili, science, socialStudies, re, preTech, agriculture, arts, coreMath, csl);
        await context.SaveChangesAsync(CancellationToken.None);

        // 5. Grade Subjects (Mapping for Junior School)
        var juniorGrades = new[] { grade7, grade8, grade9 };
        var juniorSubjects = new[] { math, english, kiswahili, science, socialStudies, re, preTech, agriculture, arts };

        foreach (var grade in juniorGrades)
        {
            foreach (var subject in juniorSubjects)
            {
                context.GradeSubjects.Add(new GradeSubject(grade.Id, subject.Id, isCore: true, periodsPerWeek: 5));
            }
        }

        // Mapping for Senior School
        var seniorGrades = new[] { grade10, grade11, grade12 };
        var seniorSubjects = new[] { coreMath, english, kiswahili, csl }; // Compulsory
        
        foreach (var grade in seniorGrades)
        {
            foreach (var subject in seniorSubjects)
            {
                context.GradeSubjects.Add(new GradeSubject(grade.Id, subject.Id, isCore: true, periodsPerWeek: 5));
            }
        }

        await context.SaveChangesAsync(CancellationToken.None);

        // 6. Deep Hierarchy for Mathematics
        var mathNumbers = new Strand(math.Id, "Numbers", 1);
        var mathAlgebra = new Strand(math.Id, "Algebra", 2);
        var mathGeometry = new Strand(math.Id, "Geometry", 3);
        context.Strands.AddRange(mathNumbers, mathAlgebra, mathGeometry);
        await context.SaveChangesAsync(CancellationToken.None);

        var mathIntegers = new SubStrand(mathNumbers.Id, "Integers", 1);
        var mathFractions = new SubStrand(mathNumbers.Id, "Fractions", 2);
        context.SubStrands.AddRange(mathIntegers, mathFractions);
        await context.SaveChangesAsync(CancellationToken.None);

        var lo1 = new LearningObjective(mathIntegers.Id, "Learner should be able to perform operations on integers.", 1);
        var lo2 = new LearningObjective(mathFractions.Id, "Learner should be able to add and subtract fractions.", 2);
        context.LearningObjectives.AddRange(lo1, lo2);

        // Deep Hierarchy for Science
        var sciBio = new Strand(science.Id, "Living Things / Biology", 1);
        var sciChem = new Strand(science.Id, "Matter / Chemistry", 2);
        context.Strands.AddRange(sciBio, sciChem);
        await context.SaveChangesAsync(CancellationToken.None);

        var sciCells = new SubStrand(sciBio.Id, "Cells", 1);
        context.SubStrands.Add(sciCells);
        await context.SaveChangesAsync(CancellationToken.None);

        var lo3 = new LearningObjective(sciCells.Id, "Learner should be able to describe the structure of a plant cell.", 1);
        context.LearningObjectives.Add(lo3);

        await context.SaveChangesAsync(CancellationToken.None);
    }
}
