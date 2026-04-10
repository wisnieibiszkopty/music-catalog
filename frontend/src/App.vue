<script setup lang="ts">
import { RouterView } from 'vue-router'

const toaster = {
  position: 'top-right',
  duration: 10000
};

</script>

<template>
  <UApp :toaster="toaster">
    <RouterView />
  </UApp>
</template>

<style scoped>

</style>
