using AutoMapper;
using ControleFinanceiro.API.DTOs;
using ControleFinanceiroPessoal.Application.DTOs;
using ControleFinanceiroPessoal.Application.Services;
using ControleFinanceiroPessoal.Domain.Entities;
using ControleFinanceiroPessoal.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ControleFinanceiroPessoal.Application.Tests.Services
{
    public class MovimentacaoServiceTests
    {
        private readonly Mock<IMovimentacaoRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MovimentacaoService _service;

        public MovimentacaoServiceTests()
        {
            _mockRepository = new Mock<IMovimentacaoRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new MovimentacaoService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeMovimentacoes()
        {
            // Arrange
            var movimentacoes = new List<Movimentacao>
            {
                new Movimentacao { Id = 1, Tipo = TipoMovimentacao.Receita, Descricao = "Salário", Valor = 5000 },
                new Movimentacao { Id = 2, Tipo = TipoMovimentacao.Despesa, Descricao = "Mercado", Valor = 200 }
            };

            var movimentacoesDto = new List<MovimentacaoDto>
            {
                new MovimentacaoDto { Id = 1, Tipo = TipoMovimentacao.Receita, Descricao = "Salário", Valor = 5000 },
                new MovimentacaoDto { Id = 2, Tipo = TipoMovimentacao.Despesa, Descricao = "Mercado", Valor = 200 }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(movimentacoes);
            _mockMapper.Setup(m => m.Map<IEnumerable<MovimentacaoDto>>(movimentacoes)).Returns(movimentacoesDto);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(movimentacoesDto);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdExistente_DeveRetornarMovimentacao()
        {
            // Arrange
            var id = 1;
            var movimentacao = new Movimentacao { Id = id, Tipo = TipoMovimentacao.Receita, Descricao = "Salário", Valor = 5000 };
            var movimentacaoDto = new MovimentacaoDto { Id = id, Tipo = TipoMovimentacao.Receita, Descricao = "Salário", Valor = 5000 };

            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(movimentacao);
            _mockMapper.Setup(m => m.Map<MovimentacaoDto>(movimentacao)).Returns(movimentacaoDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(movimentacaoDto);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var id = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Movimentacao?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetSaldoDiarioAsync_DeveCalcularSaldoCorretamente()
        {
            // Arrange
            var movimentacoes = new List<Movimentacao>
            {
                new Movimentacao { Id = 1, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 15), Descricao = "Salário", Valor = 5000 },
                new Movimentacao { Id = 2, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2024, 1, 15), Descricao = "Mercado", Valor = 200 },
                new Movimentacao { Id = 3, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2024, 1, 16), Descricao = "Freelance", Valor = 1000 },
                new Movimentacao { Id = 4, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2024, 1, 16), Descricao = "Combustível", Valor = 100 }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(movimentacoes);

            // Act
            var result = await _service.GetSaldoDiarioAsync();

            // Assert
            result.Should().NotBeNull();
            result.SaldoAtual.Should().Be(5700); // 5000 + 1000 - 200 - 100
            result.TotalReceitas.Should().Be(6000); // 5000 + 1000
            result.TotalDespesas.Should().Be(300); // 200 + 100
            result.SaldoDiario.Should().HaveCount(2);
            
            var primeiroDia = result.SaldoDiario.First(s => s.Data == new DateTime(2024, 1, 15));
            primeiroDia.TotalReceitas.Should().Be(5000);
            primeiroDia.TotalDespesas.Should().Be(200);
            primeiroDia.SaldoDia.Should().Be(4800);
            primeiroDia.SaldoAcumulado.Should().Be(4800);

            var segundoDia = result.SaldoDiario.First(s => s.Data == new DateTime(2024, 1, 16));
            segundoDia.TotalReceitas.Should().Be(1000);
            segundoDia.TotalDespesas.Should().Be(100);
            segundoDia.SaldoDia.Should().Be(900);
            segundoDia.SaldoAcumulado.Should().Be(5700);
        }

        [Fact]
        public async Task CreateAsync_DeveCriarNovaMovimentacao()
        {
            // Arrange
            var createDto = new CreateMovimentacaoDto
            {
                Tipo = TipoMovimentacao.Receita,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Salário",
                Valor = 5000
            };

            var movimentacao = new Movimentacao
            {
                Tipo = TipoMovimentacao.Receita,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Salário",
                Valor = 5000
            };

            var createdMovimentacao = new Movimentacao
            {
                Id = 1,
                Tipo = TipoMovimentacao.Receita,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Salário",
                Valor = 5000
            };

            var movimentacaoDto = new MovimentacaoDto
            {
                Id = 1,
                Tipo = TipoMovimentacao.Receita,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Salário",
                Valor = 5000
            };

            _mockMapper.Setup(m => m.Map<Movimentacao>(createDto)).Returns(movimentacao);
            _mockRepository.Setup(r => r.AddAsync(movimentacao)).ReturnsAsync(createdMovimentacao);
            _mockMapper.Setup(m => m.Map<MovimentacaoDto>(createdMovimentacao)).Returns(movimentacaoDto);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(movimentacaoDto);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Movimentacao>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComIdExistente_DeveAtualizarMovimentacao()
        {
            // Arrange
            var id = 1;
            var updateDto = new UpdateMovimentacaoDto
            {
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 20),
                Descricao = "Supermercado",
                Valor = 250
            };

            var existingMovimentacao = new Movimentacao
            {
                Id = id,
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Mercado",
                Valor = 200
            };

            var updatedMovimentacao = new Movimentacao
            {
                Id = id,
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 20),
                Descricao = "Supermercado",
                Valor = 250
            };

            var movimentacaoDto = new MovimentacaoDto
            {
                Id = id,
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 20),
                Descricao = "Supermercado",
                Valor = 250
            };

            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingMovimentacao);
            _mockMapper.Setup(m => m.Map(updateDto, existingMovimentacao));
            _mockRepository.Setup(r => r.UpdateAsync(existingMovimentacao)).ReturnsAsync(updatedMovimentacao);
            _mockMapper.Setup(m => m.Map<MovimentacaoDto>(updatedMovimentacao)).Returns(movimentacaoDto);

            // Act
            var result = await _service.UpdateAsync(id, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(movimentacaoDto);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Movimentacao>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var id = 999;
            var updateDto = new UpdateMovimentacaoDto
            {
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 20),
                Descricao = "Supermercado",
                Valor = 250
            };

            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Movimentacao?)null);

            // Act
            var result = await _service.UpdateAsync(id, updateDto);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Movimentacao>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ComIdExistente_DeveRetornarTrue()
        {
            // Arrange
            var id = 1;
            var movimentacao = new Movimentacao
            {
                Id = id,
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Mercado",
                Valor = 200
            };

            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(movimentacao);
            _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
        {
            // Arrange
            var id = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Movimentacao?)null);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
