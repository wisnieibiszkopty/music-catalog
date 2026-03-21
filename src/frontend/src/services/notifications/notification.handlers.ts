import { onEvent, startConnection } from '@/services/notifications/notifications.ts'
import { useToast } from '@nuxt/ui/composables'
import router from '@/router';
import type { Artist } from '@/services/api/artists-service.ts'

export async function initNotifications(){
  await startConnection();

  onEvent("ArtistSaved", onArtistSaved);
  onEvent("AlbumsSaved", onAlbumsSaved);
}

function onArtistSaved(artist: Artist){
  const toast = useToast();

  toast.add({
    title: 'New artist discovered',
    description: `${artist.name} was added to library`,
    onClick: () => {
      console.log('new artist discovered ' + artist.id)
      router.push(`/artists/${artist.id}`);
    },
  })
}

function onAlbumsSaved(artistId: string){
  const toast = useToast();

  toast.add({
    title: 'New albums discovered!',
    onClick: () => {
      console.log('new albums discovered ' + artistId)
      router.push('/albums')
    },
  })
}
