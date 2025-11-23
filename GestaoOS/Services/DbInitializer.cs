using GestaoOS.Data;
using GestaoOS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestaoOS.Services
{
    public interface IDbInitializer
    {
        Task Initialize();
    }

    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            try
            {
                if ((await _context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception)
            {
                // Tratar exceção (ex: DB não existe ainda)
            }

            // 2. Criar Roles (Papéis)
            if (!await _roleManager.RoleExistsAsync(RolesGlobais.Gestor))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(RolesGlobais.Gestor));
            }
            if (!await _roleManager.RoleExistsAsync(RolesGlobais.Manutencao))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(RolesGlobais.Manutencao));
            }
            if (!await _roleManager.RoleExistsAsync(RolesGlobais.Professor))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(RolesGlobais.Professor));
            }

            // 3. Opcional: Criar um usuário Admin (Gestor) Padrão
            string adminEmail = "admin@gestaoos.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var userAdmin = new Usuario
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nome = "Admin Gestor",
                    Ativo = true,
                    EmailConfirmed = true // Confirmar e-mail automaticamente para o admin
                };

                // Cria o usuário COM SENHA HASHED
                var result = await _userManager.CreateAsync(userAdmin, "SenhaForte!123");

                if (result.Succeeded)
                {
                    // Adiciona o usuário ao Papel (Role) de Gestor
                    await _userManager.AddToRoleAsync(userAdmin, RolesGlobais.Gestor);
                }
            }
        }
    }
}