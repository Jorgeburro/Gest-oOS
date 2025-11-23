using System.ComponentModel.DataAnnotations;

namespace GestaoOS.Models
{
    public class Especializacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty; // Ex: "Equipe de TI", "Equipe de Infraestrutura"
    }
}