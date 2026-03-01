<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'

const items: NavigationItems[] = [
  {
    label: 'Login',
    active: true,
    to: '/login'
  },
  {
    label: 'Register',
    active: true,
    to: '/register'
  },
  {
    label: 'Artists',
    active: true,
    to: '/artists'
  },
  {
    label: 'Albums',
    active: true,
    to: '/albums'
  }
];
</script>

<template>
  <UDashboardGroup>
    <UDashboardSidebar resizable>
      <UNavigationMenu
        :items="items"
        orientation="vertical"
      />
    </UDashboardSidebar>

    <UDashboardPanel>
      <template #header>
        <UDashboardNavbar title="Dashboard" />
        <RouterView/>
      </template>
    </UDashboardPanel>
  </UDashboardGroup>
</template>

<style scoped>

</style>
