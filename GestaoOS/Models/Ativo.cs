using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoOS.Models
{
    public class Ativo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Display(Name = "Número de Série")]
        [StringLength(100)]
        public string? NumeroSerie { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Operacional";

        [Required]
        public int SalaId { get; set; }

        // --- Propriedades de Navegação ---
        [ForeignKey("SalaId")]
        public virtual Sala? Sala { get; set; }

        // Um ativo pode ter várias OS!!!!!!!
        public virtual ICollection<OrdemDeServico> OrdensDeServico { get; set; } = new List<OrdemDeServico>();


        [Required(ErrorMessage = "O tipo de ativo é obrigatório.")]
        public int TipoAtivoId { get; set; }

        [ForeignKey("TipoAtivoId")]
        public virtual TipoAtivo? TipoAtivo { get; set; }

  
        [Display(Name = "Posição na Sala")]
        public int? PosicaoId { get; set; }

        [ForeignKey("PosicaoId")]
        public virtual Posicao? Posicao { get; set; }

    }
}