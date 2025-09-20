using ControleFinanceiroPessoal.Domain.Entities;
using ControleFinanceiroPessoal.Domain.Interfaces;
using ControleFinanceiroPessoal.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Infrastructure.Repositories
{
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        private readonly FinanceiroContext _context;

        public MovimentacaoRepository(FinanceiroContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movimentacao>> GetAllAsync()
        {
            return await _context.Movimentacoes
                .OrderByDescending(m => m.Data)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<Movimentacao?> GetByIdAsync(int id)
        {
            return await _context.Movimentacoes.FindAsync(id);
        }

        public async Task<IEnumerable<Movimentacao>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Movimentacoes
                .Where(m => m.Data >= dataInicio && m.Data <= dataFim)
                .OrderByDescending(m => m.Data)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimentacao>> GetByDataAsync(DateTime data)
        {
            return await _context.Movimentacoes
                .Where(m => m.Data.Date == data.Date)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<decimal> GetSaldoAtualAsync()
        {
            var totalReceitas = await _context.Movimentacoes
                .Where(m => m.Tipo == TipoMovimentacao.Receita)
                .SumAsync(m => m.Valor);

            var totalDespesas = await _context.Movimentacoes
                .Where(m => m.Tipo == TipoMovimentacao.Despesa)
                .SumAsync(m => m.Valor);

            return totalReceitas - totalDespesas;
        }

        public async Task<decimal> GetTotalReceitasPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Movimentacoes
                .Where(m => m.Tipo == TipoMovimentacao.Receita &&
                           m.Data >= dataInicio && m.Data <= dataFim)
                .SumAsync(m => m.Valor);
        }

        public async Task<decimal> GetTotalDespesasPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Movimentacoes
                .Where(m => m.Tipo == TipoMovimentacao.Despesa &&
                           m.Data >= dataInicio && m.Data <= dataFim)
                .SumAsync(m => m.Valor);
        }

        public async Task<Movimentacao> AddAsync(Movimentacao movimentacao)
        {
            _context.Movimentacoes.Add(movimentacao);
            await _context.SaveChangesAsync();
            return movimentacao;
        }

        public async Task<Movimentacao> UpdateAsync(Movimentacao movimentacao)
        {
            movimentacao.Alteracao = DateTime.Now;
            _context.Entry(movimentacao).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return movimentacao;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movimentacao = await _context.Movimentacoes.FindAsync(id);
            if (movimentacao == null)
                return false;

            _context.Movimentacoes.Remove(movimentacao);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
