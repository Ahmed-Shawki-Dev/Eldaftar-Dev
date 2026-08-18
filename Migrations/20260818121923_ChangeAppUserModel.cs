using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAppUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AspNetUsers_UserId",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_TenantId",
                table: "Teachers");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Tenants",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TenantId_UserId",
                table: "Teachers",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId_PhoneNumber",
                table: "AspNetUsers",
                columns: new[] { "TenantId", "PhoneNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AspNetUsers_UserId",
                table: "Tenants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AspNetUsers_UserId",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_TenantId_UserId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TenantId_PhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Tenants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TenantId",
                table: "Teachers",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AspNetUsers_UserId",
                table: "Tenants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
