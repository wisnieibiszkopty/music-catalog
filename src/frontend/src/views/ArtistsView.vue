<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { type Artist, ArtistsService } from '@/services/api/artists-service.ts'

const artists = ref<Artist[]>([])

const loadArtists = async () => {
  try {
    artists.value = await ArtistsService.getAll();
    console.log(artists);
  } catch (error) {
    // TODO display toast
    console.error(error)
  }
}

onMounted(loadArtists);
</script>

<template>
  <p>artists</p>
  <li v-for="artist in artists">
    {{ artist.name }}
  </li>
</template>

<style scoped></style>
