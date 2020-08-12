using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsAPI.Data.Migrations
{
    public partial class ThemUrlImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlImage",
                table: "News",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlImage",
                table: "News");
        }
    }
}
