using ControleFinanceiro.API.DTOs;
using ControleFinanceiroPessoal.Application.DTOs;
using ControleFinanceiroPessoal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Application.Interfaces
{
    public interface IMovimentacaoService
    {
        Task<IEnumerable<MovimentacaoDto>> GetAllAsync();
        Task<MovimentacaoDto?> GetByIdAsync(int id);
        Task<SaldoDiarioDto> GetSaldoDiarioAsync();
        Task<MovimentacaoDto> CreateAsync(CreateMovimentacaoDto createDto);
        Task<MovimentacaoDto?> UpdateAsync(int id, UpdateMovimentacaoDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}
