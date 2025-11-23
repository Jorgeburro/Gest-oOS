using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoOS.Models
{
    public class Usuario : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;

        // ... resto do seu modelo ...
        [InverseProperty("Solicitante")]
        public virtual ICollection<OrdemDeServico> OrdensDeServicoSolicitadas { get; set; } = new List<OrdemDeServico>();

        [InverseProperty("Responsavel")]
        public virtual ICollection<OrdemDeServico> OrdensDeServicoAtribuidas { get; set; } = new List<OrdemDeServico>();

        [InverseProperty("Responsavel")]
        public virtual ICollection<Sala> SalasResponsaveis { get; set; } = new List<Sala>();
        public int? EspecializacaoId { get; set; }
        public bool Ativo { get; set; } = true;


        // --- Propriedade de Navegação ---
        [ForeignKey("EspecializacaoId")]
        public virtual Especializacao? Especializacao { get; set; }
    }
}