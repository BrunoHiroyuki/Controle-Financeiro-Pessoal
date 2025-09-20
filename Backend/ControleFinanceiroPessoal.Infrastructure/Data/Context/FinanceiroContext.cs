using ControleFinanceiroPessoal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Infrastructure.Data.Context
{
    public class FinanceiroContext : DbContext
    {
        public FinanceiroContext(DbContextOptions<FinanceiroContext> options) : base(options)
        {
        }

        public DbSet<Movimentacao> Movimentacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data baseado na tabela fornecida
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movimentacao>().HasData(
                new Movimentacao { Id = 1, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2022, 8, 29), Descricao = "Cartão de Crédito", Valor = 825.82m },
                new Movimentacao { Id = 2, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2022, 8, 29), Descricao = "Curso C#", Valor = 200.00m },
                new Movimentacao { Id = 3, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2022, 8, 31), Descricao = "Salário", Valor = 7000.00m },
                new Movimentacao { Id = 4, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2022, 9, 1), Descricao = "Mercado", Valor = 3000.00m },
                new Movimentacao { Id = 5, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2022, 9, 1), Descricao = "Farmácia", Valor = 300.00m },
                new Movimentacao { Id = 6, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2022, 9, 1), Descricao = "Combustível", Valor = 800.25m },
                new Movimentacao { Id = 7, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2022, 9, 15), Descricao = "Financiamento Carro", Valor = 900.00m },
                new Movimentacao { Id = 8, Tipo = TipoMovimentacao.Despesa, Data = new DateTime(2022, 9, 22), Descricao = "Financiamento Casa", Valor = 1200.00m },
                new Movimentacao { Id = 9, Tipo = TipoMovimentacao.Receita, Data = new DateTime(2022, 9, 25), Descricao = "Freelance Projeto XPTO", Valor = 2500.00m }
            );
        }
    }
}
