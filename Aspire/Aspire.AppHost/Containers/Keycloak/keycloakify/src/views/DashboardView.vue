<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  getParsedToken,
  getToken,
  initKeycloak,
  isAuthenticated,
  logout,
  updateToken
} from '../auth/keycloak.js'

const router = useRouter()

const errorMessage = ref('')
const token = ref('')
const parsedToken = ref(null)
const showToken = ref(false)

let refreshTimer = null

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

const syncAuthState = () => {
  token.value = getToken()
  parsedToken.value = getParsedToken()
}

const startTokenRefresh = () => {
  if (refreshTimer) return

  // Keep token alive while user stays in dashboard.
  refreshTimer = window.setInterval(async () => {
    try {
      const refreshed = await updateToken(30)
      if (refreshed) {
        syncAuthState()
      }
    } catch {
      errorMessage.value = 'Phien dang nhap da het han. Hay dang nhap lai.'
      await router.replace('/login')
    }
  }, 115000)
}

const stopTokenRefresh = () => {
  if (!refreshTimer) return
  window.clearInterval(refreshTimer)
  refreshTimer = null
}

const loadSession = async () => {
  errorMessage.value = ''

  try {
    await initKeycloak({ onLoad: 'check-sso' })

    if (!isAuthenticated()) {
      await router.replace({ name: 'login', query: { redirect: '/dashboard' } })
      return
    }

    syncAuthState()
    startTokenRefresh()
  } catch (error) {
    errorMessage.value = `Khong the tai phien dang nhap: ${String(error)}`
    await router.replace('/login')
  }
}

const manualRefreshToken = async () => {
  errorMessage.value = ''

  try {
    await updateToken(0)
    syncAuthState()
  } catch (error) {
    errorMessage.value = `Lam moi token that bai: ${String(error)}`
  }
}

const handleLogout = async () => {
  errorMessage.value = ''
  stopTokenRefresh()

  try {
    await logout(`${window.location.origin}/login`)
  } catch (error) {
    errorMessage.value = `Dang xuat that bai: ${String(error)}`
  }
}

onMounted(async () => {
  await loadSession()
})

onUnmounted(() => {
  stopTokenRefresh()
})
</script>

<template>
  <main class="dashboard-page">
    <section class="card dashboard">
      <div class="dashboard-header">
        <div>
          <h1>Dashboard</h1>
          <p class="desc">Xin chao {{ displayName }}, ban da dang nhap thanh cong.</p>
        </div>
        <button class="secondary" @click="handleLogout">Dang xuat</button>
      </div>

      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

      <div class="stats-grid">
        <article class="tile">
          <h3>User</h3>
          <p class="tile-value">{{ displayName }}</p>
          <p class="tile-sub">{{ parsedToken?.preferred_username ?? 'No username' }}</p>
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
.dashboard-page {
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
    color: inherit;
  }
}
</style>

