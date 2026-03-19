import Keycloak from 'keycloak-js'

const keycloakConfig = {
  url: 'http://localhost:8080',
  realm: 'Hoanggggf',
  clientId: 'vue-app'
}

let keycloak = null
let initialized = false
let initializingPromise = null

const getOrCreateKeycloak = () => {
  if (!keycloak) {
    keycloak = new Keycloak(keycloakConfig)
  }

  return keycloak
}

export const initKeycloak = async ({ onLoad = 'check-sso' } = {}) => {
  if (initialized) {
    return Boolean(keycloak?.authenticated)
  }

  if (initializingPromise) {
    return initializingPromise
  }

  const instance = getOrCreateKeycloak()

  initializingPromise = instance
    .init({
      onLoad,
      pkceMethod: 'S256',
      checkLoginIframe: false
    })
    .then((authenticated) => {
      initialized = true
      return authenticated
    })
    .finally(() => {
      initializingPromise = null
    })

  return initializingPromise
}

export const isAuthenticated = () => Boolean(keycloak?.authenticated)

export const getToken = () => keycloak?.token ?? ''

export const getParsedToken = () => keycloak?.tokenParsed ?? null

export const updateToken = async (minValiditySeconds = 30) => {
  if (!keycloak || !initialized) {
    return false
  }

  return keycloak.updateToken(minValiditySeconds)
}

export const login = async (redirectUri) => {
  await initKeycloak({ onLoad: 'check-sso' })
  return keycloak.login({ redirectUri })
}

export const logout = async (redirectUri) => {
  if (!keycloak || !initialized) {
    return
  }

  await keycloak.logout({ redirectUri })
}

export const getKeycloakConfig = () => keycloakConfig

