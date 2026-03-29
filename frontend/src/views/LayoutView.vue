<script setup lang="ts">

</script>

<template>
  <UDashboardGroup>
    <Sidebar/>

    <UDashboardPanel>
      <template #header>
        <UDashboardNavbar title="Dashboard" />
        <RouterView/>
      </template>
    </UDashboardPanel>
  </UDashboardGroup>
</template>

<style scoped>

</style>
