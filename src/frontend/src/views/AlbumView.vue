<script setup lang="ts">
import { useRoute } from 'vue-router'
import { onMounted, ref } from 'vue'
import { CatalogService, type Track } from '@/services/api/catalog-service.ts'

const route = useRoute()

const tracks = ref<Track[]>()

onMounted(async () => {
  const albumId = route.params.id as string
  console.log(albumId)

  tracks.value = await CatalogService.getTrackByAlbumId(albumId);
})

// TODO check permissions
function deleteAlbum() {}
</script>

<template>
  <UButton color="error" @click="deleteAlbum()">Delete</UButton>
  <p>tracks</p>
  <ul>
    <li v-for="track in tracks" :key="track.id">
      <p>{{ track.name }}</p>
    </li>
  </ul>
</template>

<style scoped></style>
