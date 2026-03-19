import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'
import LoginView from '../views/LoginView.vue'
import { initKeycloak, isAuthenticated } from '../auth/keycloak.js'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/login'
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true }
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/login'
    }
  ]
})

router.beforeEach(async (to) => {
  if (to.meta.requiresAuth) {
    try {
      await initKeycloak({ onLoad: 'check-sso' })

      if (!isAuthenticated()) {
        return {
          name: 'login',
          query: { redirect: to.fullPath }
        }
      }
    } catch {
      return {
        name: 'login',
        query: { redirect: to.fullPath }
      }
    }
  }

  if (to.name === 'login') {
    try {
      await initKeycloak({ onLoad: 'check-sso' })
      if (isAuthenticated()) {
        return { name: 'dashboard' }
      }
    } catch {
      // Keep login page accessible when Keycloak is unavailable.
    }
  }

  return true
})

export default router
