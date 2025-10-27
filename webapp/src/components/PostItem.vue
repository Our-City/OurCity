<script setup lang="ts">
import Card from "primevue/card";
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
  <div class="post-card">
    <div class="post-card-left">
      <div class="post-tags">
        Tags Placeholder
      </div>
      <h1 class="post-title">
        Exploring the City Streets
      </h1>
      <div class="post-author-date">
        @a_real_prof • 6 days ago
      </div>
      <div class="post-votes-comments">
        <i class="pi pi-sort-alt"></i>
        <div class="post-number-stats">420</div>
        <i class="pi pi-comments"></i>
        <div class="post-number-stats">69</div> 
      </div>
    </div>
    <div class="post-card-right">
      <img 
        src="https://images.unsplash.com/photo-1591658522986-9eb791d2a89a?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1952" 
        alt="Post Image" 
        class="post-image"
      />
    </div>
  </div>
</template>

<style scoped>
.post-card {
  display: flex;
  width: 100%;
  height: 9.5rem;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
  border-radius: 1rem;
  padding: 1.25rem;
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.05);
}

.post-card-left {
  display: flex;
  flex-direction: column;
  width: 83%;
  justify-content: space-between;
}

.post-author-date {
  font-size: 1rem;
  color: var(--tertiary-text-color);
}

.post-title {
  font-size: 2rem;
  font-weight: 800;
  color: var(--primary-text-color);
}

.post-tags {
  font-size: 1rem;
  color: var(--tertiary-text-color);
}

.post-votes-comments {
  display: flex;
  flex-direction: row;
  align-items: center;
  color: var(--tertiary-text-color);
  margin-top: 1rem;
  gap: 0.5rem;
}

.post-votes-comments .pi {
  font-size: 1rem;
  vertical-align: middle;
}

.post-number-stats {
  font-size: 1rem;
  margin-right: 1rem;
}

.post-card-right {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 17%;
}

.post-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  aspect-ratio: 1 / 1;
  border: 0.1rem solid var(--border-color);
  border-radius: 1rem;
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.05);
}
</style>