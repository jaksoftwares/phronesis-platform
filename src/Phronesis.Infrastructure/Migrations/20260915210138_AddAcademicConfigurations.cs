using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phronesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GradeSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCore = table.Column<bool>(type: "bit", nullable: false),
                    PeriodsPerWeek = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeSubjects_GradeLevels_GradeLevelId",
                        column: x => x.GradeLevelId,
                        principalTable: "GradeLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GradeSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubStrandPrerequisites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubStrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrerequisiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubStrandPrerequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubStrandPrerequisites_SubStrands_PrerequisiteId",
                        column: x => x.PrerequisiteId,
                        principalTable: "SubStrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubStrandPrerequisites_SubStrands_SubStrandId",
                        column: x => x.SubStrandId,
                        principalTable: "SubStrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherCompetences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherCompetences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherCompetences_GradeLevels_GradeLevelId",
                        column: x => x.GradeLevelId,
                        principalTable: "GradeLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherCompetences_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherCompetences_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GradeSubjects_GradeLevelId_SubjectId",
                table: "GradeSubjects",
                columns: new[] { "GradeLevelId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GradeSubjects_SubjectId",
                table: "GradeSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubStrandPrerequisites_PrerequisiteId",
                table: "SubStrandPrerequisites",
                column: "PrerequisiteId");

            migrationBuilder.CreateIndex(
                name: "IX_SubStrandPrerequisites_SubStrandId_PrerequisiteId",
                table: "SubStrandPrerequisites",
                columns: new[] { "SubStrandId", "PrerequisiteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCompetences_GradeLevelId",
                table: "TeacherCompetences",
                column: "GradeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCompetences_SubjectId",
                table: "TeacherCompetences",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCompetences_TeacherProfileId_SubjectId_GradeLevelId",
                table: "TeacherCompetences",
                columns: new[] { "TeacherProfileId", "SubjectId", "GradeLevelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GradeSubjects");

            migrationBuilder.DropTable(
                name: "SubStrandPrerequisites");

            migrationBuilder.DropTable(
                name: "TeacherCompetences");
        }
    }
}
