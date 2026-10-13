using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGroupProjectApp.Migrations
{
    /// <inheritdoc />
    public partial class AddForecastSnapshotAndPotentialPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ForecastMaxWindSpeed",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ForecastPrecipitation",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ForecastTempHigh",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ForecastTempLow",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PotentialPoints",
                table: "Predictions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForecastMaxWindSpeed",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "ForecastPrecipitation",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "ForecastTempHigh",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "ForecastTempLow",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "PotentialPoints",
                table: "Predictions");
        }
    }
}
