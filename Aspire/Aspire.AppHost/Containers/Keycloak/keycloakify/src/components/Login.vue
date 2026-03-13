<script setup>
import Keycloak from 'keycloak-js'
import { computed, onMounted, onUnmounted, ref } from 'vue'

const isLoading = ref(false)
const isAuthenticated = ref(false)
const errorMessage = ref('')
const token = ref('')
const parsedToken = ref(null)
const showToken = ref(false)

let keycloak = null
let refreshTimer = null

const keycloakConfig = {
  url: 'http://localhost:8080',
  realm: 'Hoanggggf',
  clientId: 'vue-app'
}

const displayName = computed(() => {
  if (!parsedToken.value) return 'Unknown user'

  return (
    parsedToken.value.name ??
    parsedToken.value.preferred_username ??
    parsedToken.value.email ??
    'Unknown user'
  )
})

const userRoles = computed(() => {
  const roles = parsedToken.value?.realm_access?.roles
  return Array.isArray(roles) ? roles : []
})

const expiresAt = computed(() => {
  const exp = parsedToken.value?.exp
  if (!exp) return 'Unknown'
  return new Date(exp * 1000).toLocaleString()
})

const issuedAt = computed(() => {
  const iat = parsedToken.value?.iat
  if (!iat) return 'Unknown'
  return new Date(iat * 1000).toLocaleString()
})

const tokenDurationMinutes = computed(() => {
  const exp = parsedToken.value?.exp
  const iat = parsedToken.value?.iat
  if (!exp || !iat || exp <= iat) return 'Unknown'
  return `${Math.round((exp - iat) / 60)} min`
})

const syncAuthState = () => {
  isAuthenticated.value = Boolean(keycloak?.authenticated)
  token.value = keycloak?.token ?? ''
  parsedToken.value = keycloak?.tokenParsed ?? null

  if (!isAuthenticated.value) {
    showToken.value = false
  }
}

const startTokenRefresh = () => {
  if (refreshTimer || !keycloak) return

  // Keep token fresh while dashboard stays open.
  refreshTimer = window.setInterval(async () => {
    try {
      if (!keycloak?.authenticated) return
      await keycloak.updateToken(30)
      syncAuthState()
    } catch {
      errorMessage.value = 'Phien dang nhap da het han. Hay dang nhap lai.'
      syncAuthState()
    }
  }, 15000)
}

const stopTokenRefresh = () => {
  if (!refreshTimer) return
  window.clearInterval(refreshTimer)
  refreshTimer = null
}

const initKeycloak = async () => {
  isLoading.value = true
  errorMessage.value = ''

  try {
    keycloak = new Keycloak(keycloakConfig)

    await keycloak.init({
      onLoad: 'login-required',
      pkceMethod: 'S256',
      checkLoginIframe: false
    })

    syncAuthState()

    if (keycloak.authenticated) {
      startTokenRefresh()
    }
  } catch (error) {
    errorMessage.value = `Khong the ket noi Keycloak: ${String(error)}`
  } finally {
    isLoading.value = false
  }
}

const login = async () => {
  if (!keycloak) return

  errorMessage.value = ''

  try {
    await keycloak.login({ redirectUri: window.location.href })
  } catch (error) {
    errorMessage.value = `Dang nhap that bai: ${String(error)}`
  }
}

const manualRefreshToken = async () => {
  if (!keycloak) return

  errorMessage.value = ''

  try {
    await keycloak.updateToken(0)
    syncAuthState()
  } catch (error) {
    errorMessage.value = `Lam moi token that bai: ${String(error)}`
  }
}

const logout = async () => {
  if (!keycloak) return

  errorMessage.value = ''

  try {
    await keycloak.logout({ redirectUri: window.location.origin })
    stopTokenRefresh()
    syncAuthState()
  } catch (error) {
    errorMessage.value = `Dang xuat that bai: ${String(error)}`
  }
}

onMounted(async () => {
  await initKeycloak()
})

onUnmounted(() => {
  stopTokenRefresh()
})
</script>

