using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoOS.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoDataConclusaoNaOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataConclusao",
                table: "OrdensDeServico",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataConclusao",
                table: "OrdensDeServico");
        }
    }
}
