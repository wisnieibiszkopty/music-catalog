import { defineStore } from 'pinia'
import type { Artist } from '@/services/api/artists-service.ts'

export const useArtistsStore = defineStore('artists', {
  state: () => ({
    artists: [] as Artist[],
  }),
  actions: {
    save(newArtists: Artist[]) {
      this.artists = [...this.artists, ...newArtists];
    },

    getById(artistId: string) {
      return this.artists.find(a => a.id === artistId);
    },

    deleteById(artistId: string){
      this.artists = this.artists.filter(a => a.id !== artistId);
    }
  },
})
