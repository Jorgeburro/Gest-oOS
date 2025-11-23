using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoOS.Migrations
{
    /// <inheritdoc />
    public partial class RefatorandoHistoricoParaSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdensDeServicoHistorico_AspNetUsers_ResponsavelId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdensDeServicoHistorico_AspNetUsers_ResponsavelValidacaoId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdensDeServicoHistorico_AspNetUsers_SolicitanteId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdensDeServicoHistorico_Ativos_AtivoId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServicoHistorico_AtivoId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServicoHistorico_ResponsavelId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServicoHistorico_ResponsavelValidacaoId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServicoHistorico_SolicitanteId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "AtivoId",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.RenameColumn(
                name: "SolicitanteId",
                table: "OrdensDeServicoHistorico",
                newName: "SolicitanteIdOriginal");

            migrationBuilder.RenameColumn(
                name: "ResponsavelValidacaoId",
                table: "OrdensDeServicoHistorico",
                newName: "ResponsavelValidacaoIdOriginal");

            migrationBuilder.RenameColumn(
                name: "ResponsavelId",
                table: "OrdensDeServicoHistorico",
                newName: "AtivoIdOriginal");

            migrationBuilder.RenameColumn(
                name: "Observacao",
                table: "OrdensDeServicoHistorico",
                newName: "ObservacaoValidacao");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataConclusao",
                table: "OrdensDeServicoHistorico",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "AtivoNome",
                table: "OrdensDeServicoHistorico",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AtivoNumeroSerie",
                table: "OrdensDeServicoHistorico",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AtivoSalaNome",
                table: "OrdensDeServicoHistorico",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AtivoTipoNome",
                table: "OrdensDeServicoHistorico",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ResponsavelIdOriginal",
                table: "OrdensDeServicoHistorico",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsavelNome",
                table: "OrdensDeServicoHistorico",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsavelValidacaoNome",
                table: "OrdensDeServicoHistorico",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SolicitanteNome",
                table: "OrdensDeServicoHistorico",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServicoHistorico_OrdemDeServicoIdOriginal",
                table: "OrdensDeServicoHistorico",
                column: "OrdemDeServicoIdOriginal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServicoHistorico_OrdemDeServicoIdOriginal",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "AtivoNome",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "AtivoNumeroSerie",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "AtivoSalaNome",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "AtivoTipoNome",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "ResponsavelIdOriginal",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "ResponsavelNome",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "ResponsavelValidacaoNome",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.DropColumn(
                name: "SolicitanteNome",
                table: "OrdensDeServicoHistorico");

            migrationBuilder.RenameColumn(
                name: "SolicitanteIdOriginal",
                table: "OrdensDeServicoHistorico",
                newName: "SolicitanteId");

            migrationBuilder.RenameColumn(
                name: "ResponsavelValidacaoIdOriginal",
                table: "OrdensDeServicoHistorico",
                newName: "ResponsavelValidacaoId");

            migrationBuilder.RenameColumn(
                name: "ObservacaoValidacao",
                table: "OrdensDeServicoHistorico",
                newName: "Observacao");

            migrationBuilder.RenameColumn(
                name: "AtivoIdOriginal",
                table: "OrdensDeServicoHistorico",
                newName: "ResponsavelId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataConclusao",
                table: "OrdensDeServicoHistorico",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AtivoId",
                table: "OrdensDeServicoHistorico",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServicoHistorico_AtivoId",
                table: "OrdensDeServicoHistorico",
                column: "AtivoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServicoHistorico_ResponsavelId",
                table: "OrdensDeServicoHistorico",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServicoHistorico_ResponsavelValidacaoId",
                table: "OrdensDeServicoHistorico",
                column: "ResponsavelValidacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServicoHistorico_SolicitanteId",
                table: "OrdensDeServicoHistorico",
                column: "SolicitanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdensDeServicoHistorico_AspNetUsers_ResponsavelId",
                table: "OrdensDeServicoHistorico",
                column: "ResponsavelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdensDeServicoHistorico_AspNetUsers_ResponsavelValidacaoId",
                table: "OrdensDeServicoHistorico",
                column: "ResponsavelValidacaoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdensDeServicoHistorico_AspNetUsers_SolicitanteId",
                table: "OrdensDeServicoHistorico",
                column: "SolicitanteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdensDeServicoHistorico_Ativos_AtivoId",
                table: "OrdensDeServicoHistorico",
                column: "AtivoId",
                principalTable: "Ativos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
