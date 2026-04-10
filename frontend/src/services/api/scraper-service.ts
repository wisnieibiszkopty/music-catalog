import api from '@/services/core/api.ts'

export const ScraperService = {
  searchForArtist: async (name: string) => {
    await api.post(`/scraper/artists/${name}`);
  },

  searchForAlbums: async (artistId: string) => {
    await api.post(`/scraper/albums/${artistId}`);
  }
}
