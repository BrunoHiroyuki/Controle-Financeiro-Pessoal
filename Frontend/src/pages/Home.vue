<template>
  <div style="padding: 20px; background-color: #f5f6fa; min-height: 100vh;">
    <!-- Indicadores -->
    <n-grid cols="3" x-gap="16" y-gap="16">
      <n-gi>
        <n-card :bordered="false" style="background: linear-gradient(135deg, #6a11cb, #2575fc); color: white;">
          <div style="display: flex; align-items: center; gap: 10px;">
            <n-icon size="28"><WalletOutline /></n-icon>
            <div>
              <div style="font-size: 16px;">Saldo Atual</div>
              <div style="font-size: 20px; font-weight: bold;">
                {{ formatCurrency(indicadores.saldoAtual) }}
              </div>
            </div>
          </div>
        </n-card>
      </n-gi>

      <n-gi>
        <n-card :bordered="false" style="background: #16a34a; color: white;">
          <div style="display: flex; align-items: center; gap: 10px;">
            <n-icon size="28"><ArrowUpCircleOutline /></n-icon>
            <div>
              <div style="font-size: 16px;">Receitas</div>
              <div style="font-size: 20px; font-weight: bold;">
                {{ formatCurrency(indicadores.totalReceitas) }}
              </div>
            </div>
          </div>
        </n-card>
      </n-gi>

      <n-gi>
        <n-card :bordered="false" style="background: #dc2626; color: white;">
          <div style="display: flex; align-items: center; gap: 10px;">
            <n-icon size="28"><ArrowDownCircleOutline /></n-icon>
            <div>
              <div style="font-size: 16px;">Despesas</div>
              <div style="font-size: 20px; font-weight: bold;">
                {{ formatCurrency(indicadores.totalDespesas) }}
              </div>
            </div>
          </div>
        </n-card>
      </n-gi>
    </n-grid>

    <!-- Gráfico de Saldo Diário -->
    <SaldoDiarioChart 
      @dados-carregados="atualizarIndicadores"
      ref="chartRef"
    />

    <!-- Botão para recarregar dados -->
    <div style="text-align: center; margin-top: 20px;">
      <n-button 
        type="primary" 
        @click="recarregarTodosDados"
        :loading="carregandoIndicadores"
      >
        <template #icon>
          <n-icon><RefreshOutline /></n-icon>
        </template>
        Atualizar Dados
      </n-button>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { NCard, NGrid, NGi, NButton, NIcon, useMessage } from 'naive-ui'
import { 
  WalletOutline, 
  ArrowUpCircleOutline, 
  ArrowDownCircleOutline, 
  RefreshOutline 
} from '@vicons/ionicons5'
import SaldoDiarioChart from '../components/SaldoDiarioChart.vue'
import { fluxoCaixaService, fluxoCaixaUtils } from '../services/fluxoCaixaService.js'

// Composables
const message = useMessage()

// Refs
const chartRef = ref(null)
const carregandoIndicadores = ref(false)

// Estado reativo
const indicadores = reactive({
  saldoAtual: 0,
  totalReceitas: 0,
  totalDespesas: 0
})

// Métodos
const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

const atualizarIndicadores = (dadosGrafico) => {
  indicadores.saldoAtual = dadosGrafico.saldoFinal || 0
  indicadores.totalReceitas = dadosGrafico.totalReceitas || 0
  indicadores.totalDespesas = dadosGrafico.totalDespesas || 0
}

const carregarResumoFinanceiro = async () => {
  carregandoIndicadores.value = true
  
  try {
    const resumo = await fluxoCaixaService.getSaldoDiario()
    
    indicadores.saldoAtual = resumo.saldoAtual || 0
    indicadores.totalReceitas = resumo.totalReceitas || 0
    indicadores.totalDespesas = resumo.totalDespesas || 0
    
    message.success('Dados atualizados com sucesso!')
  } catch (error) {
    console.warn('Erro ao carregar resumo da API, usando dados do gráfico:', error)
    message.warning('API não disponível')
  } finally {
    carregandoIndicadores.value = false
  }
}

const recarregarTodosDados = async () => {
  // Recarregar gráfico
  if (chartRef.value) {
    await chartRef.value.recarregarDados()
  }
  
  // Recarregar resumo financeiro
  await carregarResumoFinanceiro()
}

// Lifecycle
onMounted(() => {
  carregarResumoFinanceiro()
})
</script>

<style scoped>
/* Estilos adicionais se necessário */
</style>
