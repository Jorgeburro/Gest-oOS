#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GestaoOS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace GestaoOS.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment; // ADICIONADO

        public ForgotPasswordModel(UserManager<Usuario> userManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment) // ADICIONADO
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment; // ADICIONADO
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Não revela que o usuário não existe ou não está confirmado
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // --- INÍCIO DA LÓGICA DO TEMPLATE DE E-MAIL ---

                // 1. Gera o código de reset e a URL de callback
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                // 2. Monta o caminho para o template HTML
                var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "email", "RedefinirSenhaTemplate.html");

                // 3. Lê o conteúdo do arquivo HTML
                var htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                // 4. Monta a URL absoluta para a imagem do logo
                var logoUrl = "https://i.imgur.com/SUA_URL_AQUI.png"; // <-- SUBSTITUA PELA SUA URL PÚBLICA

                // 5. Substitui os marcadores no template pelos valores reais
                var emailBody = htmlTemplate
                    .Replace("{{CallbackUrl}}", HtmlEncoder.Default.Encode(callbackUrl))
                    .Replace("{{LogoUrl}}", logoUrl);

                // 6. Envia o e-mail usando o corpo HTML estilizado
                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Redefinição de Senha - Gestão OS",
                    emailBody);

                // --- FIM DA LÓGICA DO TEMPLATE DE E-MAIL ---

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}