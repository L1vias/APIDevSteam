using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIDevSteam.Migrations
{
    /// <inheritdoc />
    public partial class Cupom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoOriginal",
                table: "Jogos",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Banner",
                table: "Jogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Cupons",
                columns: table => new
                {
                    CupomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desconto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: true),
                    DataDescricao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cupons", x => x.CupomId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cupons");

            migrationBuilder.AlterColumn<string>(
                name: "PrecoOriginal",
                table: "Jogos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Banner",
                table: "Jogos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
