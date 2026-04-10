import api from '@/services/core/api.ts'
import {useAlbumStore} from "@/stores/album-store.ts";

export interface Album {
  id: string;
  artistId: string;
  name: string;
  releaseDate: string;
  totalTracks: number;
  imageUrl: string | null
}

export interface Track {
  id: number;
  name: string;
  durationMs: number;
  trackNumber: number;
}

export const CatalogService = {
  getAlbumsByArtistId: async (artistId: string): Promise<Album[]> => {
    const { data } = await api.get(`/catalog/albums/${artistId}`);

    const albumStore = useAlbumStore();
    albumStore.set(data);

    return data;
  },

  getTracksByAlbumId: async (albumId: string): Promise<Track[]> => {
    const { data } = await api.get(`/catalog/albums/songs/${albumId}`)
    return data
  },

  delete: async (albumId: string): Promise<boolean> => {
    const response = await api.delete(`/catalog/${albumId}`);
    const deleted = response.status >= 200 && response.status < 300;

    if(deleted){
      const albumStore = useAlbumStore();
      albumStore.deleteById(albumId);
    }

    return deleted;
  }
}
