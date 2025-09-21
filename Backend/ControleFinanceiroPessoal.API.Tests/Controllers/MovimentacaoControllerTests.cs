using ControleFinanceiro.API.DTOs;
using ControleFinanceiroPessoal.API.Controllers;
using ControleFinanceiroPessoal.Application.DTOs;
using ControleFinanceiroPessoal.Application.Interfaces;
using ControleFinanceiroPessoal.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ControleFinanceiroPessoal.API.Tests.Controllers
{
    public class MovimentacaoControllerTests
    {
        private readonly Mock<IMovimentacaoService> _mockService;
        private readonly MovimentacaoController _controller;

        public MovimentacaoControllerTests()
        {
            _mockService = new Mock<IMovimentacaoService>();
            _controller = new MovimentacaoController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_DeveRetornarOkComListaDeMovimentacoes()
        {
            // Arrange
            var movimentacoes = new List<MovimentacaoDto>
            {
                new MovimentacaoDto { Id = 1, Tipo = TipoMovimentacao.Receita, Descricao = "Salário", Valor = 5000 },
                new MovimentacaoDto { Id = 2, Tipo = TipoMovimentacao.Despesa, Descricao = "Mercado", Valor = 200 }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(movimentacoes);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedMovimentacoes = okResult.Value.Should().BeAssignableTo<IEnumerable<MovimentacaoDto>>().Subject;
            returnedMovimentacoes.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetById_ComIdExistente_DeveRetornarOkComMovimentacao()
        {
            // Arrange
            var id = 1;
            var movimentacao = new MovimentacaoDto 
            { 
                Id = id, 
                Tipo = TipoMovimentacao.Receita, 
                Descricao = "Salário", 
                Valor = 5000 
            };

            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(movimentacao);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedMovimentacao = okResult.Value.Should().BeOfType<MovimentacaoDto>().Subject;
            returnedMovimentacao.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetById_ComIdInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((MovimentacaoDto?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be($"Movimentação com ID {id} não encontrada.");
        }

        [Fact]
        public async Task GetSaldoDiarioAsync_DeveRetornarOkComSaldoDiario()
        {
            // Arrange
            var saldoDiario = new SaldoDiarioDto
            {
                SaldoAtual = 4800,
                TotalReceitas = 5000,
                TotalDespesas = 200,
                SaldoDiario = new List<FluxoCaixaDto>
                {
                    new FluxoCaixaDto { Data = new DateTime(2024, 1, 15), SaldoDia = 4800, SaldoAcumulado = 4800 }
                }
            };

            _mockService.Setup(s => s.GetSaldoDiarioAsync()).ReturnsAsync(saldoDiario);

            // Act
            var result = await _controller.GetSaldoDiarioAsync();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedSaldo = okResult.Value.Should().BeOfType<SaldoDiarioDto>().Subject;
            returnedSaldo.SaldoAtual.Should().Be(4800);
            returnedSaldo.TotalReceitas.Should().Be(5000);
            returnedSaldo.TotalDespesas.Should().Be(200);
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateMovimentacaoDto
            {
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

            _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(movimentacaoDto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues!["id"].Should().Be(1);
            
            var returnedMovimentacao = createdResult.Value.Should().BeOfType<MovimentacaoDto>().Subject;
            returnedMovimentacao.Id.Should().Be(1);
            returnedMovimentacao.Descricao.Should().Be("Salário");
        }

        [Fact]
        public async Task Create_ComModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var createDto = new CreateMovimentacaoDto();
            _controller.ModelState.AddModelError("Descricao", "A descrição é obrigatória");

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ComIdExistenteEDadosValidos_DeveRetornarOkComMovimentacaoAtualizada()
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

            var movimentacaoDto = new MovimentacaoDto
            {
                Id = id,
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 20),
                Descricao = "Supermercado",
                Valor = 250
            };

            _mockService.Setup(s => s.UpdateAsync(id, updateDto)).ReturnsAsync(movimentacaoDto);

            // Act
            var result = await _controller.Update(id, updateDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedMovimentacao = okResult.Value.Should().BeOfType<MovimentacaoDto>().Subject;
            returnedMovimentacao.Id.Should().Be(id);
            returnedMovimentacao.Descricao.Should().Be("Supermercado");
        }

        [Fact]
        public async Task Update_ComIdInexistente_DeveRetornarNotFound()
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

            _mockService.Setup(s => s.UpdateAsync(id, updateDto)).ReturnsAsync((MovimentacaoDto?)null);

            // Act
            var result = await _controller.Update(id, updateDto);

            // Assert
            var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be($"Movimentação com ID {id} não encontrada.");
        }

        [Fact]
        public async Task Update_ComModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 1;
            var updateDto = new UpdateMovimentacaoDto();
            _controller.ModelState.AddModelError("Descricao", "A descrição é obrigatória");

            // Act
            var result = await _controller.Update(id, updateDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_ComIdExistente_DeveRetornarNoContent()
        {
            // Arrange
            var id = 1;
            _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ComIdInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;
            _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be($"Movimentação com ID {id} não encontrada.");
        }

        [Fact]
        public async Task GetAll_DeveExecutarSemExcecao()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<MovimentacaoDto>());

            // Act & Assert
            var act = async () => await _controller.GetAll();
            await act.Should().NotThrowAsync();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task GetById_ComDiferentesIds_DeveChamarServiceCorretamente(int id)
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((MovimentacaoDto?)null);

            // Act
            await _controller.GetById(id);

            // Assert
            _mockService.Verify(s => s.GetByIdAsync(id), Times.Once);
        }
    }
}
