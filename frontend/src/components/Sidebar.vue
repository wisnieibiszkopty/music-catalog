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
    target: '_blank',
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
    <template #header>
      <div class="flex items-center gap-2 px-2">
        <UIcon name="i-lucide-music" class="w-6 h-6 text-primary" />

        <p class="text-xl font-bold tracking-tight text-gray-900 dark:text-white">
          Music <span class="text-primary">Catalog</span>
        </p>
      </div>
    </template>

    <template #default="{ collapsed }">
      <ArtistScraper :collapsed="collapsed" />

      <UNavigationMenu
        :collapsed="collapsed"
        :items="adminItems"
        orientation="vertical"
      />

      <div class="mt-auto flex flex-col gap-4 p-2">

        <UColorModeSelect v-if="!collapsed" class="w-full" />

        <UNavigationMenu
          :collapsed="collapsed"
          :items="notLoggedInBottomItems"
          orientation="vertical"
        />
      </div>
    </template>
  </UDashboardSidebar>
</template>

<style scoped></style>
