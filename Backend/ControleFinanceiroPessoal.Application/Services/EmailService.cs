using ControleFinanceiroPessoal.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task EnviarNotificacaoSaldoNegativoAsync(decimal saldoAtual)
        {
            var assunto = "Controle Financeiro - Saldo Negativo";
            var corpo = $"Você atualmente possui um saldo negativo de {saldoAtual.ToString("C", new CultureInfo("pt-BR"))}";

            await EnviarEmailAsync(_emailSettings.EmailDestinatario, assunto, corpo);
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpo)
        {
            try
            {
                _logger.LogInformation($"Tentando enviar email para {destinatario}...");

                var message = new MailMessage();
                message.From = new MailAddress(_emailSettings.EmailRemetente, _emailSettings.NomeRemetente);
                message.To.Add(new MailAddress(destinatario));
                message.Subject = assunto;
                message.Body = corpo;
                message.IsBodyHtml = false;

                using var smtpClient = new SmtpClient(_emailSettings.ServidorSmtp, _emailSettings.PortaSmtp);
                smtpClient.Credentials = new NetworkCredential(_emailSettings.EmailRemetente, _emailSettings.SenhaRemetente);
                smtpClient.EnableSsl = true;
                smtpClient.Timeout = 30000; // 30 segundos

                await smtpClient.SendMailAsync(message);

                _logger.LogInformation($"Email enviado com sucesso para {destinatario}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar email para {destinatario}");
                throw;
            }
        }
    }

    public class EmailSettings
    {
        public string ServidorSmtp { get; set; } = string.Empty;
        public int PortaSmtp { get; set; }
        public string EmailRemetente { get; set; } = string.Empty;
        public string SenhaRemetente { get; set; } = string.Empty;
        public string NomeRemetente { get; set; } = string.Empty;
        public string EmailDestinatario { get; set; } = string.Empty;
    }
}
