<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useArtistsStore } from '@/stores/artists-store.ts'
import { type Artist, ArtistsService } from '@/services/api/artists-service.ts'
import { type Album, CatalogService } from '@/services/api/catalog-service.ts'
import AlbumsList from '@/components/Album/AlbumsList.vue'
import { useToast } from '@nuxt/ui/composables'
import keycloak from '@/services/core/keycloak.ts'
import { ScraperService } from '@/services/api/scraper-service.ts'
import type { ButtonProps } from '@nuxt/ui/components/Button.vue'
import ImageHeader from '@/components/ImageHeader.vue'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const artistsStore = useArtistsStore()

const artist = ref<Artist>({
  id: '',
  name: '',
  imageUrl: '',
})

const albums = ref<Album[]>([])

const buttons = ref<ButtonProps[]>([])

const loading = ref(true)

onMounted(async () => {
  try {
    const artistId = route.params.id as string

    const foundArtist = artistsStore.getById(artistId)
    if (!foundArtist) return

    artist.value = foundArtist

    const minDelay = new Promise((resolve) => setTimeout(resolve, 300))

    const [fetchedAlbums] = await Promise.all([
      CatalogService.getAlbumsByArtistId(artist.value.id),
      minDelay,
    ])

    albums.value = fetchedAlbums.sort(
      (a, b) => new Date(b.releaseDate).getTime() - new Date(a.releaseDate).getTime(),
    )

    setupButtons()
  } finally {
    loading.value = false
  }
})

function setupButtons() {
  buttons.value = keycloak.isAdmin()
    ? [
        {
          label: "Get Album's data",
          icon: 'i-lucide-rocket',
          loadingAuto: true,
          onClick: searchForAlbums,
        },
        {
          label: 'Delete',
          color: 'error',
          variant: 'solid',
          icon: 'i-lucide-trash-2',
          loadingAuto: true,
          onClick: deleteArtist,
        },
      ]
    : []
}

async function searchForAlbums() {
  await ScraperService.searchForAlbums(artist.value.id)

  toast.add({
    title: 'Search started',
    color: 'primary'
  })
}

function deleteArtist() {
  ArtistsService.delete(artist.value.id)

  toast.add({
    title: `Deleted artist: ${artist.value.name}`,
    color: 'error',
  })

  artistsStore.deleteById(artist.value.id)

  router.push('/artists')
}
</script>

<template>
  <div class="main">
    <ImageHeader :image-url="artist.imageUrl" />

    <div class="page-header">
      <UPageHeader :title="artist.name" headline="Artist" :links="buttons" />
    </div>

    <template v-if="loading">
      <div class="empty">
        <USkeleton class="h-6 w-[300px] mb-4" />
        <USkeleton class="h-6 w-[200px] mb-8" />

        <div class="grid gap-4">
          <USkeleton v-for="i in 5" :key="i" class="h-20 w-full rounded-lg" />
        </div>
      </div>
    </template>

    <template v-else-if="albums.length === 0">
      <div class="empty">
        <UEmpty
          icon="i-lucide-music"
          title="No releases available"
          description="There are no albums listed for this artist in current catalog."
        />
      </div>
    </template>

    <template v-else>
      <AlbumsList :albums="albums" />
    </template>
  </div>
</template>

<style scoped>
.main {
  height: 100%;
  overflow-y: auto;
}

.page-header {
  padding: 0 128px;
  margin-top: -60px;
  position: relative;
  z-index: 10;
}

.empty {
  padding: 50px 120px;
}
</style>
