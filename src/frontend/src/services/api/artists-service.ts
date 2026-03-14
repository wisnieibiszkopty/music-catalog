import api from '@/services/core/api.ts'

export interface Artist {
  id: string;
  name: string;
  imageUrl: string;
}

export const ArtistsService = {
  getAll: async (): Promise<Artist[]> => {
    const { data } = await api.get('/artists');
    return data
  }
}
