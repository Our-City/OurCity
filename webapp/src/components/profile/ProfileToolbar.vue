<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  It also assisted with error handling.-->
<script setup lang="ts">
import Toolbar from "../utils/ToolbarCmp.vue";

function handlePosts() {}
</script>

<template>
  <Toolbar variant="standard">
    <template #start>
      <button class="posts-button" @click="handlePosts">Posts</button>
    </template>
  </Toolbar>
</template>

<style scoped></style>
