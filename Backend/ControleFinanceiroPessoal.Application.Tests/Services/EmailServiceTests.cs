using ControleFinanceiroPessoal.Application.Interfaces;
using ControleFinanceiroPessoal.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ControleFinanceiroPessoal.Application.Tests.Services
{
    public class EmailServiceTests
    {
        private readonly Mock<IOptions<EmailSettings>> _mockEmailSettings;
        private readonly Mock<ILogger<EmailService>> _mockLogger;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _mockEmailSettings = new Mock<IOptions<EmailSettings>>();
            _mockLogger = new Mock<ILogger<EmailService>>();
            
            SetupEmailSettings();
            _emailService = new EmailService(_mockEmailSettings.Object, _mockLogger.Object);
        }

        private void SetupEmailSettings()
        {
            var emailSettings = new EmailSettings
            {
                ServidorSmtp = "smtp.gmail.com",
                PortaSmtp = 587,
                EmailRemetente = "hiroyuki.bruno@gmail.com",
                SenhaRemetente = "testpassword",
                NomeRemetente = "Controle Financeiro Pessoal",
                EmailDestinatario = "test-controle-financeiro@gmail.com"
            };

            _mockEmailSettings.Setup(x => x.Value).Returns(emailSettings);
        }

        [Fact]
        public void EmailService_DeveCriarInstanciaComConfiguracaoCorreta()
        {
            // Act & Assert
            _emailService.Should().NotBeNull();
        }

        [Fact]
        public async Task EnviarEmailAsync_ComParametrosValidos_DeveExecutarSemExcecao()
        {
            // Arrange
            var destinatario = "teste@email.com";
            var assunto = "Teste";
            var corpo = "Teste";

            // Act & Assert
            var act = async () => await _emailService.EnviarEmailAsync(destinatario, assunto, corpo);
            await act.Should().NotThrowAsync();
        }
    }
}
