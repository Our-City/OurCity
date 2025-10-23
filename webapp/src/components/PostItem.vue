<script setup lang="ts">
import Card from "primevue/card";
import VoteButton from "primevue/button";
import { defineProps, defineEmits } from "vue";
import type { PostResponseDto } from "@/types/posts";

defineProps<{post: PostResponseDto}>();

const emit = defineEmits<{
  (e: "upvote"): void;
  (e: "downvote"): void;
}>();

function upvote() {
  emit("upvote");
}

function downvote() {
  emit("downvote");
}

</script>

<template>
  <Card class="post-card">
    <template #title>
      <div class="post-author-date">
        Author • Date
      </div>
      <h1 class="post-title">
        {{ post.title }}
      </h1>
    </template>
    <template #content>
      <div class="post-tags">
        Tags Placeholder
      </div>
    </template>
  </Card>
</template>

<style scoped>
.post-card {
  width: 100%;
  height: 8rem;
  border: 1px solid var(--primary-text-color);
  border-radius: 1rem;
  padding: 1rem;
}

.post-author-date {
  font-size: 0.75rem;
  color: var(--tertiary-text-color);
}

.post-title {
  font-size: 1.5rem;
  color: var(--primary-text-color);
}

.post-tags {
  font-size: 0.75rem;
  color: var(--tertiary-text-color);
}

</style>