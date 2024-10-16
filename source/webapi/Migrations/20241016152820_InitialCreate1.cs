using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Regions_Coordinate_LocationId",
                table: "Regions");

            migrationBuilder.DropTable(
                name: "Coordinate");

            migrationBuilder.DropIndex(
                name: "IX_Regions_LocationId",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Regions");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Regions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Regions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Regions");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Regions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coordinate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Regions_LocationId",
                table: "Regions",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Regions_Coordinate_LocationId",
                table: "Regions",
                column: "LocationId",
                principalTable: "Coordinate",
                principalColumn: "Id");
        }
    }
}
