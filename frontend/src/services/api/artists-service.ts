import api from '@/services/core/api.ts'
import { useArtistsStore } from '@/stores/artists-store.ts'

export interface Artist {
  id: string;
  name: string;
  imageUrl: string;
}

export const ArtistsService = {
  getAll: async (): Promise<Artist[]> => {
    const { data } = await api.get('/artists');

    const artistStore = useArtistsStore();
    artistStore.set(data);

    return data;
  },

  delete: async (artistId: string) => {
    const response = await api.delete(`/artists/${artistId}`);

    if (response.status >= 200 && response.status < 300){
      const artistsStore = useArtistsStore();
      artistsStore.deleteById(artistId);
    }
  }
}
