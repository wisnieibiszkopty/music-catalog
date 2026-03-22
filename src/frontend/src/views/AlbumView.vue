<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { CatalogService, type Track } from '@/services/api/catalog-service.ts'
import keycloak from '@/services/core/keycloak.ts'
import { useToast } from '@nuxt/ui/composables'

const route = useRoute()
const router = useRouter();
const toast = useToast()

const tracks = ref<Track[]>()
const albumId = ref('')

onMounted(async () => {
  albumId.value = route.params.id as string
  console.log(albumId)

  tracks.value = await CatalogService.getTrackByAlbumId(albumId.value)
})

async function deleteAlbum() {
  const deleted = await CatalogService.delete(albumId.value)

  if (deleted) {
    toast.add({
      title: 'Deleted album',
      color: 'error',
    });

    // TODO push do artists with id, load album data to this component
    router.push('/artists')
  }
}
</script>

<template>
  <UButton v-if="keycloak.isAdmin()" color="error" @click="deleteAlbum()"> Delete </UButton>
  <p>tracks</p>
  <ul>
    <li v-for="track in tracks" :key="track.id">
      <p>{{ track.name }}</p>
    </li>
  </ul>
</template>

<style scoped></style>
