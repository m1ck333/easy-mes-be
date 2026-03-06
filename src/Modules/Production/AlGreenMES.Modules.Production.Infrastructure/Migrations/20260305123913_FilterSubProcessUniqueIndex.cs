using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlGreenMES.Modules.Production.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FilterSubProcessUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sub_processes_process_id_sequence_order",
                schema: "production",
                table: "sub_processes");

            migrationBuilder.CreateIndex(
                name: "ix_sub_processes_process_id_sequence_order",
                schema: "production",
                table: "sub_processes",
                columns: new[] { "process_id", "sequence_order" },
                unique: true,
                filter: "is_active = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sub_processes_process_id_sequence_order",
                schema: "production",
                table: "sub_processes");

            migrationBuilder.CreateIndex(
                name: "ix_sub_processes_process_id_sequence_order",
                schema: "production",
                table: "sub_processes",
                columns: new[] { "process_id", "sequence_order" },
                unique: true);
        }
    }
}
