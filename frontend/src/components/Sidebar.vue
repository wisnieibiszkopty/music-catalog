<script setup lang="ts">
import keycloak from '@/services/core/keycloak'
import type { NavigationMenuItem } from '@nuxt/ui'
import ArtistScraper from '@/components/ArtistScraper.vue'
import { baseUrl } from '@/services/core/api.ts'

const items: NavigationMenuItem[] = [
  {
    label: 'Artists',
    icon: 'i-lucide-inbox',
    to: '/artists',
  },
]

const adminItems: NavigationMenuItem[] = [
  ...items,
  {
    label: 'Grafana dashboard',
    icon: 'i-lucide-chart-line',
    href: `${baseUrl}/grafana/`,
    target: '_blank'
  },
]

const notLoggedInBottomItems: NavigationMenuItem = [
  {
    label: 'Login',
    icon: 'i-lucide-log-in',
    onSelect: () => {
      keycloak.login({
        redirectUri: window.location.origin,
      })
    },
  },
  {
    label: 'Register',
    icon: 'i-lucide-user-plus',
    onSelect: () => {
      keycloak.register({
        redirectUri: window.location.origin,
      })
    },
  },
]

const loggedInBottomItems: NavigationMenuItem = [
  {
    label: `Welcome ${keycloak.tokenParsed?.preferred_username}`,
    icon: 'i-lucide-user',
    active: false,
  },
  {
    label: 'Logout',
    icon: 'i-lucide-log-out',
    onSelect: () => {
      keycloak.logout({
        redirectUri: window.location.origin,
      })
    },
  },
]
</script>

<template>
  <UDashboardSidebar resizable :min-size="15" :default-size="25" :max-size="30" mode="drawer">
    <!--  TODO add real logo    -->
    <template #header="{ collapsed }">
      <Logo v-if="!collapsed" class="h-5 w-auto shrink-0" />
      <UIcon v-else name="i-simple-icons-nuxtdotjs" class="size-5 text-primary mx-auto" />
    </template>

    <template #default="{ collapsed }">
      <ArtistScraper :collapsed="collapsed" />

      <UNavigationMenu
        :collapsed="collapsed"
        :items="keycloak.authenticated ? adminItems : items"
        orientation="vertical"
      />

      <UNavigationMenu
        :collapsed="collapsed"
        :items="keycloak.authenticated ? loggedInBottomItems : notLoggedInBottomItems"
        orientation="vertical"
        class="mt-auto"
      />
    </template>
  </UDashboardSidebar>
</template>

<style scoped></style>
