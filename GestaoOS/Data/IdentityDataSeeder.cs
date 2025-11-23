using GestaoOS.Models;
using Microsoft.AspNetCore.Identity;

namespace GestaoOS.Data
{
    public static class IdentityDataSeeder
    {
        // Esta é uma "extension method" que será chamada a partir do Program.cs
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            // Pega os serviços necessários (RoleManager e UserManager)
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();

            string[] roleNames = { "Gestor", "Professor", "Manutencao" };

            foreach (var roleName in roleNames)
            {
                // Verifica se a Role (Gestor, Professor, etc.) JÁ EXISTE no banco (tabela AspNetRoles)
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Se não existe, cria a role
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }

            // --- LÓGICA OPCIONAL: CRIAR UM ADMIN PADRÃO ---
            var adminUser = await userManager.FindByEmailAsync("gestor@seuemail.com");

            if (adminUser == null)
            {
                var newAdmin = new Usuario
                {
                    UserName = "gestor@seuemail.com",
                    Email = "gestor@seuemail.com",
                    Nome = "Gestor Padrão",
                    EmailConfirmed = true, // Pula a confirmação de email para o admin
                };

                // Cria o usuário Gestor com a senha definida
                var result = await userManager.CreateAsync(newAdmin, "SenhaForte!123");

                if (result.Succeeded)
                {
                    // Se o usuário foi criado, atribui a ele a role "Gestor" (Sistema 2)
                    await userManager.AddToRoleAsync(newAdmin, "Gestor");
                }
            }
        }
    }
}