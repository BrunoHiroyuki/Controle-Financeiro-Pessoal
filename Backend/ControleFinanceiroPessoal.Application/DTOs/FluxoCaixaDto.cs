using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Application.DTOs
{
    public class FluxoCaixaDto
    {
        public DateTime Data { get; set; }
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal SaldoDia { get; set; }
        public decimal SaldoAcumulado { get; set; }
    }

    public class SaldoDiarioDto
    {
        public decimal SaldoAtual { get; set; }
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }

        public List<FluxoCaixaDto> SaldoDiario { get; set; } = new List<FluxoCaixaDto>();
    }
}
