using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPlacement.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeToApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "Applications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "Applications");
        }
    }
}
