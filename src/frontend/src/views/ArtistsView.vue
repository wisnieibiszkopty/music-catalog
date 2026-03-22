<script setup lang="ts">
import { onMounted } from 'vue'
import { ArtistsService } from '@/services/api/artists-service.ts'
import { useArtistsStore } from '@/stores/artists-store.ts'

const artistsStore = useArtistsStore()

onMounted(async () => {
  await loadArtists()
})

const loadArtists = async () => {
  await ArtistsService.getAll()
}
</script>

<template>
  <div>
    <ul>
      <RouterLink
        v-for="artist in artistsStore.artists"
        :key="artist.id"
        :to="`/artists/${artist.id}`"
        custom
        v-slot="{ navigate }"
      >
        <TransitionGroup name="list" tag="ul">
          <li @click="navigate" >
            <img :src="artist.imageUrl" :alt="artist.name" />
            <p>{{ artist.name }}</p>
          </li>
        </TransitionGroup>
      </RouterLink>
    </ul>
  </div>
</template>

<style scoped>
div {
  height: 100%;
  overflow-y: auto;
}

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

.list-move {
  transition: transform 0.3s ease;
}
</style>
