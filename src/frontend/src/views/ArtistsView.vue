<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { type Artist, ArtistsService } from '@/services/api/artists-service.ts'

const artists = ref<Artist[]>([])

const loadArtists = async () => {
  artists.value = await ArtistsService.getAll();
}

onMounted(loadArtists);
</script>

<template>
  <ul>
    <RouterLink
      v-for="artist in artists"
      :key="artist.id"
      :to="`/artists/${artist.id}`"
      custom
      v-slot="{ navigate }"
    >
      <li @click="navigate">
        <img :src="artist.imageUrl" :alt="artist.name" />
        <p>{{ artist.name }}</p>
      </li>
    </RouterLink>
  </ul>
</template>

<style scoped>
ul {
  list-style: none;
  margin: 0;
  padding: 20px;

  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 180px));
  justify-content: center;
  gap: 48px;
}

li {
  cursor: pointer;
  text-align: center;
}

li img {
  width: 100%;
  aspect-ratio: 1;
  object-fit: cover;
  border-radius: 50%;
  margin-bottom: 8px;
}
</style>
