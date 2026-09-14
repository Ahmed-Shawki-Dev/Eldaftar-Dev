using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGroupAndStudentGroupStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StudentGroups");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "StudentGroups",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentGroups");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StudentGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
