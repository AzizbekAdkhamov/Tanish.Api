using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tanish.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupSizeAndFixForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DesiredGroupSize",
                table: "ActivityProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredGroupSize",
                table: "ActivityProfiles");
        }
    }
}
