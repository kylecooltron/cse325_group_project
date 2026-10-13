using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorGroupProjectApp.Migrations
{
    /// <inheritdoc />
    public partial class SplitTempAndWindFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WindSpeed",
                table: "WeatherObservations",
                newName: "TempLow");

            migrationBuilder.RenameColumn(
                name: "Temperature",
                table: "WeatherObservations",
                newName: "TempHigh");

            migrationBuilder.RenameColumn(
                name: "PredictedWindSpeed",
                table: "Predictions",
                newName: "PredictedTempLow");

            migrationBuilder.RenameColumn(
                name: "PredictedTemperature",
                table: "Predictions",
                newName: "PredictedTempHigh");

            migrationBuilder.RenameColumn(
                name: "TemperatureAccuracy",
                table: "PredictionResults",
                newName: "TempLowAccuracy");

            migrationBuilder.RenameColumn(
                name: "ActualWindSpeed",
                table: "PredictionResults",
                newName: "TempHighAccuracy");

            migrationBuilder.RenameColumn(
                name: "ActualTemperature",
                table: "PredictionResults",
                newName: "ActualTempLow");

            migrationBuilder.AddColumn<double>(
                name: "MaxWindSpeed",
                table: "WeatherObservations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PredictedMaxWindSpeed",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ActualMaxWindSpeed",
                table: "PredictionResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ActualTempHigh",
                table: "PredictionResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxWindSpeed",
                table: "WeatherObservations");

            migrationBuilder.DropColumn(
                name: "PredictedMaxWindSpeed",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "ActualMaxWindSpeed",
                table: "PredictionResults");

            migrationBuilder.DropColumn(
                name: "ActualTempHigh",
                table: "PredictionResults");

            migrationBuilder.RenameColumn(
                name: "TempLow",
                table: "WeatherObservations",
                newName: "WindSpeed");

            migrationBuilder.RenameColumn(
                name: "TempHigh",
                table: "WeatherObservations",
                newName: "Temperature");

            migrationBuilder.RenameColumn(
                name: "PredictedTempLow",
                table: "Predictions",
                newName: "PredictedWindSpeed");

            migrationBuilder.RenameColumn(
                name: "PredictedTempHigh",
                table: "Predictions",
                newName: "PredictedTemperature");

            migrationBuilder.RenameColumn(
                name: "TempLowAccuracy",
                table: "PredictionResults",
                newName: "TemperatureAccuracy");

            migrationBuilder.RenameColumn(
                name: "TempHighAccuracy",
                table: "PredictionResults",
                newName: "ActualWindSpeed");

            migrationBuilder.RenameColumn(
                name: "ActualTempLow",
                table: "PredictionResults",
                newName: "ActualTemperature");
        }
    }
}
