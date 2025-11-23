using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoOS.Migrations
{
    public partial class AdicionandoIndicesDePerformance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ativos_Status",
                table: "Ativos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServico_DataCriacao",
                table: "OrdensDeServico",
                column: "DataCriacao");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServico_Status_Responsavel",
                table: "OrdensDeServico",
                columns: new[] { "Status", "ResponsavelId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_Ativos_Status",
                table: "Ativos");

            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServico_DataCriacao",
                table: "OrdensDeServico");

            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServico_Status_Responsavel",
                table: "OrdensDeServico");
        }
    }
}