using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Para IFormFile

namespace GestaoOS.Models
{
    public class OrdemDeServico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AtivoId { get; set; }

        [Required]
        public int SolicitanteId { get; set; }

        public int? ResponsavelId { get; set; }

        [Required(ErrorMessage = "A descrição do problema é obrigatória.")]
        [StringLength(1000, MinimumLength = 10)]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Observacao { get; set; } // Usado para justificativa de "Em Espera"

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Aberta";

        public DateTime DataCriacao { get; set; }

        public DateTime? DataInicioExecucao { get; set; }
        
        public DateTime? DataConclusao { get; set; }

        public DateTime? SlaAlvo { get; set; }

        public byte[]? ImagemConteudo { get; set; }

        [NotMapped] 
        public IFormFile Imagem { get; set; }

        // --- Propriedades de Navegação ---
        [ForeignKey("AtivoId")]
        public virtual Ativo? Ativo { get; set; }

        [ForeignKey("SolicitanteId")]
        public virtual Usuario? Solicitante { get; set; }

        [ForeignKey("ResponsavelId")]
        public virtual Usuario? Responsavel { get; set; }
    }
}