using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorageDBCodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnsInStorageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_ProductTypes_ProductTypeId",
                table: "Storages");

            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Products_ProductId",
                table: "Storages");

            migrationBuilder.DropIndex(
                name: "IX_Storages_ProductTypeId",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "Storages");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Storages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Products_ProductId",
                table: "Storages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Products_ProductId",
                table: "Storages");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Storages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeId",
                table: "Storages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Storages_ProductTypeId",
                table: "Storages",
                column: "ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_ProductTypes_ProductTypeId",
                table: "Storages",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Products_ProductId",
                table: "Storages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
