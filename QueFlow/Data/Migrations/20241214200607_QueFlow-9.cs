using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class QueFlow9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "AspNetUsers");
        }
    }
}
