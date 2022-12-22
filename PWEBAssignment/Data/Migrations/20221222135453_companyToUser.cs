using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWEBAssignment.Data.Migrations
{
    /// <inheritdoc />
    public partial class companyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "AspNetUsers",
                newName: "CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Company_CompanyID",
                table: "AspNetUsers",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Company_CompanyID",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CompanyID",
                table: "AspNetUsers",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_CompanyID",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");
        }
    }
}
