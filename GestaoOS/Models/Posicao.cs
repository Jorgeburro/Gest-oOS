using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoOS.Models
{
    public class Posicao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 40, ErrorMessage = "A posição deve ser um número entre 1 e 40.")]
        public int NumeroPosicao { get; set; }

        [Required]
        public int SalaId { get; set; }

        [ForeignKey("SalaId")]
        public virtual Sala? Sala { get; set; }

        public virtual ICollection<Ativo> Ativos { get; set; } = new List<Ativo>();
    }
}