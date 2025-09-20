import apiClient from './api.js'

// Enum para tipos de movimentação (deve corresponder ao backend)
export const TipoMovimentacao = {
  RECEITA: 1,
  DESPESA: 2
}

// Serviço para operações com movimentações
export const movimentacaoService = {
  /**
   * Buscar todas as movimentações
   */
  async getAll() {
    try {
      const response = await apiClient.get('/movimentacao')
      return response.data
    } catch (error) {
      console.error('Erro ao buscar movimentações:', error)
      throw error
    }
  },

  /**
   * Buscar movimentação por ID
   */
  async getById(id) {
    try {
      const response = await apiClient.get(`/movimentacao/${id}`)
      return response.data
    } catch (error) {
      console.error(`Erro ao buscar movimentação ${id}:`, error)
      throw error
    }
  },

  /**
   * Buscar movimentações por período
   */
  async getByPeriodo(dataInicio, dataFim) {
    try {
      const params = new URLSearchParams({
        dataInicio: dataInicio.toISOString().split('T')[0],
        dataFim: dataFim.toISOString().split('T')[0]
      })
      
      const response = await apiClient.get(`/movimentacao/periodo?${params}`)
      return response.data
    } catch (error) {
      console.error('Erro ao buscar movimentações por período:', error)
      throw error
    }
  },

  /**
   * Buscar movimentações por tipo
   */
  async getByTipo(tipo) {
    try {
      const response = await apiClient.get(`/movimentacao/tipo/${tipo}`)
      return response.data
    } catch (error) {
      console.error(`Erro ao buscar movimentações do tipo ${tipo}:`, error)
      throw error
    }
  },

  /**
   * Buscar resumo financeiro
   */
  async getResumoFinanceiro() {
    try {
      const response = await apiClient.get('/movimentacao/resumo')
      return response.data
    } catch (error) {
      console.error('Erro ao buscar resumo financeiro:', error)
      throw error
    }
  },

  /**
   * Criar nova movimentação
   */
  async create(movimentacao) {
    try {
      // Transformar dados para o formato esperado pela API
      const createDto = {
        tipo: movimentacao.tipo,
        data: new Date(movimentacao.data).toISOString().split('T')[0],
        descricao: movimentacao.descricao,
        valor: parseFloat(movimentacao.valor)
      }

      const response = await apiClient.post('/movimentacao', createDto)
      return response.data
    } catch (error) {
      console.error('Erro ao criar movimentação:', error)
      throw error
    }
  },

  /**
   * Atualizar movimentação existente
   */
  async update(id, movimentacao) {
    try {
      // Transformar dados para o formato esperado pela API
      const updateDto = {
        tipo: movimentacao.tipo,
        data: new Date(movimentacao.data).toISOString().split('T')[0],
        descricao: movimentacao.descricao,
        valor: parseFloat(movimentacao.valor)
      }

      const response = await apiClient.put(`/movimentacao/${id}`, updateDto)
      return response.data
    } catch (error) {
      console.error(`Erro ao atualizar movimentação ${id}:`, error)
      throw error
    }
  },

  /**
   * Excluir movimentação
   */
  async delete(id) {
    try {
      await apiClient.delete(`/movimentacao/${id}`)
      return true
    } catch (error) {
      console.error(`Erro ao excluir movimentação ${id}:`, error)
      throw error
    }
  }
}

// Utilitários para formatação
export const movimentacaoUtils = {
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
    return new Date(dateString).toLocaleDateString('pt-BR')
  },

  /**
   * Converter tipo numérico para texto
   */
  getTipoText(tipo) {
    return tipo === TipoMovimentacao.RECEITA ? 'Receita' : 'Despesa'
  },

  /**
   * Obter cor do tipo para UI
   */
  getTipoColor(tipo) {
    return tipo === TipoMovimentacao.RECEITA ? 'success' : 'error'
  }
}
