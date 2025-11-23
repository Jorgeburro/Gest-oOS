// Adicione estes usings
using GestaoOS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Para o SelectList
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Para o ToListAsync() nas Roles

// O namespace já deve estar correto
namespace GestaoOS.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        // ADICIONADO: Injetar o RoleManager para acessar os papéis
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public RegisterModel(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole<int>> roleManager) // ADICIONADO: Injetar no construtor
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager; // ADICIONADO: Atribuir
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        // MODIFICADO: Esta propriedade agora carregará as Roles
        public SelectList RolesDisponiveis { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O campo Nome é obrigatório.")]
            [Display(Name = "Nome")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "O campo Email é obrigatório.")]
            [EmailAddress(ErrorMessage = "O Email não é válido.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            // ADICIONADO: Campo para receber o nome da Role do formulário
            [Required(ErrorMessage = "É obrigatório selecionar um perfil.")]
            [Display(Name = "Perfil")]
            public string RoleSelecionada { get; set; }

            [Required(ErrorMessage = "O campo Senha é obrigatório.")]
            [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar senha")]
            [Compare("Password", ErrorMessage = "A senha e a confirmação de senha não são iguais.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            // MODIFICADO: Carregar as ROLES do Identity para o dropdown
            RolesDisponiveis = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new Usuario
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Nome = Input.Nome,
                    Ativo = true,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário criou uma nova conta com senha.");

                  
                    await _userManager.AddToRoleAsync(user, Input.RoleSelecionada);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se o modelo for inválido, precisamos recarregar o dropdown
            RolesDisponiveis = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return Page();
        }
    }
}