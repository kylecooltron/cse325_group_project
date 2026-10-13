using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGroupProjectApp.Migrations
{
    /// <inheritdoc />
    public partial class DropUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Predictions_UserProfiles_UserProfileId",
                table: "Predictions");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Predictions_UserProfileId",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Predictions");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Predictions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_UserId",
                table: "Predictions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Predictions_AspNetUsers_UserId",
                table: "Predictions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Predictions_AspNetUsers_UserId",
                table: "Predictions");

            migrationBuilder.DropIndex(
                name: "IX_Predictions_UserId",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Predictions");

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "Predictions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_UserProfileId",
                table: "Predictions",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Predictions_UserProfiles_UserProfileId",
                table: "Predictions",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
