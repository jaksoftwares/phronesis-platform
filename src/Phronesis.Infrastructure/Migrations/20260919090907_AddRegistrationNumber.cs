using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phronesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "LearnerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LearnerGuardianLinkRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LearnerProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuardianProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationshipType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerGuardianLinkRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearnerGuardianLinkRequests_GuardianProfiles_GuardianProfileId",
                        column: x => x.GuardianProfileId,
                        principalTable: "GuardianProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearnerGuardianLinkRequests_LearnerProfiles_LearnerProfileId",
                        column: x => x.LearnerProfileId,
                        principalTable: "LearnerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearnerGuardianLinkRequests_GuardianProfileId",
                table: "LearnerGuardianLinkRequests",
                column: "GuardianProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnerGuardianLinkRequests_LearnerProfileId",
                table: "LearnerGuardianLinkRequests",
                column: "LearnerProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearnerGuardianLinkRequests");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "LearnerProfiles");
        }
    }
}
