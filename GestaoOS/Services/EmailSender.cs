using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace GestaoOS.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _configuration["SendGridKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Chave 'SendGridKey' não encontrada. Verifique seus User Secrets.");
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("jorjaozinho1@gmail.com", "Sistema Gestão OS"); // Use o e-mail que você verificou!
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

            var response = await client.SendEmailAsync(msg);

            // Código melhorado para reportar erros
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                var errorMessage = $"Falha ao enviar e-mail com SendGrid. Status: {response.StatusCode}. Erro: {errorBody}";

                // Mostra o erro detalhado no console de debug
                System.Diagnostics.Debug.WriteLine(errorMessage);

                // Lança uma exceção para que o erro seja mais visível durante o desenvolvimento
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}