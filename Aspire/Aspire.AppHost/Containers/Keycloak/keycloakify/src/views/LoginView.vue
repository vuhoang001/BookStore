<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getKeycloakConfig, initKeycloak, isAuthenticated, login } from '../auth/keycloak.js'

const route = useRoute()
const router = useRouter()

const isLoading = ref(false)
const errorMessage = ref('')

const keycloakConfig = getKeycloakConfig()

const redirectPath = computed(() => {
  const rawRedirect = route.query.redirect

  if (typeof rawRedirect === 'string' && rawRedirect.startsWith('/')) {
    return rawRedirect
  }

  return '/dashboard'
})

const toAbsoluteUrl = (path) => `${window.location.origin}${path}`

const ensureSession = async () => {
  isLoading.value = true
  errorMessage.value = ''

  try {
    await initKeycloak({ onLoad: 'check-sso' })

    if (isAuthenticated()) {
      await router.replace(redirectPath.value)
    }
  } catch (error) {
    errorMessage.value = `Khong the ket noi Keycloak: ${String(error)}`
  } finally {
    isLoading.value = false
  }
}

const handleLogin = async () => {
  isLoading.value = true
  errorMessage.value = ''

  try {
    await login(toAbsoluteUrl(redirectPath.value))
  } catch (error) {
    errorMessage.value = `Dang nhap that bai: ${String(error)}`
    isLoading.value = false
  }
}

onMounted(async () => {
  await ensureSession()
})
</script>

<template>
  <main class="login-page">
    <section class="card">
      <h1>Dang nhap voi Keycloak</h1>
      <p class="desc">Ban can dang nhap truoc khi vao trang dashboard.</p>

      <div class="config">
        <div><strong>URL:</strong> {{ keycloakConfig.url }}</div>
        <div><strong>Realm:</strong> {{ keycloakConfig.realm }}</div>
        <div><strong>Client:</strong> {{ keycloakConfig.clientId }}</div>
      </div>

      <p v-if="isLoading">Dang kiem tra phien dang nhap...</p>
      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

      <div class="actions">
        <button @click="handleLogin" :disabled="isLoading">Dang nhap</button>
      </div>
    </section>
  </main>
</template>

<style scoped>
.login-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 24px;
  box-sizing: border-box;
}

.card {
  width: min(480px, 100%);
  text-align: left;
  border: 1px solid #d9d9e0;
  border-radius: 12px;
  padding: 20px;
  background: #fff;
  color: #111;
}

.desc {
  margin-top: -8px;
  margin-bottom: 16px;
}

.config {
  display: grid;
  gap: 6px;
  margin-bottom: 16px;
  font-size: 14px;
}

.actions {
  display: flex;
  gap: 8px;
  margin: 16px 0;
}

button {
  border: 1px solid #333;
  background: #111;
  color: #fff;
  border-radius: 8px;
  padding: 10px 14px;
  cursor: pointer;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error {
  color: #b00020;
}

@media (prefers-color-scheme: dark) {
  .card {
    background: #1c1d24;
    color: #f2f3f8;
    border-color: #383a48;
  }

  button {
    background: #f2f3f8;
    color: #111;
    border-color: #f2f3f8;
  }
}
</style>

