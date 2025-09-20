<template>
  <n-form
    ref="formRef"
    :model="formData"
    :rules="rules"
    label-placement="top"
    require-mark-placement="right-hanging"
  >
    <n-grid cols="2" x-gap="12" y-gap="12">
      <n-grid-item>
        <n-form-item label="Tipo" path="tipo">
          <n-select
            v-model:value="formData.tipo"
            placeholder="Selecione o tipo"
            :options="tipoOptions"
          />
        </n-form-item>
      </n-grid-item>
      <n-grid-item>
        <n-form-item label="Data" path="data">
          <n-date-picker
            v-model:value="formData.data"
            type="date"
            value-format="dd/MM/yyyy"
            placeholder="Selecione a data"
            style="width: 100%"
          />
        </n-form-item>
      </n-grid-item>
    </n-grid>

    <n-form-item label="Descrição" path="descricao">
      <n-input
        v-model:value="formData.descricao"
        placeholder="Ex: Salário, Supermercado, Combustível..."
        maxlength="200"
        show-count
      />
    </n-form-item>

    <n-form-item label="Valor" path="valor">
      <n-input-number
        v-model:value="formData.valor"
        placeholder="0,00"
        :precision="2"
        :min="0.01"
        style="width: 100%"
      >
        <template #prefix>
          R$
        </template>
      </n-input-number>
    </n-form-item>

    <n-space justify="end" class="mt-4">
      <n-button @click="handleCancel">
        Cancelar
      </n-button>
      <n-button
        type="primary"
        :loading="loading"
        @click="handleSave"
      >
        {{ isEdit ? 'Atualizar' : 'Salvar' }}
      </n-button>
    </n-space>
  </n-form>
</template>

<script setup>
import { ref, reactive, watch, nextTick } from 'vue'
import { 
  NForm, NFormItem, NGrid, NGridItem, NSelect, NDatePicker, 
  NInput, NInputNumber, NSpace, NButton, useMessage 
} from 'naive-ui'

// Props
const props = defineProps({
  movimentacao: {
    type: Object,
    default: () => ({})
  },
  isEdit: {
    type: Boolean,
    default: false
  }
})

// Emits
const emit = defineEmits(['save', 'cancel'])

// Composables
const message = useMessage()

// Refs
const formRef = ref(null)
const loading = ref(false)

// Estado reativo
const formData = reactive({
  tipo: null,
  data: null,
  descricao: '',
  valor: null
})

// Opções para selects (usando valores da API)
const tipoOptions = [
  { label: 'Receita', value: 1 }, // TipoMovimentacao.RECEITA
  { label: 'Despesa', value: 2 }  // TipoMovimentacao.DESPESA
]

// Regras de validação
const rules = {
  tipo: {
    required: true,
    message: 'Tipo é obrigatório',
    trigger: ['blur', 'change'],
    validator: (rule, value) => {
      if (value === null || value === undefined) {
        return new Error('Tipo é obrigatório')
      }
      return true
    }
  },
  data: {
    required: true,
    message: 'Data é obrigatória',
    trigger: ['blur', 'change'],
    validator: (rule, value) => {
      if (!value) {
        return new Error('Data é obrigatória')
      }
      return true
    }
  },
  descricao: {
    required: true,
    message: 'Descrição é obrigatória',
    trigger: ['blur', 'input'],
    validator: (rule, value) => {
      if (!value || value.trim().length < 3) {
        return new Error('Descrição deve ter pelo menos 3 caracteres')
      }
      return true
    }
  },
  valor: {
    required: true,
    message: 'Valor é obrigatório',
    trigger: ['blur', 'change'],
    validator: (rule, value) => {
      if (!value || value <= 0) {
        return new Error('Valor deve ser maior que zero')
      }
      return true
    }
  }
}

// Watchers
watch(() => props.movimentacao, (newMovimentacao) => {
  if (newMovimentacao) {
    console.log('Dados recebidos para edição:', newMovimentacao)
    
    formData.tipo = newMovimentacao.tipo || null
    
    // Converter data corretamente para o DatePicker do Naive UI
    if (newMovimentacao.data) {
      const dataObj = new Date(newMovimentacao.data)
      formData.data = dataObj.getTime()
      console.log('Data convertida:', { original: newMovimentacao.data, convertida: formData.data, dataObj })
    } else {
      formData.data = null
    }
    
    formData.descricao = newMovimentacao.descricao || ''
    formData.valor = newMovimentacao.valor || null
    
    // Limpar validação após carregar os dados
    nextTick(() => {
      formRef.value?.restoreValidation()
    })
  }
}, { immediate: true, deep: true })

// Métodos
const handleSave = async () => {
  try {
    await formRef.value?.validate()
    
    loading.value = true
    
    const movimentacaoData = {
      ...props.movimentacao,
      tipo: formData.tipo,
      data: new Date(formData.data).toISOString().split('T')[0],
      descricao: formData.descricao.trim(),
      valor: formData.valor
    }
    
    // Simular delay de salvamento
    await new Promise(resolve => setTimeout(resolve, 500))
    
    emit('save', movimentacaoData)
  } catch (error) {
    message.error('Por favor, corrija os erros no formulário')
  } finally {
    loading.value = false
  }
}

const handleCancel = () => {
  emit('cancel')
}

// Resetar formulário quando necessário
const resetForm = () => {
  formData.tipo = null
  formData.data = null
  formData.descricao = ''
  formData.valor = null
  
  nextTick(() => {
    formRef.value?.restoreValidation()
  })
}

// Expor métodos para o componente pai
defineExpose({
  resetForm
})
</script>

<style scoped>
.mt-4 {
  margin-top: 1rem;
}
</style>
