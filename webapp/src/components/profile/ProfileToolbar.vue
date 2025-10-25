<script setup lang="ts">
import Toolbar from '../utils/Toolbar.vue';

</script>

<template>  
    <Toolbar variant="standard">
    <template #start>
      <button class="posts-button" @click="">Posts</button>
      <button class="saved-button" @click="">Saved</button>
    </template>
  </Toolbar>

</template>

<style scoped> 
</style>