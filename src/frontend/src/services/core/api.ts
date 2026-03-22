import axios from 'axios';
import keycloak from './keycloak';
import { useToast } from '@nuxt/ui/composables'

export const baseUrl = 'http://localhost:8080/api';

const api = axios.create({
  baseURL: baseUrl
});

api.interceptors.request.use(async (config) => {
  if(keycloak.authenticated){
    try {
      await keycloak.updateToken(30);

      config.headers.Authorization = `Bearer ${keycloak.token}`;
    } catch(error) {
      keycloak.login();
    }
  }

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
