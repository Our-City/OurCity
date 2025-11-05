<script setup lang="ts">
import Toolbar from "../utils/ToolbarCmp.vue";

function handlePosts() {}

function handleSaved() {}

function handleDrafts() {}

function handleUnderConsideration() {}

function handleAccepted() {}
</script>

<template>
  <Toolbar variant="standard">
    <template #start>
      <button class="posts-button" @click="handlePosts">Posts</button>
    </template>
  </Toolbar>
</template>

<style scoped></style>
