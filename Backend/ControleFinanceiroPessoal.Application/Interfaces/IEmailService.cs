using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarNotificacaoSaldoNegativoAsync(decimal saldoAtual);
        Task EnviarEmailAsync(string destinatario, string assunto, string corpo);
    }
}
