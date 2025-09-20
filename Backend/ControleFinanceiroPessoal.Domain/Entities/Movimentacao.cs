using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiroPessoal.Domain.Entities
{
    public class Movimentacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TipoMovimentacao Tipo { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public decimal Valor { get; set; }

        [Required]
        public DateTime Inclusao { get; set; } = DateTime.Now;

        public DateTime? Alteracao { get; set; }
    }

    public enum TipoMovimentacao
    {
        Receita = 1,
        Despesa = 2
    }
}
