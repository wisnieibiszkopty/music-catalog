<script setup lang="ts">
import keycloak from '@/services/keycloak';

function login(){
  keycloak.login({
    redirectUri: window.location.origin
  })
}

</script>

<template>
<p>artists</p>

  <UButton @click="login" icon="i-lucide-rocket" size="md" color="primary" variant="solid">Login</UButton>

</template>

<style scoped>

</style>
