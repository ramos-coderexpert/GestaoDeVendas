using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoDeVendas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlteracaoTabelasClienteEVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeCliente",
                table: "Vendas");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Clientes",
                newName: "DataRegistro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataRegistro",
                table: "Clientes",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "NomeCliente",
                table: "Vendas",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
