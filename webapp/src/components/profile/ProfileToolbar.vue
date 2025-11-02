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
      <button class="saved-button" @click="handleSaved">Saved</button>
      <button class="drafts-button" @click="handleDrafts">Drafts</button>
      <button class="under-consideration-button" @click="handleUnderConsideration">
        Under Consideration
      </button>
      <button class="accepted-button" @click="handleAccepted">Accepted</button>
    </template>
  </Toolbar>
</template>

<style scoped></style>
