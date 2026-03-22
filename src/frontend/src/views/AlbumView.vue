<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { type Album, CatalogService, type Track } from '@/services/api/catalog-service.ts'
import keycloak from '@/services/core/keycloak.ts'
import { useToast } from '@nuxt/ui/composables'
import { useAlbumStore } from '@/stores/album-store.ts'
import type { ButtonProps } from '@nuxt/ui/components/Button.vue'
import TrackTable from '@/components/TrackTable.vue'
import AlbumHeader from '@/components/AlbumHeader.vue'
import { useArtistsStore } from '@/stores/artists-store.ts'

const route = useRoute()
const router = useRouter()
const toast = useToast()

const albumStore = useAlbumStore()
const artistStore = useArtistsStore()

const tracks = ref<Track[]>()
const album = ref<Album>({
  id: '',
  artistId: '',
  name: '',
  releaseDate: '',
  totalTracks: 0,
  imageUrl: '',
})

const artistName = ref('');

const buttons = ref<ButtonProps[]>([])

onMounted(async () => {
  const albumId = route.params.id as string
  album.value = albumStore.getById(albumId)!

  const artist = artistStore.getById(album.value.artistId)!
  artistName.value = artist.name;

  tracks.value = await CatalogService.getTracksByAlbumId(album.value.id)

  setupButtons()
})

function setupButtons() {
  buttons.value = keycloak.isAdmin()
    ? [
        {
          label: 'Delete',
          color: 'error',
          variant: 'solid',
          icon: 'i-lucide-trash-2',
          onClick: deleteAlbum,
        },
      ]
    : []
}

async function deleteAlbum() {
  const deleted = await CatalogService.delete(album.value.id)

  if (deleted) {
    toast.add({
      title: `Deleted album: ${album.value.name}`,
      color: 'error',
    })

    router.push(`/artists/${album.value.artistId}`)
  }
}
</script>

<template>
  <div class="main">
    <AlbumHeader :album="album" :artist-name="artistName"/>
    <div class="h-[1px] w-full my-10"></div>
    <TrackTable :tracks="tracks ?? []" />
  </div>
</template>

<style scoped>
.main {
  height: 100%;
  overflow-y: auto;
  padding: 40px 128px;
}

@media (max-width: 1400px) {
  .main {
    padding: 40px 64px;
  }
}
</style>