<template>
  <main class="login-page">
    <section class="card" v-if="!isAuthenticated">
      <h1>Keycloak Playground</h1>
      <p class="desc">UI de test login/logout va token khi ket noi voi Keycloak.</p>

      <div class="config">
        <div><strong>URL:</strong> {{ keycloakConfig.url }}</div>
        <div><strong>Realm:</strong> {{ keycloakConfig.realm }}</div>
        <div><strong>Client:</strong> {{ keycloakConfig.clientId }}</div>
      </div>

      <p v-if="isLoading">Dang khoi tao ket noi Keycloak...</p>
      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

      <div class="actions">
        <button @click="login" :disabled="isLoading">Dang nhap</button>
      </div>
    </section>

    <section class="card dashboard" v-else>
      <div class="dashboard-header">
        <div>
          <h1>Dashboard</h1>
          <p class="desc">Xin chao {{ displayName }}, ban da dang nhap thanh cong.</p>
        </div>
        <button class="secondary" @click="logout">Dang xuat</button>
      </div>

      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

      <div class="stats-grid">
        <article class="tile">
          <h3>User</h3>
          <p class="tile-value">{{ displayName }}</p>
          <p class="tile-sub">{{ parsedToken?.preferred_username ?? 'No username' }}</p>
        </article>

        <article class="tile">
          <h3>Session</h3>
          <p class="tile-value">{{ tokenDurationMinutes }}</p>
          <p class="tile-sub">Issued: {{ issuedAt }}</p>
        </article>

        <article class="tile">
          <h3>Expire at</h3>
          <p class="tile-value">{{ expiresAt }}</p>
          <p class="tile-sub">Subject: {{ parsedToken?.sub ?? 'Unknown' }}</p>
        </article>
      </div>

      <section class="panel">
        <h2>Roles</h2>
        <div v-if="userRoles.length" class="role-list">
          <span v-for="role in userRoles" :key="role" class="role-item">{{ role }}</span>
        </div>
        <p v-else>Khong co role nao trong token.</p>
      </section>

      <section class="panel">
        <div class="panel-header">
          <h2>Token</h2>
          <div class="actions compact">
            <button class="secondary" @click="manualRefreshToken">Lam moi token</button>
            <button class="secondary" @click="showToken = !showToken">
              {{ showToken ? 'An token' : 'Xem token' }}
            </button>
          </div>
        </div>

        <textarea v-if="showToken" readonly :value="token"></textarea>
        <p v-else>Token dang duoc an. Bam "Xem token" neu ban can debug.</p>
      </section>
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
  width: min(980px, 100%);
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
  align-items: center;
  gap: 8px;
  margin: 16px 0;
}

.actions.compact {
  margin: 0;
}

button {
  border: 1px solid #333;
  background: #111;
  color: #fff;
  border-radius: 8px;
  padding: 10px 14px;
  cursor: pointer;
}

button.secondary {
  background: transparent;
  color: inherit;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error {
  color: #b00020;
}

.dashboard {
  display: grid;
  gap: 16px;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 12px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(210px, 1fr));
  gap: 12px;
}

.tile,
.panel {
  border: 1px solid #ececf2;
  border-radius: 8px;
  padding: 12px;
}

.tile h3,
.panel h2 {
  margin-top: 0;
  margin-bottom: 8px;
}

.tile-value {
  margin: 0;
  font-weight: 700;
}

.tile-sub {
  margin-bottom: 0;
  color: #646780;
  font-size: 13px;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  align-items: center;
}

.role-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.role-item {
  border: 1px solid #d5d7e6;
  border-radius: 999px;
  padding: 4px 10px;
  font-size: 13px;
}

textarea {
  width: 100%;
  min-height: 170px;
  resize: vertical;
  font-family: ui-monospace, SFMono-Regular, Menlo, monospace;
  font-size: 12px;
}

@media (prefers-color-scheme: dark) {
  .card {
    background: #1c1d24;
    color: #f2f3f8;
    border-color: #383a48;
  }

  .tile,
  .panel {
    border-color: #383a48;
  }

  .tile-sub {
    color: #a4a9c5;
  }

  .role-item {
    border-color: #5b5f78;
  }

  button {
    background: #f2f3f8;
    color: #111;
    border-color: #f2f3f8;
  }

  button.secondary {
    background: transparent;
    color: #f2f3f8;
    border-color: #f2f3f8;
  }

  textarea {
    background: #11131a;
    color: #f2f3f8;
    border-color: #383a48;
  }
}
</style>

