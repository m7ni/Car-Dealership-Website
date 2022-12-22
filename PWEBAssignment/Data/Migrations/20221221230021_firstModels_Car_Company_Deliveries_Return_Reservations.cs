using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWEBAssignment.Data.Migrations
{
    public partial class firstModels_Car_Company_Deliveries_Return_Reservations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicencePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Damage = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<bool>(type: "bit", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<bool>(type: "bit", nullable: false),
                    CompanyID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Car", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Car_Company_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Company",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientUserId = table.Column<int>(type: "int", nullable: true),
                    ClientUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CarId = table.Column<int>(type: "int", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnId = table.Column<int>(type: "int", nullable: true),
                    DeliveryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_AspNetUsers_ClientUserId1",
                        column: x => x.ClientUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Car_CarId",
                        column: x => x.CarId,
                        principalTable: "Car",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NumberOfKm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleDamage = table.Column<bool>(type: "bit", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeUserId = table.Column<int>(type: "int", nullable: true),
                    EmployeUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReservationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliveries_AspNetUsers_EmployeUserId1",
                        column: x => x.EmployeUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Deliveries_Reservations_Id",
                        column: x => x.Id,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Returns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NumberOfKm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleDamage = table.Column<bool>(type: "bit", nullable: false),
                    PhotoEvidence = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeUserId = table.Column<int>(type: "int", nullable: true),
                    EmployeUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReservationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Returns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Returns_AspNetUsers_EmployeUserId1",
                        column: x => x.EmployeUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Returns_Reservations_Id",
                        column: x => x.Id,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Car_CompanyID",
                table: "Car",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_EmployeUserId1",
                table: "Deliveries",
                column: "EmployeUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CarId",
                table: "Reservations",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ClientUserId1",
                table: "Reservations",
                column: "ClientUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CompanyId",
                table: "Reservations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_EmployeUserId1",
                table: "Returns",
                column: "EmployeUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "Returns");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");
        }
    }
}
