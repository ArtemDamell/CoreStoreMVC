using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreStoreMVC.Data.Migrations
{
    public partial class ClientModelAdding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserIP",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserMAC",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserMAC",
                table: "AspNetUsers");
        }
    }
}
