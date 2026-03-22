import Keycloak from "keycloak-js";

class CustomKeycloak extends Keycloak {
  isAdmin(): boolean {
    return this.hasRealmRole('admin');
  }
}

const keycloak = new CustomKeycloak({
  url: 'http://localhost:8080/auth',
  realm: 'music-catalog',
  clientId: 'api-client'
});

export default keycloak;
