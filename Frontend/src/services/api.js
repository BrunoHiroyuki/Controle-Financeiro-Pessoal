import axios from 'axios'

// Criar instância do axios com configurações padrão
const apiClient = axios.create({
  baseURL: 'https://localhost:7234/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Interceptor para requests
apiClient.interceptors.request.use(
  (config) => {
    return config
  },
  (error) => {
    console.error('Request Error:', error)
    return Promise.reject(error)
  }
)

// Interceptor para responses
apiClient.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    console.error('Response Error:', error.response?.data || error.message)
    
    // Tratar erros específicos
    if (error.response?.status === 404) {
      console.warn('Recurso não encontrado')
    } else if (error.response?.status === 500) {
      console.error('Erro interno do servidor')
    } else if (error.response?.status === 0 || error.code === 'NETWORK_ERROR') {
      console.error('Erro de conexão - Verifique se a API está rodando')
    }
    
    return Promise.reject(error)
  }
)

export default apiClient
