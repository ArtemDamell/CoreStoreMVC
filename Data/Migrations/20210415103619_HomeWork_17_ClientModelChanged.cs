using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreStoreMVC.Data.Migrations
{
    public partial class HomeWork_17_ClientModelChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Appointments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Appointments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerFirstName",
                table: "Appointments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerLastName",
                table: "Appointments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CustomerFirstName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CustomerLastName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
