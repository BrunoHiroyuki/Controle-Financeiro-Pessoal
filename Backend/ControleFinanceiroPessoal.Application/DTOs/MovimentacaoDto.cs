using ControleFinanceiroPessoal.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.API.DTOs
{
    public class MovimentacaoDto
    {
        public int Id { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    public class CreateMovimentacaoDto
    {
        [Required]
        public TipoMovimentacao Tipo { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }
    }

    public class UpdateMovimentacaoDto
    {
        [Required]
        public TipoMovimentacao Tipo { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }
    }
}
