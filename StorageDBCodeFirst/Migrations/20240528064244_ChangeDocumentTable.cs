using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorageDBCodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDocumentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Products_ProductId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ProductId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Documents");

            migrationBuilder.AddColumn<string>(
                name: "Party",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeID",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ProductTypeID",
                table: "Documents",
                column: "ProductTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ProductTypes_ProductTypeID",
                table: "Documents",
                column: "ProductTypeID",
                principalTable: "ProductTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ProductTypes_ProductTypeID",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ProductTypeID",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Party",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ProductTypeID",
                table: "Documents");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ProductId",
                table: "Documents",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Products_ProductId",
                table: "Documents",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
