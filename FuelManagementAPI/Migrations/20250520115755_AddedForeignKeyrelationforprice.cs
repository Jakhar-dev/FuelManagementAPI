using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedForeignKeyrelationforprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceHistory_ProductCategoriesType_ProductCategoryTypeCateg~",
                table: "PriceHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceHistory_ProductCategories_CategoryId",
                table: "PriceHistory");

            migrationBuilder.DropIndex(
                name: "IX_PriceHistory_ProductCategoryTypeCategoryTypeId",
                table: "PriceHistory");

            migrationBuilder.DropColumn(
                name: "ProductCategoryTypeCategoryTypeId",
                table: "PriceHistory");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_CategoryTypeId",
                table: "PriceHistory",
                column: "CategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceHistory_ProductCategoriesType_CategoryTypeId",
                table: "PriceHistory",
                column: "CategoryTypeId",
                principalTable: "ProductCategoriesType",
                principalColumn: "CategoryTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceHistory_ProductCategories_CategoryId",
                table: "PriceHistory",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceHistory_ProductCategoriesType_CategoryTypeId",
                table: "PriceHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceHistory_ProductCategories_CategoryId",
                table: "PriceHistory");

            migrationBuilder.DropIndex(
                name: "IX_PriceHistory_CategoryTypeId",
                table: "PriceHistory");

            migrationBuilder.AddColumn<int>(
                name: "ProductCategoryTypeCategoryTypeId",
                table: "PriceHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
    }
}
