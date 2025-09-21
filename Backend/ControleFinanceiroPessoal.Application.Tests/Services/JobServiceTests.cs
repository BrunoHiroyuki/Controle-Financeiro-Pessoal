using ControleFinanceiroPessoal.Application.Interfaces;
using ControleFinanceiroPessoal.Application.Services;
using ControleFinanceiroPessoal.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleFinanceiroPessoal.Application.Tests.Services
{
    public class JobServiceTests
    {
        private readonly Mock<IMovimentacaoRepository> _mockMovimentacaoRepository;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<ILogger<JobService>> _mockLogger;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly JobService _jobService;

        public JobServiceTests()
        {
            _mockMovimentacaoRepository = new Mock<IMovimentacaoRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _jobService = new JobService(_mockServiceProvider.Object, _mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task ExecutarVerificacaoSaldoDiarioAsync_ComSaldoNegativo_DeveEnviarEmail()
        {
            // Arrange
            var saldoNegativo = -500.00m;
            _mockMovimentacaoRepository.Setup(r => r.GetSaldoAtualAsync()).ReturnsAsync(saldoNegativo);
            _mockEmailService.Setup(e => e.EnviarNotificacaoSaldoNegativoAsync(saldoNegativo));

            // Act & Assert
            var act = async () => _jobService.ExecutarVerificacaoSaldoDiarioAsync(null);
            await act.Should().NotThrowAsync();
        }
    }
}
