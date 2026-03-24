import Keycloak from "keycloak-js";
import { baseUrl } from '@/services/core/api.ts'

class CustomKeycloak extends Keycloak {
  isAdmin(): boolean {
    return this.hasRealmRole('admin');
  }
}

const keycloak = new CustomKeycloak({
  url: `${baseUrl}/auth`,
  realm: 'music-catalog',
  clientId: 'api-client'
});

export default keycloak;
