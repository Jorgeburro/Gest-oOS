using GestaoOS.Models; // <-- ESTA LINHA ESTAVA FALTANDO (CAUSA DO ERRO 1 E 3)
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoOS.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TipoAtivo> TipoAtivos { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Ativo> Ativos { get; set; }
        public DbSet<OrdemDeServico> OrdensDeServico { get; set; }
        public DbSet<OrdemDeServicoHistorico> OrdensDeServicoHistorico { get; set; }
        public DbSet<Especializacao> Especializacoes { get; set; }

        public DbSet<Posicao> Posicoes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<OrdemDeServico>()
                   .HasIndex(os => new { os.Status, os.ResponsavelId })
                   .HasDatabaseName("IX_OrdensDeServico_Status_Responsavel");

            modelBuilder.Entity<OrdemDeServico>()
                .HasIndex(os => os.DataCriacao)
                .HasDatabaseName("IX_OrdensDeServico_DataCriacao");

            modelBuilder.Entity<Ativo>()
                .HasIndex(a => a.Status)
                .HasDatabaseName("IX_Ativos_Status");


            modelBuilder.Entity<Especializacao>().HasData(
                new Especializacao { Id = 1, Nome = "Equipe de TI" },
                new Especializacao { Id = 2, Nome = "Equipe de Infraestrutura" }
            );

       

            modelBuilder.Entity<Sala>()
                .HasOne(s => s.Responsavel)
                .WithMany(u => u.SalasResponsaveis)
                .HasForeignKey(s => s.ResponsavelId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrdemDeServico>()
                .HasOne(os => os.Solicitante)
                .WithMany(u => u.OrdensDeServicoSolicitadas)
                .HasForeignKey(os => os.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdemDeServico>()
                .HasOne(os => os.Responsavel)
                .WithMany(u => u.OrdensDeServicoAtribuidas)
                .HasForeignKey(os => os.ResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdemDeServico>()
                .HasOne(os => os.Ativo)
                .WithMany(a => a.OrdensDeServico)
                .HasForeignKey(os => os.AtivoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdemDeServicoHistorico>()
                .HasIndex(h => h.OrdemDeServicoIdOriginal);

            modelBuilder.Entity<Usuario>().HasQueryFilter(u => u.Ativo);

            modelBuilder.Entity<Sala>()
                .HasMany(sala => sala.Posicoes) 
                .WithOne(pos => pos.Sala)      
                .HasForeignKey(pos => pos.SalaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Posicao>()
                .HasMany(pos => pos.Ativos)     
                .WithOne(ativo => ativo.Posicao) 
                .HasForeignKey(ativo => ativo.PosicaoId) 
                .OnDelete(DeleteBehavior.SetNull); 

           
            modelBuilder.Entity<Sala>()
                .HasMany(s => s.Ativos) 
                .WithOne(a => a.Sala)   
                .HasForeignKey(a => a.SalaId) 
                .OnDelete(DeleteBehavior.Restrict); 

        }
    }
}