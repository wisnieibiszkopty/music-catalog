import type { Artist } from '@/services/api/artists-service.ts'

export interface NotificationEvents {
  AlbumsSaved: [artistId: string],
  ArtistSaved: [aritst: Artist]
}
