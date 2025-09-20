using ControleFinanceiroPessoal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Domain.Interfaces
{
    public interface IMovimentacaoRepository
    {
        Task<IEnumerable<Movimentacao>> GetAllAsync();
        Task<Movimentacao?> GetByIdAsync(int id);
        Task<IEnumerable<Movimentacao>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<Movimentacao>> GetByDataAsync(DateTime data);
        Task<decimal> GetSaldoAtualAsync();
        Task<decimal> GetTotalReceitasPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<decimal> GetTotalDespesasPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<Movimentacao> AddAsync(Movimentacao movimentacao);
        Task<Movimentacao> UpdateAsync(Movimentacao movimentacao);
        Task<bool> DeleteAsync(int id);
    }
}
