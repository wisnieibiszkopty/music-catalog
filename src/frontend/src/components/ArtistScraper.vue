<script setup lang="ts">
import keycloak from '@/services/core/keycloak.ts'
import { ref } from 'vue'
import { ScraperService } from '@/services/api/scraper-service.ts'
import { defineShortcuts } from '@nuxt/ui/composables'

const props = defineProps<{
  collapsed: boolean
}>()

const open = ref(false)
const artistName = ref('');

defineShortcuts({
  meta_s: () => {
    open.value = !open.value;
  }
})

function search() {
  ScraperService.searchForArtist(artistName.value);
  artistName.value = '';
  open.value = false;
}
</script>

<template>
  <UDrawer
    v-model:open="open"
    title="Search for artist"
    description="Type in the artist's name to initiate an automated search from available sources."
    :ui="{ container: 'max-w-xl mx-auto' }"
  >
    <UButton
      v-if="keycloak.isAdmin()"
      :label="props.collapsed ? undefined : 'Search...'"
      icon="i-lucide-search"
      color="neutral"
      variant="outline"
      block
      :square="props.collapsed"
    >
      <template v-if="!props.collapsed" #trailing>
        <div class="flex items-center gap-0.5 ms-auto">
          <UKbd value="meta" variant="subtle" />
          <UKbd value="S" variant="subtle" />
        </div>
      </template>
    </UButton>

    <template #body>
      <div class="placeholder"></div>
    </template>

    <template #footer>
      <UInput v-model="artistName" placeholder="Artist name" />
      <UButton @click="search" label="Search" class="justify-center" icon="i-lucide-search" />
    </template>
  </UDrawer>
</template>

<style scoped>
.placeholder{
  padding-top: 72px;
}
</style>
