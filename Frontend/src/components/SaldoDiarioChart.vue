<template>
  <div class="chart-container">
    <n-card :bordered="false" style="margin-top: 20px;">
      
      <div v-if="loading" style="text-align: center; padding: 40px;">
        <n-spin size="large" />
        <div style="margin-top: 10px;">Carregando dados...</div>
      </div>

      <div v-else-if="error" style="text-align: center; padding: 40px;">
        <n-alert type="error" :show-icon="false">
          {{ error }}
        </n-alert>
      </div>

      <div v-else style="height: 400px; position: relative;">
        <Line
          :data="chartData"
          :options="chartOptions"
          style="max-height: 400px;"
        />
      </div>
    </n-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { NCard, NButton, NSpace, NSpin, NAlert } from 'naive-ui'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'
import { Line } from 'vue-chartjs'
import { fluxoCaixaService, fluxoCaixaUtils } from '../services/fluxoCaixaService.js'

// Registrar componentes do Chart.js
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

// Props
const props = defineProps({
  altura: {
    type: Number,
    default: 400
  }
})

// Emits
const emit = defineEmits(['dadosCarregados'])

// Estado reativo
const loading = ref(false)
const error = ref(null)
const dados = ref([])
const periodo = ref(30)

// Dados do gráfico
const chartData = computed(() => {
  if (!dados.value.length) return { labels: [], datasets: [] }

  const labels = dados.value.map(item => fluxoCaixaUtils.formatDate(item.data))
  const saldos = dados.value.map(item => item.saldoAcumulado)

  return {
    labels,
    datasets: [
      {
        label: 'Saldo Acumulado',
        data: saldos,
        borderColor: '#2563eb',
        backgroundColor: 'rgba(37, 99, 235, 0.1)',
        borderWidth: 3,
        fill: true,
        tension: 0.4,
        pointBackgroundColor: '#2563eb',
        pointBorderColor: '#ffffff',
        pointBorderWidth: 2,
        pointRadius: 4,
        pointHoverRadius: 6
      }
    ]
  }
})

// Opções do gráfico
const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'top',
      labels: {
        font: {
          size: 12,
          family: 'Inter, sans-serif'
        }
      }
    },
    tooltip: {
      mode: 'index',
      intersect: false,
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      titleColor: 'white',
      bodyColor: 'white',
      borderColor: '#2563eb',
      borderWidth: 1,
      callbacks: {
        label: function(context) {
          return `Saldo: ${fluxoCaixaUtils.formatCurrency(context.parsed.y)}`
        }
      }
    }
  },
  scales: {
    x: {
      display: true,
      title: {
        display: true,
        text: 'Data',
        font: {
          size: 12,
          weight: 'bold'
        }
      },
      grid: {
        display: false
      }
    },
    y: {
      display: true,
      title: {
        display: true,
        text: 'Saldo (R$)',
        font: {
          size: 12,
          weight: 'bold'
        }
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.1)'
      },
      ticks: {
        callback: function(value) {
          return fluxoCaixaUtils.formatCurrency(value)
        }
      }
    }
  },
  interaction: {
    mode: 'nearest',
    axis: 'x',
    intersect: false
  },
  elements: {
    point: {
      hoverBackgroundColor: '#2563eb'
    }
  }
}))

// Métodos
const carregarDados = async () => {
  loading.value = true
  error.value = null

  try {
    try {
      const dadosApi = await fluxoCaixaService.getSaldoDiario()
      dados.value = dadosApi.saldoDiario.map(item => ({
        data: item.data,
        receitas: item.totalReceitas,
        despesas: item.totalDespesas,
        saldoDia: item.saldoDia,
        saldoAcumulado: item.saldoAcumulado
      }))
    } catch (apiError) {
      console.warn('API não disponível', apiError)
    }

    // Emitir dados para o componente pai
    const totais = fluxoCaixaUtils.calcularTotais(dados.value)
    emit('dadosCarregados', totais)

  } catch (err) {
    error.value = 'Erro ao carregar dados do gráfico'
    console.error('Erro ao carregar dados:', err)
  } finally {
    loading.value = false
  }
}

const alterarPeriodo = (novoPeriodo) => {
  periodo.value = novoPeriodo
  carregarDados()
}

// Watchers
watch(periodo, () => {
  carregarDados()
})

// Lifecycle
onMounted(() => {
  carregarDados()
})

// Expor métodos para o componente pai
defineExpose({
  recarregarDados: carregarDados
})
</script>

<style scoped>
.chart-container {
  width: 100%;
}

.chart-container canvas {
  border-radius: 8px;
}
</style>
