<script setup lang="ts">
import Toolbar from "../utils/Toolbar.vue";
</script>

<template>
  <Toolbar variant="standard">
    <template #start>
      <button class="posts-button" @click="">Posts</button>
      <button class="saved-button" @click="">Saved</button>
      <button class="drafts-button" @click="">Drafts</button>
      <button class="under-consideration-button" @click="">Under Consideration</button>
      <button class="accepted-button" @click="">Accepted</button>
    </template>
  </Toolbar>
</template>

<style scoped></style>
