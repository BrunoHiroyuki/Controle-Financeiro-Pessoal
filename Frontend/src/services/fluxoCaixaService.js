import apiClient from './api.js'

// Serviço para operações com fluxo de caixa
export const fluxoCaixaService = {
  /**
   * Buscar saldo diário (novo endpoint)
   */
  async getSaldoDiario() {
    try {
      const response = await apiClient.get(`/movimentacao/fluxo-caixa`)
      return response.data
    } catch (error) {
      console.error('Erro ao buscar saldo diário:', error)
      throw error
    }
  }

}

// Utilitários para formatação
export const fluxoCaixaUtils = {
  /**
   * Formatar valor para moeda brasileira
   */
  formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value)
  },

  /**
   * Formatar data para exibição
   */
  formatDate(dateString) {
    return new Date(dateString).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: '2-digit'
    })
  },

  /**
   * Calcular totais do período
   */
  calcularTotais(dados) {
    return dados.reduce((acc, item) => ({
      totalReceitas: acc.totalReceitas + item.receitas,
      totalDespesas: acc.totalDespesas + item.despesas,
      saldoFinal: item.saldoAcumulado // Último saldo
    }), { totalReceitas: 0, totalDespesas: 0, saldoFinal: 0 })
  }
}
