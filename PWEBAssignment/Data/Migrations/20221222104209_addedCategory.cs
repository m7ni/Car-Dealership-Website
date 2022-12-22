using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWEBAssignment.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Car");

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Car",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceHour = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Car_CategoryID",
                table: "Car",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_Category_CategoryID",
                table: "Car",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_Category_CategoryID",
                table: "Car");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Car_CategoryID",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Car");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Car",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Car",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
