<script setup lang="ts">
const props = defineProps<{
  imageUrl: string
}>()
</script>

<template>
  <div class="artist-hero">
    <div class="artist-bg" :style="{ backgroundImage: `url(${props.imageUrl})` }"></div>
    <div class="overlay"></div>
  </div>
</template>

<style scoped>
.artist-hero {
  position: relative;
  width: 100%;
  height: 500px;
  overflow: hidden;
  mask-image: linear-gradient(to bottom, black 80%, transparent 100%);
}

.artist-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 110%;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  transition: transform 0.8s cubic-bezier(0.4, 0, 0.2, 1);
}

.overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    to bottom,
    transparent 0%,
    rgba(0, 0, 0, 0.3) 50%,
    rgba(15, 23, 42, 0.9) 100%
  );
  pointer-events: none;
}
</style>
