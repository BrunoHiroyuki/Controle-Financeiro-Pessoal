using ControleFinanceiroPessoal.Application.Interfaces;
using ControleFinanceiroPessoal.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Infrastructure.Services
{
    public class JobService : IJobService
    {
        private readonly IEmailService _emailService;
        private readonly IMovimentacaoRepository _movimentacaoRepository;
        private readonly ILogger<JobService> _logger;

        public JobService(
            IEmailService emailService,
            IMovimentacaoRepository movimentacaoRepository,
            ILogger<JobService> logger)
        {
            _emailService = emailService;
            _movimentacaoRepository = movimentacaoRepository;
            _logger = logger;
        }

        public async Task ExecutarVerificacaoSaldoDiarioAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando verificação de saldo diário...");

                var saldo = await _movimentacaoRepository.GetSaldoAtualAsync();

                if (saldo < 0)
                {
                    var saldoAtual = await _movimentacaoRepository.GetSaldoAtualAsync();
                    await _emailService.EnviarNotificacaoSaldoNegativoAsync(saldoAtual);

                    _logger.LogWarning($"Saldo negativo detectado: R$ {saldoAtual:N2}. Notificação enviada por e-mail.");
                }
                else
                {
                    _logger.LogInformation("Saldo positivo. Nenhuma notificação necessária.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a verificação de saldo diário");
                throw;
            }
        }
    }
}
