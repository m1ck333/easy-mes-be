using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnsToMutableEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "orders",
                table: "work_sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "orders",
                table: "order_item_sub_processes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "orders",
                table: "order_item_sub_processes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "orders",
                table: "order_item_processes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "orders",
                table: "order_item_processes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "orders",
                table: "change_requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "orders",
                table: "block_requests",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "orders",
                table: "work_sessions");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "orders",
                table: "order_item_sub_processes");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "orders",
                table: "order_item_sub_processes");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "orders",
                table: "order_item_processes");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "orders",
                table: "order_item_processes");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "orders",
                table: "change_requests");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "orders",
                table: "block_requests");
        }
    }
}
