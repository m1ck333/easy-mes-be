using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlGreenMES.Modules.Production.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialProduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "production");

            migrationBuilder.CreateTable(
                name: "processes",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sequence_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_processes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_categories",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "special_request_types",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    adds_processes = table.Column<List<Guid>>(type: "jsonb", nullable: false),
                    removes_processes = table.Column<List<Guid>>(type: "jsonb", nullable: false),
                    only_processes = table.Column<List<Guid>>(type: "jsonb", nullable: false),
                    ignores_dependencies = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_special_request_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sub_processes",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sequence_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sub_processes", x => x.id);
                    table.ForeignKey(
                        name: "fk_sub_processes_processes_process_id",
                        column: x => x.process_id,
                        principalSchema: "production",
                        principalTable: "processes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_category_dependencies",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    depends_on_process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_category_dependencies", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_category_dependencies_processes_depends_on_process_",
                        column: x => x.depends_on_process_id,
                        principalSchema: "production",
                        principalTable: "processes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_category_dependencies_processes_process_id",
                        column: x => x.process_id,
                        principalSchema: "production",
                        principalTable: "processes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_category_dependencies_product_categories_product_ca",
                        column: x => x.product_category_id,
                        principalSchema: "production",
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_category_processes",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_complexity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    sequence_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_category_processes", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_category_processes_processes_process_id",
                        column: x => x.process_id,
                        principalSchema: "production",
                        principalTable: "processes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_category_processes_product_categories_product_categ",
                        column: x => x.product_category_id,
                        principalSchema: "production",
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_processes_tenant_id_code",
                schema: "production",
                table: "processes",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_tenant_id_name",
                schema: "production",
                table: "product_categories",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_category_dependencies_depends_on_process_id",
                schema: "production",
                table: "product_category_dependencies",
                column: "depends_on_process_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_category_dependencies_process_id",
                schema: "production",
                table: "product_category_dependencies",
                column: "process_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_category_dependencies_product_category_id_process_i",
                schema: "production",
                table: "product_category_dependencies",
                columns: new[] { "product_category_id", "process_id", "depends_on_process_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_category_processes_process_id",
                schema: "production",
                table: "product_category_processes",
                column: "process_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_category_processes_product_category_id_process_id",
                schema: "production",
                table: "product_category_processes",
                columns: new[] { "product_category_id", "process_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_special_request_types_tenant_id_code",
                schema: "production",
                table: "special_request_types",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sub_processes_process_id_sequence_order",
                schema: "production",
                table: "sub_processes",
                columns: new[] { "process_id", "sequence_order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_category_dependencies",
                schema: "production");

            migrationBuilder.DropTable(
                name: "product_category_processes",
                schema: "production");

            migrationBuilder.DropTable(
                name: "special_request_types",
                schema: "production");

            migrationBuilder.DropTable(
                name: "sub_processes",
                schema: "production");

            migrationBuilder.DropTable(
                name: "product_categories",
                schema: "production");

            migrationBuilder.DropTable(
                name: "processes",
                schema: "production");
        }
    }
}
