<script setup lang="ts">
import type { Album } from '@/services/api/catalog-service.ts'
import keycloak from '@/services/core/keycloak.ts'

const props = defineProps<{
  album: Album
  artistName: string
}>()

const emit = defineEmits<{
  (e: 'delete'): void
}>()

function handleDelete() {
  emit('delete')
}
</script>

<template>
  <div class="album-header">
    <div class="info-side">
      <p class="text-primary font-bold uppercase tracking-widest text-xs mb-2">Album</p>

      <h1 class="text-6xl font-black leading-none mb-6">
        {{ props.album.name }}
      </h1>

      <div class="flex items-center gap-2 font-medium mb-8">
        <span class="font-bold"> {{ props.artistName }} </span>
        <span>&bull;</span>
        <span>{{ props.album.releaseDate }}</span>
        <span>&bull;</span>
        <span>{{ props.album.totalTracks }} tracks</span>
      </div>

      <div v-if="keycloak.isAdmin()" class="flex gap-3">
        <UButton @click="handleDelete" color="error" icon="i-lucide-trash-2">Delete</UButton>
      </div>
    </div>

    <div class="image-side">
      <img :src="props.album.imageUrl ?? ''" :alt="props.album.name" class="album-cover" />
    </div>
  </div>
</template>

<style scoped>
.album-header {
  display: grid;
  grid-template-columns: 1fr auto;
  gap: 60px;
  align-items: end;
}

.album-cover {
  width: 300px;
  height: 300px;
  object-fit: cover;
  border-radius: var(--ui-radius, 0.75rem);
}
</style>
