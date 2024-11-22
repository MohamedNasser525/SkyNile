using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesToFlight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DepartureLocation",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ArrivalLocation",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ArrivalLocation",
                table: "Flights",
                column: "ArrivalLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ArrivalTime",
                table: "Flights",
                column: "ArrivalTime");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DepartureLocation",
                table: "Flights",
                column: "DepartureLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DepartureTime",
                table: "Flights",
                column: "DepartureTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flights_ArrivalLocation",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_ArrivalTime",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DepartureLocation",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_DepartureTime",
                table: "Flights");

            migrationBuilder.AlterColumn<string>(
                name: "DepartureLocation",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ArrivalLocation",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
