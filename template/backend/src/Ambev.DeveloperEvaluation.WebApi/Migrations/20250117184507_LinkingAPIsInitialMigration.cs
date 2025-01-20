using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class LinkingAPIsInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "SalesItems",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            /*
            added manually 

            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "SalesItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            */

            migrationBuilder.AddColumn<int>(
                name: "CartId",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SalesItems_CartItemId",
                table: "SalesItems",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesItems_ProductId",
                table: "SalesItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesItems_Carts_CartItemId",
                table: "SalesItems",
                column: "CartItemId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesItems_Products_ProductId",
                table: "SalesItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesItems_Carts_CartItemId",
                table: "SalesItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesItems_Products_ProductId",
                table: "SalesItems");

            migrationBuilder.DropIndex(
                name: "IX_SalesItems_CartItemId",
                table: "SalesItems");

            migrationBuilder.DropIndex(
                name: "IX_SalesItems_ProductId",
                table: "SalesItems");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "SalesItems");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "Sales");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "SalesItems",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
