using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ControleFinanceiroPessoal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movimentacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Inclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Alteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimentacoes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Movimentacoes",
                columns: new[] { "Id", "Alteracao", "Data", "Descricao", "Inclusao", "Tipo", "Valor" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2022, 8, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cartão de Crédito", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(210), 2, 825.82m },
                    { 2, null, new DateTime(2022, 8, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Curso C#", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(227), 2, 200.00m },
                    { 3, null, new DateTime(2022, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Salário", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(229), 1, 7000.00m },
                    { 4, null, new DateTime(2022, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mercado", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(230), 2, 3000.00m },
                    { 5, null, new DateTime(2022, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Farmácia", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(232), 2, 300.00m },
                    { 6, null, new DateTime(2022, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Combustível", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(233), 2, 800.25m },
                    { 7, null, new DateTime(2022, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Financiamento Carro", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(234), 2, 900.00m },
                    { 8, null, new DateTime(2022, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Financiamento Casa", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(236), 2, 1200.00m },
                    { 9, null, new DateTime(2022, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Freelance Projeto XPTO", new DateTime(2025, 9, 20, 16, 22, 38, 33, DateTimeKind.Local).AddTicks(237), 1, 2500.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimentacoes");
        }
    }
}
