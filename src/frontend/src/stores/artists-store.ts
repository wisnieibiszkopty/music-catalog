import { defineStore } from 'pinia'
import type { Artist } from '@/services/api/artists-service.ts'

export const useArtistsStore = defineStore('artists', {
  state: () => ({
    artists: [] as Artist[],
  }),
  actions: {
    add(newArtist: Artist){
      this.artists.push(newArtist);
    },

    set(newArtists: Artist[]) {
      this.artists.splice(0, this.artists.length, ...newArtists);
    },

    getById(artistId: string) {
      return this.artists.find(a => a.id === artistId);
    },

    deleteById(artistId: string){
      this.artists = this.artists.filter(a => a.id !== artistId);
    }
  },
})
