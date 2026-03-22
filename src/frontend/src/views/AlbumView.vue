<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { type Album, CatalogService, type Track } from '@/services/api/catalog-service.ts'
import keycloak from '@/services/core/keycloak.ts'
import { useToast } from '@nuxt/ui/composables'
import {useAlbumStore} from "@/stores/album-store.ts";

const route = useRoute()
const router = useRouter()
const toast = useToast()

const albumStore = useAlbumStore();

const tracks = ref<Track[]>()
const album = ref<Album>({
  id: '',
  artistId: '',
  name: '',
  releaseDate: '',
  totalTracks: 0,
  imageUrl: ''
})

onMounted(async () => {
  const albumId = route.params.id as string
  album.value = albumStore.getById(albumId)!;

  tracks.value = await CatalogService.getTracksByAlbumId(album.value.id)
})

async function deleteAlbum() {
  const deleted = await CatalogService.delete(album.value.id)

  if (deleted) {
    toast.add({
      title: `Deleted album: ${album.value.name}`,
      color: 'error',
    })

    router.push(`/artists/${album.value.artistId}`);
  }
}
</script>

<template>
  <UButton v-if="keycloak.isAdmin()" color="error" @click="deleteAlbum()">
    Delete
  </UButton>
  <p>tracks</p>
  <ul>
    <li v-for="track in tracks" :key="track.id">
      <p>{{ track.name }}</p>
    </li>
  </ul>
</template>

<style scoped></style>
