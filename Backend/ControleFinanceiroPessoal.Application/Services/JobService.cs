using ControleFinanceiroPessoal.Application.Interfaces;
using ControleFinanceiroPessoal.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Application.Services
{
    public class JobService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobService> _logger;
        private readonly IConfiguration _configuration;
        private Timer? _timer;
        private bool _disposed = false;

        public JobService(
            IServiceProvider serviceProvider,
            ILogger<JobService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public async void ExecutarVerificacaoSaldoDiarioAsync(object? state)
        {
            try
            {
                _logger.LogInformation("Iniciando verificação de saldo diário...");

                using var scope = _serviceProvider.CreateScope();
                var movimentacaoRepository = scope.ServiceProvider.GetRequiredService<IMovimentacaoRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var saldo = await movimentacaoRepository.GetSaldoAtualAsync();

                if (saldo < 0)
                {
                    await emailService.EnviarNotificacaoSaldoNegativoAsync(saldo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a verificação de saldo diário");
                throw;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Cria o timer com a primeira execução no tempo calculado e depois a cada 24 horas
            _timer = new Timer(ExecutarVerificacaoSaldoDiarioAsync, null, TimeSpan.Zero, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _timer?.Dispose();
                _disposed = true;
            }
        }
    }
}
