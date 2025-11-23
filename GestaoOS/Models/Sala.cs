using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoOS.Models
{
    public class Sala
    {

        [Required(ErrorMessage = "O Bloco é obrigatório.")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "O Bloco deve conter apenas uma letra.")]
        public string Bloco { get; set; } = string.Empty;

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descricao { get; set; }

        public int? ResponsavelId { get; set; }

        // --- Propriedades de Navegação ---
        [ForeignKey("ResponsavelId")]
        public virtual Usuario? Responsavel { get; set; }
        public virtual ICollection<Ativo> Ativos { get; set; } = new List<Ativo>();

        public virtual ICollection<Posicao> Posicoes { get; set; } = new List<Posicao>();
    }
}