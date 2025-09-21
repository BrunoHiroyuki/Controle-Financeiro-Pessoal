using ControleFinanceiroPessoal.Domain.Entities;
using FluentAssertions;

namespace ControleFinanceiroPessoal.Domain.Tests.Entities
{
    public class MovimentacaoTests
    {
        [Fact]
        public void Movimentacao_DeveCriarInstanciaComPropriedadesCorretas()
        {
            // Arrange
            var id = 1;
            var tipo = TipoMovimentacao.Receita;
            var data = new DateTime(2024, 1, 15);
            var descricao = "Salário";
            var valor = 5000.00m;
            var inclusao = DateTime.Now;

            // Act
            var movimentacao = new Movimentacao
            {
                Id = id,
                Tipo = tipo,
                Data = data,
                Descricao = descricao,
                Valor = valor,
                Inclusao = inclusao
            };

            // Assert
            movimentacao.Id.Should().Be(id);
            movimentacao.Tipo.Should().Be(tipo);
            movimentacao.Data.Should().Be(data);
            movimentacao.Descricao.Should().Be(descricao);
            movimentacao.Valor.Should().Be(valor);
            movimentacao.Inclusao.Should().Be(inclusao);
            movimentacao.Alteracao.Should().BeNull();
        }

        [Fact]
        public void Movimentacao_DeveInicializarInclusaoComDataAtual()
        {
            // Arrange
            var dataAntes = DateTime.Now;

            // Act
            var movimentacao = new Movimentacao();

            // Assert
            var dataDepois = DateTime.Now;
            movimentacao.Inclusao.Should().BeAfter(dataAntes.AddSeconds(-1));
            movimentacao.Inclusao.Should().BeBefore(dataDepois.AddSeconds(1));
        }

        [Fact]
        public void Movimentacao_DevePermitirAlteracaoDePropriedades()
        {
            // Arrange
            var movimentacao = new Movimentacao
            {
                Tipo = TipoMovimentacao.Despesa,
                Data = new DateTime(2024, 1, 15),
                Descricao = "Mercado",
                Valor = 200.00m
            };

            var novaData = new DateTime(2024, 1, 20);
            var novaDescricao = "Supermercado";
            var novoValor = 250.00m;
            var dataAlteracao = DateTime.Now;

            // Act
            movimentacao.Data = novaData;
            movimentacao.Descricao = novaDescricao;
            movimentacao.Valor = novoValor;
            movimentacao.Alteracao = dataAlteracao;

            // Assert
            movimentacao.Data.Should().Be(novaData);
            movimentacao.Descricao.Should().Be(novaDescricao);
            movimentacao.Valor.Should().Be(novoValor);
            movimentacao.Alteracao.Should().Be(dataAlteracao);
        }

        [Fact]
        public void TipoMovimentacao_DeveConterValoresCorretos()
        {
            // Assert
            ((int)TipoMovimentacao.Receita).Should().Be(1);
            ((int)TipoMovimentacao.Despesa).Should().Be(2);
        }

        [Fact]
        public void Movimentacao_DevePermitirDescricaoAte200Caracteres()
        {
            // Arrange
            var descricaoLonga = new string('A', 200);

            // Act
            var movimentacao = new Movimentacao { Descricao = descricaoLonga };

            // Assert
            movimentacao.Descricao.Should().Be(descricaoLonga);
            movimentacao.Descricao.Length.Should().Be(200);
        }
    }
}
