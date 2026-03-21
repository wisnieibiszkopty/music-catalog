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
    artistStore.save(data);

    return data;
  },

  delete: async (artistId: string) => {
    await api.delete(`/artists/${artistId}`);
  }
}
