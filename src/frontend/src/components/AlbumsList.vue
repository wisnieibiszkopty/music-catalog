<script setup lang="ts">
import { type Album } from '@/services/api/catalog-service.ts'

const props = defineProps<{
  albums: Album[]
}>()
</script>

<template>
  <ul>
    <li v-for="album in props.albums" :key="album.id">
      <UPageCard
        :to="`/albums/${album.id}`"
        :title="album.name"
        :description="`release date: ${album.releaseDate} | total tracks: ${album.totalTracks}`"
        orientation="horizontal"
        variant="subtle"
        spotlight
        spotlight-color="primary"
      >
        <img
          v-if="album.imageUrl"
          :src="album.imageUrl"
          :alt="album.name"
          width="200"
          class="w-[300px] h-auto object-cover self-end lg:justify-self-end"
        />
      </UPageCard>
    </li>
  </ul>
</template>

<style scoped>
li {
  padding: 20px 100px;
}
</style>
