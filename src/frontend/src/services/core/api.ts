import axios from 'axios';
import keycloak from './keycloak';

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

// TODO add interceptor for 401 and other errors

export default api;
