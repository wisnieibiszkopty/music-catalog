import axios from 'axios';
import keycloak from './keycloak';
import { useToast } from '@nuxt/ui/composables'

export const baseUrl = '';

const api = axios.create({
  baseURL: `${baseUrl}/api`
});

api.interceptors.request.use(async (config) => {
  return config;
}, (error) => {
  return Promise.reject(error);
});

api.interceptors.response.use(
  async (response) => response,
  async (error) => {
    const toast = useToast();
    toast.add({
      title: 'Error occured',
      description: error.message,
      color: 'error'
    });

    return Promise.reject(error);
  }
);

export default api;
