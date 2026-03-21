<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useArtistsStore } from '@/stores/artists-store.ts'
import { type Artist, ArtistsService } from '@/services/api/artists-service.ts'
import { type Album, CatalogService } from '@/services/api/catalog-service.ts'
import AlbumsList from '@/components/AlbumsList.vue'
import { useToast } from '@nuxt/ui/composables'

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

onMounted(async () => {
  const artistId = route.params.id as string
  // TODO get artist from api if not in store
  artist.value = artistsStore.getById(artistId)!

  albums.value = await CatalogService.getAlbumsByArtistId(artist.value.id)

  console.log(albums.value)
  console.log(artist)
})

// TODO check permissions
function deleteArtist() {
  ArtistsService.delete(artist.value.id)

  toast.add({
    title: `Delete artist: ${artist.value.name}`,
  })

  artistsStore.deleteById(artist.value.id)

  router.push('/artists')
}
</script>
<template>
  <div>
    <UButton color="error" @click="deleteArtist()">Delete</UButton>
    <UPageSection :title="artist.name" orientation="horizontal">
      <img :src="artist.imageUrl" :alt="artist.name" loading="lazy" />
    </UPageSection>
    <AlbumsList :albums="albums" />
  </div>
</template>

<style scoped>
div{
  height: 100%;
  overflow-y: auto;
}
</style>
