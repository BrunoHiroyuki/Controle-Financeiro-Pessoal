using AutoMapper;
using ControleFinanceiro.API.DTOs;
using ControleFinanceiroPessoal.Application.DTOs;
using ControleFinanceiroPessoal.Application.Interfaces;
using ControleFinanceiroPessoal.Domain.Entities;
using ControleFinanceiroPessoal.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Application.Services
{
    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly IMovimentacaoRepository _movimentacaoRepository;
        private readonly IMapper _mapper;

        public MovimentacaoService(
            IMovimentacaoRepository movimentacaoRepository,
            IMapper mapper)
        {
            _movimentacaoRepository = movimentacaoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MovimentacaoDto>> GetAllAsync()
        {
            var movimentacoes = await _movimentacaoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MovimentacaoDto>>(movimentacoes);
        }

        public async Task<MovimentacaoDto?> GetByIdAsync(int id)
        {
            var movimentacao = await _movimentacaoRepository.GetByIdAsync(id);
            return movimentacao != null ? _mapper.Map<MovimentacaoDto>(movimentacao) : null;
        }

        public async Task<SaldoDiarioDto> GetSaldoDiarioAsync()
        {
            var movimentacoes = await _movimentacaoRepository.GetAllAsync();

            var receitas = movimentacoes.Where(w => w.Tipo == TipoMovimentacao.Receita).Sum(s => s.Valor);
            var despesas = movimentacoes.Where(w => w.Tipo == TipoMovimentacao.Despesa).Sum(s => s.Valor);

            var retorno = new SaldoDiarioDto()
            {
                SaldoAtual = receitas - despesas,
                TotalDespesas = despesas,
                TotalReceitas = receitas,
            };

            // Agrupar movimentações por data
            var movimentacoesPorData = movimentacoes
                .GroupBy(m => m.Data.Date)
                .OrderBy(g => g.Key)
                .ToList();

            var saldoDiario = new List<FluxoCaixaDto>();
            decimal saldoAcumulado = 0;

            foreach(var data in movimentacoesPorData)
            {
                var movimentacoesDoDia = movimentacoesPorData.FirstOrDefault(g => g.Key == data.Key);

                decimal totalReceitas = 0;
                decimal totalDespesas = 0;

                if (movimentacoesDoDia != null)
                {
                    totalReceitas = movimentacoesDoDia.Where(m => m.Tipo == TipoMovimentacao.Receita).Sum(m => m.Valor);

                    totalDespesas = movimentacoesDoDia.Where(m => m.Tipo == TipoMovimentacao.Despesa).Sum(m => m.Valor);
                }

                var saldoDia = totalReceitas - totalDespesas;
                saldoAcumulado += saldoDia;

                saldoDiario.Add(new FluxoCaixaDto
                {
                    Data = data.Key,
                    TotalReceitas = totalReceitas,
                    TotalDespesas = totalDespesas,
                    SaldoDia = saldoDia,
                    SaldoAcumulado = saldoAcumulado
                });

            }

            retorno.SaldoDiario = saldoDiario;

            return retorno;
        }

        public async Task<MovimentacaoDto> CreateAsync(CreateMovimentacaoDto createDto)
        {
            var movimentacao = _mapper.Map<Movimentacao>(createDto);
            var createdMovimentacao = await _movimentacaoRepository.AddAsync(movimentacao);

            return _mapper.Map<MovimentacaoDto>(createdMovimentacao);
        }

        public async Task<MovimentacaoDto?> UpdateAsync(int id, UpdateMovimentacaoDto updateDto)
        {
            var existingMovimentacao = await _movimentacaoRepository.GetByIdAsync(id);
            if (existingMovimentacao == null)
                return null;

            var dataAnterior = existingMovimentacao.Data;
            _mapper.Map(updateDto, existingMovimentacao);

            var updatedMovimentacao = await _movimentacaoRepository.UpdateAsync(existingMovimentacao);

            return _mapper.Map<MovimentacaoDto>(updatedMovimentacao);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movimentacao = await _movimentacaoRepository.GetByIdAsync(id);
            if (movimentacao == null)
                return false;

            var dataMovimentacao = movimentacao.Data;
            var deleted = await _movimentacaoRepository.DeleteAsync(id);

            return deleted;
        }
    }
}
