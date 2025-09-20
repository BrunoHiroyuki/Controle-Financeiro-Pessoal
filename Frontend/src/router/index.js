import { createRouter, createWebHistory } from 'vue-router'
import Home from '../pages/Home.vue'
import Movimentacoes from '../pages/Movimentacoes.vue'

const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/movimentacoes', name: 'Movimentacoes', component: Movimentacoes }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
