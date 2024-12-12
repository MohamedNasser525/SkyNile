using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Flights_Entity_Columns_Modifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartureLocation",
                table: "Flights",
                newName: "DepartureCountry");

            migrationBuilder.RenameColumn(
                name: "ArrivalLocation",
                table: "Flights",
                newName: "DepartureAirport");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_DepartureLocation",
                table: "Flights",
                newName: "IX_Flights_DepartureCountry");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_ArrivalLocation",
                table: "Flights",
                newName: "IX_Flights_DepartureAirport");

            migrationBuilder.AddColumn<string>(
                name: "ArrivalAirport",
                table: "Flights",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArrivalCountry",
                table: "Flights",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ArrivalAirport",
                table: "Flights",
                column: "ArrivalAirport");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ArrivalCountry",
                table: "Flights",
                column: "ArrivalCountry");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flights_ArrivalAirport",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_ArrivalCountry",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "ArrivalAirport",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "ArrivalCountry",
                table: "Flights");

            migrationBuilder.RenameColumn(
                name: "DepartureCountry",
                table: "Flights",
                newName: "DepartureLocation");

            migrationBuilder.RenameColumn(
                name: "DepartureAirport",
                table: "Flights",
                newName: "ArrivalLocation");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_DepartureCountry",
                table: "Flights",
                newName: "IX_Flights_DepartureLocation");

            migrationBuilder.RenameIndex(
                name: "IX_Flights_DepartureAirport",
                table: "Flights",
                newName: "IX_Flights_ArrivalLocation");
        }
    }
}
