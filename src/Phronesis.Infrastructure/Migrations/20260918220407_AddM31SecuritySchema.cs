using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phronesis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddM31SecuritySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordResetTokenExpiryTime",
                table: "Users",
                newName: "PasswordResetTokenExpiry");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEnd",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorSecret",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId1",
                table: "UserSessions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_UserId1",
                table: "UserSessions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_UserId1",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserSessions_UserId1",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwoFactorSecret",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordResetTokenExpiry",
                table: "Users",
                newName: "PasswordResetTokenExpiryTime");
        }
    }
}
