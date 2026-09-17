using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phronesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentAttachmentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationalContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUri = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentAttachments_EducationalContents_EducationalContentId",
                        column: x => x.EducationalContentId,
                        principalTable: "EducationalContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentAttachments_EducationalContentId",
                table: "ContentAttachments",
                column: "EducationalContentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentAttachments");
        }
    }
}
