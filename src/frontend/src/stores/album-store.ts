import { defineStore } from 'pinia'
import type { Album } from '@/services/api/catalog-service.ts'

export const useAlbumStore = defineStore('albums', {
  state: () => ({
    albums: [] as Album[],
  }),

  actions: {
    set(newAlbums: Album[]) {
      this.albums.splice(0, this.albums.length, ...newAlbums);
    },

    getById(albumId: string){
      return this.albums.find(a => a.id === albumId);
    },

    deleteById(albumId: string){
      this.albums = this.albums.filter(a => a.id !== albumId);
    }
  },
})
