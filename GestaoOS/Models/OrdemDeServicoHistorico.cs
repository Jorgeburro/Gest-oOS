using Microsoft.EntityFrameworkCore; // Para [Index]
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoOS.Models
{
    // Adiciona um índice para otimizar buscas pelo ID da OS original
    [Index(nameof(OrdemDeServicoIdOriginal))]
    public class OrdemDeServicoHistorico
    {
        [Key]
        public int Id { get; set; }

        
        [Required]
        public int OrdemDeServicoIdOriginal { get; set; }

        [Required]
        public int AtivoIdOriginal { get; set; } 

        [Required]
        [StringLength(150)]
        public string AtivoNome { get; set; } = string.Empty;

        [StringLength(100)]
        public string? AtivoNumeroSerie { get; set; }

        [Required]
        [StringLength(100)]
        public string AtivoSalaNome { get; set; } = string.Empty; 

        [Required]
        [StringLength(100)]
        public string AtivoTipoNome { get; set; } = string.Empty; 
        [Required]
        public int SolicitanteIdOriginal { get; set; }
        [Required]
        [StringLength(200)]
        public string SolicitanteNome { get; set; } = string.Empty;

        public int? ResponsavelIdOriginal { get; set; } 
        [StringLength(200)]
        public string? ResponsavelNome { get; set; } 

        [Required]
        public int ResponsavelValidacaoIdOriginal { get; set; }
        [Required]
        [StringLength(200)]
        public string ResponsavelValidacaoNome { get; set; } = string.Empty;

        // --- Dados da OS ---
        [Required]
        [StringLength(1000)]
        public string Descricao { get; set; } = string.Empty; 

        [StringLength(1000)]
        public string? SolucaoAplicada { get; set; } 

        [StringLength(1000)]
        public string? ObservacaoValidacao { get; set; } 

        [Required]
        public DateTime DataCriacao { get; set; }
        public DateTime? DataInicioExecucao { get; set; }
        public DateTime? DataConclusao { get; set; } 
        [Required]
        public DateTime DataValidacao { get; set; } 
    }
}