import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
import naive from 'naive-ui' // Importando Naive UI

const app = createApp(App)

app.use(router)
app.use(naive)

app.mount('#app')