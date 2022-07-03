using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMvcProject.Data.Migrations
{
    public partial class AddProduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Production",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Production", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionDetails_Production_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Production",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductionDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionDetails_ProductId",
                table: "ProductionDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionDetails_ProductionId",
                table: "ProductionDetails",
                column: "ProductionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionDetails");

            migrationBuilder.DropTable(
                name: "Production");
        }
    }
}
