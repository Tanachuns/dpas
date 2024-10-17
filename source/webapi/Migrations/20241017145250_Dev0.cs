using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class Dev0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegionID",
                table: "AlertSettings",
                newName: "RegionId");

            migrationBuilder.AlterColumn<string[]>(
                name: "DisasterTypes",
                table: "Regions",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlertSettings_RegionId",
                table: "AlertSettings",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlertSettings_Regions_RegionId",
                table: "AlertSettings",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlertSettings_Regions_RegionId",
                table: "AlertSettings");

            migrationBuilder.DropIndex(
                name: "IX_AlertSettings_RegionId",
                table: "AlertSettings");

            migrationBuilder.RenameColumn(
                name: "RegionId",
                table: "AlertSettings",
                newName: "RegionID");

            migrationBuilder.AlterColumn<string[]>(
                name: "DisasterTypes",
                table: "Regions",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }
    }
}
