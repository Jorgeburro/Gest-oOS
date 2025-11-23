using GestaoOS.Data;
using GestaoOS.Models;
using GestaoOS.Services;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoOS.Controllers
{
    [Authorize(Roles = RolesGlobais.Gestor)]

    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context,
                                  UserManager<Usuario> userManager,
                                  RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            // Usamos _context.Users, que é a propriedade correta do IdentityDbContext
            return View(await _context.Users.ToListAsync());
        }

        // GET: Usuarios/Details/5 (O ID de usuário do Identity é uma STRING)
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            ViewData["RolesDisponiveis"] = new SelectList(_roleManager.Roles, "Name", "Name");
            ViewData["Especializacoes"] = new SelectList(await _context.Especializacoes.ToListAsync(), "Id", "Nome");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario viewModel, string RoleSelecionada)
        {
            ModelState.Remove("OrdensDeServicoSolicitadas");
            ModelState.Remove("OrdensDeServicoAtribuidas");
            ModelState.Remove("SalasResponsaveis");

            if (RoleSelecionada == RolesGlobais.Manutencao && !viewModel.EspecializacaoId.HasValue)
            {
                ModelState.AddModelError("EspecializacaoId", "O campo Especialização é obrigatório para o perfil Manutenção.");
            }

            if (ModelState.IsValid)
            {
                var novoUsuario = new Usuario
                {
                    Nome = viewModel.Nome,
                    Email = viewModel.Email,
                    UserName = viewModel.Email,
                    Ativo = viewModel.Ativo,
                    EmailConfirmed = true,
                    EspecializacaoId = (RoleSelecionada == RolesGlobais.Manutencao) ? viewModel.EspecializacaoId : null
                };

                // Lógica para gerar a senha aleatória que implementamos anteriormente.
                var senhaTemporaria = GenerateRandomPassword();
                var result = await _userManager.CreateAsync(novoUsuario, senhaTemporaria);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(RoleSelecionada))
                    {
                        await _userManager.AddToRoleAsync(novoUsuario, RoleSelecionada);
                    }

                    TempData["Success"] = $"Usuário '{novoUsuario.Nome}' criado com sucesso! O funcionário agora deve usar a opção 'Esqueceu sua senha?' para definir uma senha pessoal.";

                    return RedirectToAction(nameof(Index));
                }

                // Se a criação falhar, adiciona os erros para serem exibidos na view.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se o ModelState for inválido, recarregamos os dropdowns e retornamos para a view.
            ViewData["RolesDisponiveis"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name", RoleSelecionada);
            ViewData["Especializacoes"] = new SelectList(await _context.Especializacoes.ToListAsync(), "Id", "Nome", viewModel.EspecializacaoId);
            return View(viewModel);
        }

        // GET: Usuarios/Edit/5 (O ID de usuário do Identity é uma STRING)
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario == null) return NotFound();

            // Pega a role atual do usuário para pré-selecionar o dropdown
            var rolesUsuario = await _userManager.GetRolesAsync(usuario);
            string roleAtual = rolesUsuario.FirstOrDefault();

            ViewData["RolesDisponiveis"] = new SelectList(_roleManager.Roles, "Name", "Name", roleAtual);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email")] Usuario viewModel, string RoleSelecionada)
        {
            if (id != viewModel.Id) return NotFound();

            ModelState.Remove("OrdensDeServicoAbertas");
            ModelState.Remove("OrdensDeServicoAtendidas");

            if (ModelState.IsValid)
            {
                var userToUpdate = await _userManager.FindByIdAsync(id.ToString());
                if (userToUpdate == null) return NotFound();

                userToUpdate.Nome = viewModel.Nome;
                userToUpdate.Email = viewModel.Email;
                userToUpdate.UserName = viewModel.Email;
                userToUpdate.Ativo = viewModel.Ativo;

                var result = await _userManager.UpdateAsync(userToUpdate);

                if (result.Succeeded)
                {
                    var rolesAntigas = await _userManager.GetRolesAsync(userToUpdate);
                    await _userManager.RemoveFromRolesAsync(userToUpdate, rolesAntigas);
                    if (!string.IsNullOrEmpty(RoleSelecionada))
                    {
                        await _userManager.AddToRoleAsync(userToUpdate, RoleSelecionada);
                    }
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewData["RolesDisponiveis"] = new SelectList(_roleManager.Roles, "Name", "Name", RoleSelecionada);
            return View(viewModel);
        }


        // GET: Usuarios/Delete/5 (O ID de usuário do Identity é uma STRING)
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        // A assinatura deve receber INT
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario != null)
            {
                await _userManager.DeleteAsync(usuario);
            }
            return RedirectToAction(nameof(Index));
        }
        private string GenerateRandomPassword()
        {
            // Pega as opções de senha que você configurou no Program.cs
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;
            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            var password = new StringBuilder();
            var random = new Random();
            var charSets = new List<string>();

            if (lowercase) charSets.Add("abcdefghijklmnopqrstuvwxyz");
            if (uppercase) charSets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            if (digit) charSets.Add("0123456789");
            if (nonAlphanumeric) charSets.Add("!@#$%^&*()");

            // Garante que a senha terá pelo menos um caractere de cada conjunto exigido
            foreach (var charSet in charSets)
            {
                password.Append(charSet[random.Next(charSet.Length)]);
            }

            // Preenche o resto da senha com caracteres aleatórios de todos os conjuntos
            var allChars = string.Concat(charSets);
            for (int i = password.Length; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Embaralha a senha para que os primeiros caracteres não sejam sempre os mesmos
            return new string(password.ToString().ToCharArray().OrderBy(c => random.Next()).ToArray());
        }
    }
}