using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedCatedoyrIdIntoPriceHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PriceType",
                table: "PriceHistory",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "PriceHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryTypeId",
                table: "PriceHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductCategoryTypeCategoryTypeId",
                table: "PriceHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_CategoryId",
                table: "PriceHistory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_ProductCategoryTypeCategoryTypeId",
                table: "PriceHistory",
                column: "ProductCategoryTypeCategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceHistory_ProductCategoriesType_ProductCategoryTypeCateg~",
                table: "PriceHistory",
                column: "ProductCategoryTypeCategoryTypeId",
                principalTable: "ProductCategoriesType",
                principalColumn: "CategoryTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceHistory_ProductCategories_CategoryId",
                table: "PriceHistory",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceHistory_ProductCategoriesType_ProductCategoryTypeCateg~",
                table: "PriceHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceHistory_ProductCategories_CategoryId",
                table: "PriceHistory");

            migrationBuilder.DropIndex(
                name: "IX_PriceHistory_CategoryId",
                table: "PriceHistory");

            migrationBuilder.DropIndex(
                name: "IX_PriceHistory_ProductCategoryTypeCategoryTypeId",
                table: "PriceHistory");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "PriceHistory");

            migrationBuilder.DropColumn(
                name: "CategoryTypeId",
                table: "PriceHistory");

            migrationBuilder.DropColumn(
                name: "ProductCategoryTypeCategoryTypeId",
                table: "PriceHistory");

            migrationBuilder.AlterColumn<int>(
                name: "PriceType",
                table: "PriceHistory",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
