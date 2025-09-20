<template>
  <n-message-provider>
    <n-layout style="min-height: 100vh; width: 100vw;">
      <!-- Header -->
      <n-layout-header
          bordered
          style="height: 64px; display: flex; align-items: center; padding: 0 30px;"
      >

          <!-- Titulo -->
          <div style="display: flex; align-items: center; gap: 10px; font-size: 18px; font-weight: 600; margin-right: 5rem;">
              <n-icon size="24">
                  <wallet-outline />
              </n-icon>
              Controle Financeiro
          </div>

        <n-menu
          mode="horizontal"
          :options="menuOptions"
          v-model:value="activeKey"
          @update:value="handleMenuSelect"
          style="flex: 1; font-weight: 500;"
          responsive
        />
      </n-layout-header>

      <!-- Corpo principal -->
      <n-layout-content content-style="padding: 20px;">
          <div>
              <router-view />
          </div>
      </n-layout-content>
    </n-layout>
  </n-message-provider>
</template>

<script setup lang="ts">
import type { MenuOption } from 'naive-ui'
import type { Component } from 'vue'
import { ref, watch, h } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { NIcon, NMessageProvider } from 'naive-ui'
import { WalletOutline, Home, CashOutline } from '@vicons/ionicons5'

const router = useRouter()
const route = useRoute()

// estado do menu
const activeKey = ref(route.name)

function renderIcon(icon: Component) {
  return () => h(NIcon, null, { default: () => h(icon) })
}

const menuOptions = [
  {
    label: 'Home',
    key: 'Home',
    icon: renderIcon(Home)
  },
  {
    label: 'Movimentações',
    key: 'Movimentacoes',
    icon: renderIcon(CashOutline)
  }
]

// quando clicar em uma opção do menu
const handleMenuSelect = (key) => {
  router.push({ name: key })
}

// atualizar o menu quando mudar de rota (ex: pelo botão voltar do navegador)
watch(
  () => route.name,
  (newName) => {
    activeKey.value = newName
  }
)
</script>