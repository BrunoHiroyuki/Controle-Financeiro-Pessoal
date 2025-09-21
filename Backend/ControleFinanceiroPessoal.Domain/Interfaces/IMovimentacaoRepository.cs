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
        Task<decimal> GetSaldoAtualAsync();
        Task<Movimentacao> AddAsync(Movimentacao movimentacao);
        Task<Movimentacao> UpdateAsync(Movimentacao movimentacao);
        Task<bool> DeleteAsync(int id);
    }
}
