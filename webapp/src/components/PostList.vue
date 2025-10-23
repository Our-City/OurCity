<script setup lang="ts">
import PostItem from "./PostItem.vue";
import type { PostResponseDto } from "@/types/posts";

defineProps <{
  posts: PostResponseDto[];
}>();
</script>

<template>
  <div class="post-list">
    <router-link
      v-for="post in posts"
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
}

.post-link {
  text-decoration: none;
}
</style>