using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWEBAssignment.Data.Migrations
{
    /// <inheritdoc />
    public partial class changesInReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfirmReturn",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Rejected",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmReturn",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Rejected",
                table: "Reservations");
        }
    }
}
