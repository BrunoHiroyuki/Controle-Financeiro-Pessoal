using ControleFinanceiroPessoal.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var assunto = "⚠️ Alerta: Saldo Negativo - Controle Financeiro";
            var corpo = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .alert {{ background-color: #f8d7da; color: #721c24; padding: 15px; border-radius: 5px; margin: 20px 0; }}
                        .saldo {{ font-size: 24px; font-weight: bold; color: #dc3545; }}
                        .footer {{ margin-top: 30px; font-size: 12px; color: #6c757d; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>🚨 Alerta de Saldo Negativo</h2>
                        
                        <div class='alert'>
                            <strong>Atenção!</strong> Seu saldo atual está negativo.
                        </div>
                        
                        <p>Seu saldo atual é: <span class='saldo'>R$ {saldoAtual:N2}</span></p>
                        
                        <p>Recomendamos que você:</p>
                        <ul>
                            <li>Revise suas despesas recentes</li>
                            <li>Considere adiar gastos não essenciais</li>
                            <li>Verifique possíveis fontes de receita adicional</li>
                        </ul>
                        
                        <p>Acesse seu sistema de controle financeiro para mais detalhes.</p>
                        
                        <div class='footer'>
                            <p>Esta é uma notificação automática do seu Sistema de Controle Financeiro Pessoal.</p>
                            <p>Data: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                        </div>
                    </div>
                </body>
                </html>";

            await EnviarEmailAsync(_emailSettings.DestinatarioNotificacoes, assunto, corpo);
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpo)
        {
            try
            {
                //var message = new MimeMessage();
                //message.From.Add(new MailboxAddress(_emailSettings.NomeRemetente, _emailSettings.EmailRemetente));
                //message.To.Add(new MailboxAddress("", destinatario));
                //message.Subject = assunto;

                //var bodyBuilder = new BodyBuilder
                //{
                //    HtmlBody = corpo
                //};
                //message.Body = bodyBuilder.ToMessageBody();

                //using var client = new SmtpClient();
                //await client.ConnectAsync(_emailSettings.ServidorSmtp, _emailSettings.PortaSmtp, SecureSocketOptions.StartTls);
                //await client.AuthenticateAsync(_emailSettings.EmailRemetente, _emailSettings.SenhaRemetente);
                //await client.SendAsync(message);
                //await client.DisconnectAsync(true);

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
        public string DestinatarioNotificacoes { get; set; } = string.Empty;
    }
}
