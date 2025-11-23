using System.ComponentModel.DataAnnotations;

namespace GestaoOS.Models
{
    public class TipoAtivo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty; // Ex: "Computador", "Teclado", "Cadeira"
    }
}