using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWorkSessionProcessId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_work_sessions_process_id",
                schema: "orders",
                table: "work_sessions");

            migrationBuilder.DropColumn(
                name: "process_id",
                schema: "orders",
                table: "work_sessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "process_id",
                schema: "orders",
                table: "work_sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_work_sessions_process_id",
                schema: "orders",
                table: "work_sessions",
                column: "process_id");
        }
    }
}
