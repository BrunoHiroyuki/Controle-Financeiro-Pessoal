using ControleFinanceiro.API.DTOs;
using ControleFinanceiroPessoal.Application.DTOs;
using ControleFinanceiroPessoal.Application.Interfaces;
using ControleFinanceiroPessoal.Application.Services;
using ControleFinanceiroPessoal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiroPessoal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentacaoController : ControllerBase
    {
        private readonly IMovimentacaoService _movimentacaoService;

        public MovimentacaoController(IMovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        /// <summary>
        /// Obtém todas as movimentações
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimentacaoDto>>> GetAll()
        {
            var movimentacoes = await _movimentacaoService.GetAllAsync();
            return Ok(movimentacoes);
        }

        /// <summary>
        /// Obtém uma movimentação por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimentacaoDto>> GetById(int id)
        {
            var movimentacao = await _movimentacaoService.GetByIdAsync(id);

            if (movimentacao == null)
                return NotFound($"Movimentação com ID {id} não encontrada.");

            return Ok(movimentacao);
        }

        ///// <summary>
        ///// Obtém resumo financeiro atual
        ///// </summary>
        [HttpGet("fluxo-caixa")]
        public async Task<ActionResult<FluxoCaixaDto>> GetSaldoDiarioAsync()
        {
            var resumo = await _movimentacaoService.GetSaldoDiarioAsync();
            return Ok(resumo);
        }

        /// <summary>
        /// Cria uma nova movimentação
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MovimentacaoDto>> Create([FromBody] CreateMovimentacaoDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var movimentacao = await _movimentacaoService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = movimentacao.Id }, movimentacao);
        }

        /// <summary>
        /// Atualiza uma movimentação existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MovimentacaoDto>> Update(int id, [FromBody] UpdateMovimentacaoDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var movimentacao = await _movimentacaoService.UpdateAsync(id, updateDto);

            if (movimentacao == null)
                return NotFound($"Movimentação com ID {id} não encontrada.");

            return Ok(movimentacao);
        }

        /// <summary>
        /// Remove uma movimentação
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _movimentacaoService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Movimentação com ID {id} não encontrada.");

            return NoContent();
        }
    }
}
