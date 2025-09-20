<template>
  <div class="movimentacoes">

    <div class="header-section" style="margin-bottom: 20px;">
        <n-space>
          <n-button 
            type="primary" 
            size="medium"
            @click="novaMovimentacao"
            :loading="loading"
          >
            + Nova Movimentação
          </n-button>
        </n-space>
    </div>

    <n-data-table
      :columns="columns"
      :data="movimentacoes"
      :loading="loading"
      :pagination="false"
      :row-key="rowKey"
      striped
      size="medium"
    />

    <!-- Modal de Movimentação -->
    <n-modal
      v-model:show="showModalMovimentacao"
      preset="card"
      :title="isEdit ? 'Editar Movimentação' : 'Nova Movimentação'"
      style="width: 600px"
      :bordered="false"
      size="huge"
      role="dialog"
      aria-modal="true"
    >
      <MovimentacaoForm
        :movimentacao="movimentacaoSelecionada"
        :is-edit="isEdit"
        @save="onMovimentacaoSalva"
        @cancel="hideModal"
      />
    </n-modal>

    <!-- Modal de Confirmação de Exclusão -->
    <n-modal
      v-model:show="showConfirmDelete"
      preset="dialog"
      title="Confirmar Exclusão"
      content="Tem certeza que deseja excluir esta movimentação?"
      positive-text="Excluir"
      negative-text="Cancelar"
      @positive-click="excluirMovimentacao"
      @negative-click="hideConfirmDelete"
    >
      <div v-if="movimentacaoParaExcluir" class="mt-3">
        <n-alert type="warning" :show-icon="false">
          <strong>{{ movimentacaoParaExcluir.descricao }}</strong><br>
          {{ formatCurrency(movimentacaoParaExcluir.valor) }} - {{ formatDate(movimentacaoParaExcluir.data) }}
        </n-alert>
      </div>
    </n-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, h } from 'vue'
import { 
  NDataTable, NModal, NAlert, NButton, NIcon, NSpace, NTag, useMessage 
} from 'naive-ui'
import { 
  CreateOutline, TrashOutline
} from '@vicons/ionicons5'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import MovimentacaoForm from '../components/MovimentacaoForm.vue'
import { movimentacaoService, TipoMovimentacao, movimentacaoUtils } from '../services/movimentacaoService.js'

// Composables
const message = useMessage()

// Estado reativo
const loading = ref(false)
const movimentacoes = ref([])
const showModalMovimentacao = ref(false)
const showConfirmDelete = ref(false)
const isEdit = ref(false)
const movimentacaoSelecionada = ref(null)
const movimentacaoParaExcluir = ref(null)

// Configuração das colunas do DataTable
const columns = [
  {
    title: 'Descrição',
    key: 'descricao',
    sortOrder: false,
    sorter: 'default'
  },
  {
    title: 'Tipo',
    key: 'tipo',
    sortOrder: false,
    sorter: 'default',
    render(row) {
      return h(NTag, {
        type: movimentacaoUtils.getTipoColor(row.tipo),
        size: 'small'
      }, {
        default: () => movimentacaoUtils.getTipoText(row.tipo)
      })
    }
  },
  {
    title: 'Data',
    key: 'data',
    sortOrder: false,
    sorter: 'default',
    render(row) {
      return formatDate(row.data)
    }
  },
  {
    title: 'Valor',
    key: 'valor',
    sortOrder: false,
    sorter: 'default',
    align: 'right',
    render(row) {
      return movimentacaoUtils.formatCurrency(row.valor)
    }
  },
  {
    title: 'Ações',
    key: 'actions',
    align: 'center',
    width: 120,
    render(row) {
      return h(NSpace, { justify: 'center' }, {
        default: () => [
          h(NButton, {
            size: 'small',
            type: 'primary',
            circle: true,
            onClick: () => editarMovimentacao(row)
          }, {
            icon: () => h(NIcon, null, { default: () => h(CreateOutline) })
          }),
          h(NButton, {
            size: 'small',
            type: 'error',
            circle: true,
            onClick: () => confirmarExclusao(row)
          }, {
            icon: () => h(NIcon, null, { default: () => h(TrashOutline) })
          })
        ]
      })
    }
  }
]

// Métodos
const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

const formatDate = (dateString) => {
  return format(new Date(dateString), 'dd/MM/yyyy', { locale: ptBR })
}

const rowKey = (row) => row.id

const carregarMovimentacoes = async () => {
  loading.value = true
  try {
    const data = await movimentacaoService.getAll()
    movimentacoes.value = data
    message.success(`${data.length} movimentações carregadas com sucesso!`)
  } catch (error) {
    message.error('Erro ao carregar movimentações: ' + (error.response?.data?.message || error.message))
    
    movimentacoes.value = []
  } finally {
    loading.value = false
  }
}

const editarMovimentacao = (movimentacao) => {
  isEdit.value = true
  movimentacaoSelecionada.value = { ...movimentacao }
  showModalMovimentacao.value = true
}

const novaMovimentacao = () => {
  console.log('Botão Nova Movimentação clicado!')
  isEdit.value = false
  movimentacaoSelecionada.value = null
  showModalMovimentacao.value = true
  console.log('Modal deve abrir:', showModalMovimentacao.value)
}

const hideModal = () => {
  showModalMovimentacao.value = false
  movimentacaoSelecionada.value = null
}

const confirmarExclusao = (movimentacao) => {
  movimentacaoParaExcluir.value = movimentacao
  showConfirmDelete.value = true
}

const hideConfirmDelete = () => {
  showConfirmDelete.value = false
  movimentacaoParaExcluir.value = null
}

const excluirMovimentacao = async () => {
  if (!movimentacaoParaExcluir.value) return

  try {
    await movimentacaoService.delete(movimentacaoParaExcluir.value.id)
    
    // Remover da lista local
    movimentacoes.value = movimentacoes.value.filter(
      m => m.id !== movimentacaoParaExcluir.value.id
    )
    
    message.success('Movimentação excluída com sucesso!')
    hideConfirmDelete()
  } catch (error) {
    message.error('Erro ao excluir movimentação: ' + (error.response?.data?.message || error.message))
    console.error('Erro ao excluir movimentação:', error)
  }
}

const onMovimentacaoSalva = async (movimentacao) => {
  try {
    if (isEdit.value) {
      // Atualizar movimentação existente
      const movimentacaoAtualizada = await movimentacaoService.update(movimentacao.id, movimentacao)
      
      const index = movimentacoes.value.findIndex(m => m.id === movimentacao.id)
      if (index !== -1) {
        movimentacoes.value[index] = movimentacaoAtualizada
      }
      
      message.success('Movimentação atualizada com sucesso!')
    } else {
      // Criar nova movimentação
      const novaMovimentacao = await movimentacaoService.create(movimentacao)
      movimentacoes.value.unshift(novaMovimentacao)
      message.success('Movimentação criada com sucesso!')
    }
    
    hideModal()
  } catch (error) {
    message.error('Erro ao salvar movimentação: ' + (error.response?.data?.message || error.message))
    console.error('Erro ao salvar movimentação:', error)
  }
}

// Lifecycle
onMounted(() => {
  carregarMovimentacoes()
})
</script>

<style scoped>
.movimentacoes {
  padding: 20px;
}

.mt-3 {
  margin-top: 1rem;
}
</style>