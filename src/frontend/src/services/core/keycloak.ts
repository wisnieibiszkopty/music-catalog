import Keycloak from "keycloak-js";

const keycloak = new Keycloak({
  url: 'http://localhost:8080/auth',
  realm: 'music-catalog',
  clientId: 'api-client'
});

export default keycloak;
