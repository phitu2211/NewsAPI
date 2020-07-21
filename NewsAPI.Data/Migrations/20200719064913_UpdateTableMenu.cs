using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsAPI.Data.Migrations
{
    public partial class UpdateTableMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Href",
                table: "Menus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Menus",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Href",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Menus");
        }
    }
}
