using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlGreenMES.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnsToTenantSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "tenancy",
                table: "tenant_settings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "tenancy",
                table: "tenant_settings",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "tenancy",
                table: "tenant_settings");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "tenancy",
                table: "tenant_settings");
        }
    }
}
