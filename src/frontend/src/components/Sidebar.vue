<script setup lang="ts">
import keycloak from '@/services/core/keycloak';
import type { NavigationMenuItem } from '@nuxt/ui'

const items: NavigationMenuItem[] = [
  {
    label: 'Artists',
    icon: 'i-lucide-inbox',
    to: '/artists',
  },
  {
    label: 'Albums',
    icon: 'i-lucide-users',
    to: '/albums',
  },
  {
    label: 'Scrapper',
    icon: 'i-lucide-searchint',
    to: '/scrapper',
  },
];

const notLoggedInBottomItems: NavigationMenuItem = [
  {
    label: 'Login',
    icon: 'i-lucide-log-in',
    onSelect: () => {
      keycloak.login({
        redirectUri: window.location.origin
      });
    }
  },
  {
    label: 'Register',
    icon: 'i-lucide-user-plus',
    onSelect: () => {
      keycloak.register({
        redirectUri: window.location.origin
      });
    }
  },
];



const loggedInBottomItems: NavigationMenuItem = [
  {
    label: `Welcome ${keycloak.tokenParsed?.preferred_username}`,
    icon: 'i-lucide-user',
    active: false
  },
  {
    label: 'Logout',
    icon: 'i-lucide-log-out',
    onSelect: () => {
      keycloak.logout({
        redirectUri: window.location.origin
      })
    }
  }
];
</script>

<template>
  <UDashboardSidebar resizable :min-size="15" :default-size="25" :max-size="30" mode="drawer">
    <!--  TODO add real logo    -->
    <template #header="{ collapsed }">
      <Logo v-if="!collapsed" class="h-5 w-auto shrink-0" />
      <UIcon v-else name="i-simple-icons-nuxtdotjs" class="size-5 text-primary mx-auto" />
    </template>

    <template #default="{ collapsed }">
      <!--  TODO remove in the future    -->
      <UButton
        :label="collapsed ? undefined : 'Search...'"
        icon="i-lucide-search"
        color="neutral"
        variant="outline"
        block
        :square="collapsed"
      >
        <template v-if="!collapsed" #trailing>
          <div class="flex items-center gap-0.5 ms-auto">
            <UKbd value="meta" variant="subtle" />
            <UKbd value="K" variant="subtle" />
          </div>
        </template>
      </UButton>

      <UNavigationMenu
        :collapsed="collapsed"
        :items="items"
        orientation="vertical"
      />

      <UNavigationMenu
        :collapsed="collapsed"
        :items="keycloak.authenticated ? loggedInBottomItems : notLoggedInBottomItems"
        orientation="vertical"
        class="mt-auto"
      />
    </template>

    <!--  TODO remove in the future    -->
    <template #footer="{ collapsed }">
      <UButton
        :label="collapsed ? undefined : 'Footer'"
        color="neutral"
        variant="ghost"
        class="w-full"
        :block="collapsed"
      />
    </template>
  </UDashboardSidebar>
</template>

<style scoped>

</style>
