using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlGreenMES.Modules.Production.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryWarningCriticalDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "default_critical_days",
                schema: "production",
                table: "product_categories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "default_warning_days",
                schema: "production",
                table: "product_categories",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "default_critical_days",
                schema: "production",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "default_warning_days",
                schema: "production",
                table: "product_categories");
        }
    }
}
