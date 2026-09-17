using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phronesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedContentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationalContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedContents_EducationalContents_EducationalContentId",
                        column: x => x.EducationalContentId,
                        principalTable: "EducationalContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedContents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedContents_EducationalContentId",
                table: "SavedContents",
                column: "EducationalContentId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedContents_UserId_EducationalContentId",
                table: "SavedContents",
                columns: new[] { "UserId", "EducationalContentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedContents");
        }
    }
}
