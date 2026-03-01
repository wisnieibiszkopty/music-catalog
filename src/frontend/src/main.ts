import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import ui from '@nuxt/ui/vue-plugin'
import keycloak from './services/keycloak'


const app = createApp(App)

app.use(createPinia())
app.use(ui)

keycloak.init({
  onLoad: 'check-sso',
  pkceMethod: 'S256'
}).then((auth) => {
  app.use(router)
  app.mount('#app')

  console.log(keycloak);

  console.log(auth ? 'Logged in' : 'Guest')
}).catch(err => {
  console.error("Cannot initialize Keycloak:", err)
  app.use(router)
  app.mount('#app')
})
