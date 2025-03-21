using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorageDBCodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class NotNullFieldsInDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ProductTypes_ProductTypeID",
                table: "Documents");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeID",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Party",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ProductTypes_ProductTypeID",
                table: "Documents",
                column: "ProductTypeID",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ProductTypes_ProductTypeID",
                table: "Documents");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeID",
                table: "Documents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Party",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ProductTypes_ProductTypeID",
                table: "Documents",
                column: "ProductTypeID",
                principalTable: "ProductTypes",
                principalColumn: "Id");
        }
    }
}
