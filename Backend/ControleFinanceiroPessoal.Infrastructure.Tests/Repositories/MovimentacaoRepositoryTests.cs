using ControleFinanceiroPessoal.Domain.Entities;
using ControleFinanceiroPessoal.Infrastructure.Data.Context;
using ControleFinanceiroPessoal.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiroPessoal.Infrastructure.Tests.Repositories
{
    public class MovimentacaoRepositoryTests : IDisposable
    {
        private readonly FinanceiroContext _context;
        private readonly MovimentacaoRepository _repository;

        public MovimentacaoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FinanceiroContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FinanceiroContext(options);
            _repository = new MovimentacaoRepository(_context);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodasMovimentacoesOrdenadas()
        {
            // Arrange
            var movimentacoes = new List<Movimentacao>
            {
                new Movimentacao { Id = 1, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 15), Descricao = "Salário", Valor = 5000 },
                new Movimentacao { Id = 2, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2024, 1, 16), Descricao = "Mercado", Valor = 200 },
                new Movimentacao { Id = 3, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 14), Descricao = "Freelance", Valor = 1000 }
            };

            await _context.Movimentacoes.AddRangeAsync(movimentacoes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            
            var resultList = result.ToList();
            resultList[0].Data.Should().Be(new DateTime(2024, 1, 16)); // Mais recente primeiro
            resultList[1].Data.Should().Be(new DateTime(2024, 1, 15));
            resultList[2].Data.Should().Be(new DateTime(2024, 1, 14));
        }

        [Fact]
        public async Task GetByIdAsync_ComIdExistente_DeveRetornarMovimentacao()
        {
            // Arrange
            var movimentacao = new Movimentacao 
            { 
                Id = 1, 
                Tipo = TipoMovimentacao.Receita, 
                Data = new DateTime(2024, 1, 15), 
                Descricao = "Salário", 
                Valor = 5000 
            };

            await _context.Movimentacoes.AddAsync(movimentacao);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Descricao.Should().Be("Salário");
            result.Valor.Should().Be(5000);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetSaldoAtualAsync_DeveCalcularSaldoCorretamente()
        {
            // Arrange
            var movimentacoes = new List<Movimentacao>
            {
                new Movimentacao { Id = 1, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 15), Descricao = "Salário", Valor = 5000 },
                new Movimentacao { Id = 2, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 16), Descricao = "Freelance", Valor = 1500 },
                new Movimentacao { Id = 3, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2024, 1, 17), Descricao = "Mercado", Valor = 300 },
                new Movimentacao { Id = 4, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2024, 1, 18), Descricao = "Combustível", Valor = 200 }
            };

            await _context.Movimentacoes.AddRangeAsync(movimentacoes);
            await _context.SaveChangesAsync();

            // Act
            var saldo = await _repository.GetSaldoAtualAsync();

            // Assert
            saldo.Should().Be(6000); // (5000 + 1500) - (300 + 200) = 6000
        }

        [Fact]
        public async Task GetTotalReceitasPeriodoAsync_DeveCalcularTotalReceitas()
        {
            // Arrange
            var movimentacoes = new List<Movimentacao>
            {
                new Movimentacao { Id = 1, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 10), Descricao = "Anterior", Valor = 1000 },
                new Movimentacao { Id = 2, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 15), Descricao = "No período", Valor = 2000 },
                new Movimentacao { Id = 3, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 20), Descricao = "No período", Valor = 1500 },
                new Movimentacao { Id = 4, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2024, 1, 18), Descricao = "Despesa no período", Valor = 500 }
            };

            await _context.Movimentacoes.AddRangeAsync(movimentacoes);
            await _context.SaveChangesAsync();

            var dataInicio = new DateTime(2024, 1, 15);
            var dataFim = new DateTime(2024, 1, 20);

            // Act
            var totalReceitas = await _repository.GetTotalReceitasPeriodoAsync(dataInicio, dataFim);

            // Assert
            totalReceitas.Should().Be(3500); // 2000 + 1500
        }

        [Fact]
        public async Task AddAsync_DeveAdicionarNovaMovimentacao()
        {
            // Arrange
            var novaMovimentacao = new Movimentacao
            {
                Tipo = TipoMovimentacao.Receita,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Nova receita",
                Valor = 1000
            };

            // Act
            var result = await _repository.AddAsync(novaMovimentacao);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            
            var movimentacaoNoBanco = await _context.Movimentacoes.FindAsync(result.Id);
            movimentacaoNoBanco.Should().NotBeNull();
            movimentacaoNoBanco!.Descricao.Should().Be("Nova receita");
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarMovimentacaoExistente()
        {
            // Arrange
            var movimentacao = new Movimentacao
            {
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Descrição original",
                Valor = 100
            };

            await _context.Movimentacoes.AddAsync(movimentacao);
            await _context.SaveChangesAsync();

            // Modificar a movimentação
            movimentacao.Descricao = "Descrição atualizada";
            movimentacao.Valor = 200;

            // Act
            var result = await _repository.UpdateAsync(movimentacao);

            // Assert
            result.Should().NotBeNull();
            result.Descricao.Should().Be("Descrição atualizada");
            result.Valor.Should().Be(200);
            result.Alteracao.Should().NotBeNull();
            result.Alteracao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task DeleteAsync_ComIdExistente_DeveRemoverMovimentacao()
        {
            // Arrange
            var movimentacao = new Movimentacao
            {
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Para deletar",
                Valor = 100
            };

            await _context.Movimentacoes.AddAsync(movimentacao);
            await _context.SaveChangesAsync();
            var id = movimentacao.Id;

            // Act
            var result = await _repository.DeleteAsync(id);

            // Assert
            result.Should().BeTrue();
            
            var movimentacaoNoBanco = await _context.Movimentacoes.FindAsync(id);
            movimentacaoNoBanco.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
