using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class price : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Tickets",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Flights",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Flights");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Tickets",
                newName: "Price");
        }
    }
}
