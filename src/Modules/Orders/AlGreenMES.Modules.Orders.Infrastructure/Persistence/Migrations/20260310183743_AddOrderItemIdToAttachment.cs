using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlGreenMES.Modules.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemIdToAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "order_item_id",
                schema: "orders",
                table: "order_attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_attachments_order_item_id",
                schema: "orders",
                table: "order_attachments",
                column: "order_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_attachments_order_items_order_item_id",
                schema: "orders",
                table: "order_attachments",
                column: "order_item_id",
                principalSchema: "orders",
                principalTable: "order_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_attachments_order_items_order_item_id",
                schema: "orders",
                table: "order_attachments");

            migrationBuilder.DropIndex(
                name: "ix_order_attachments_order_item_id",
                schema: "orders",
                table: "order_attachments");

            migrationBuilder.DropColumn(
                name: "order_item_id",
                schema: "orders",
                table: "order_attachments");
        }
    }
}
