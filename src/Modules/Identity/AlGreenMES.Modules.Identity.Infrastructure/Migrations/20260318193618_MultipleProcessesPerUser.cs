using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlGreenMES.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MultipleProcessesPerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "process_id",
                schema: "identity",
                table: "users");

            migrationBuilder.CreateTable(
                name: "user_processes",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_processes", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_processes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_processes_process_id",
                schema: "identity",
                table: "user_processes",
                column: "process_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_processes_user_id_process_id",
                schema: "identity",
                table: "user_processes",
                columns: new[] { "user_id", "process_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_processes",
                schema: "identity");

            migrationBuilder.AddColumn<Guid>(
                name: "process_id",
                schema: "identity",
                table: "users",
                type: "uuid",
                nullable: true);
        }
    }
}
