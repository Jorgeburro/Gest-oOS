using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GestaoOS.Data
{
    /// <summary>
    /// Esta classe é usada apenas pelas ferramentas do Entity Framework (ex: para criar migrations)
    /// para que elas saibam como instanciar o ApplicationDbContext em tempo de design.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Cria um construtor de configuração para ler o appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Cria um construtor de opções para o DbContext
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Pega a string de conexão do appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configura o DbContext para usar SQL Server com a string de conexão
            builder.UseSqlServer(connectionString);

            // Retorna uma nova instância do seu DbContext com as opções configuradas
            return new ApplicationDbContext(builder.Options);
        }
    }
}