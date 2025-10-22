<script setup lang="ts">
import PostItem from "./PostItem.vue";
import { mockPosts } from "@/data/mockData.ts";
</script>

<template>
  <div class="post-list">
    <router-link
      v-for="post in mockPosts"
      :key="post.id"
      :to="`/posts/${post.id}`"
      class="post-link"
    >
      <PostItem :post="post" />
    </router-link>
  </div>
</template>

<style scoped>
.post-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1rem;
}

.post-link {
  text-decoration: none;
}
</style>