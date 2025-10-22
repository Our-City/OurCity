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
    <template #content>
      <div class="post-content">
        <div class="post-votes">
          <VoteButton icon="pi pi-chevron-up" class="vote-btn upvote" @click.prevent="upvote" />
          <div class="vote-count">{{ post.votes ?? 0 }}</div>
          <VoteButton
            icon="pi pi-chevron-down"
            class="vote-btn downvote"
            @click.prevent="downvote"
          />
        </div>

        <div class="post-text-content">
          <h2 class="post-title">{{ post.title }}</h2>
          <div class="post-location">
            <i class="pi pi-map-marker location-icon"></i>
            <span>{{ post.location }}</span>
          </div>
          <p class="post-description">{{ post.description }}</p>
        </div>
        <div v-if="post.images && post.images.length" class="post-image-container">
          <img :src="post.images[0].url" :alt="post.title" class="post-image" />
        </div>
      </div>
    </template>
  </Card>
</template>

<style scoped>
.post-card {
  display: flex;
  border: 1px solid var(--primary-text-color);
  border-radius: 1rem;
}

.post-content {
  display: flex;
  gap: 1.5rem;
}

.post-votes {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 56px;
  flex-shrink: 0;
  gap: 8px;
}

.vote-count {
  font-weight: 700;
  font-size: 1rem;
  color: var(--text-color);
}

.vote-btn {
  width: 28px;
  height: 28px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  border-radius: 4px;
  transition:
    background-color 150ms ease,
    color 150ms ease,
    transform 120ms ease;
}

.vote-btn.p-button {
  background: transparent !important;
  border: none !important;
  box-shadow: none !important;
  color: var(--subtle-text-color) !important;
}

.vote-btn.p-button .pi {
  font-size: 0.9rem;
}

.vote-btn.p-button:hover {
  background: rgba(0, 0, 0, 0.04) !important;
  color: var(--text-color) !important;
  transform: translateY(-1px);
}

.vote-btn.upvote.p-button:hover {
  color: var(--secondary-color) !important;
}

.vote-btn.downvote.p-button:hover {
  color: var(--danger-color) !important;
}

.post-text-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.post-title {
  font-size: 1.5rem;
  color: var(--surface-color);
}

.post-location {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--subtle-text-color);
}

.location-icon {
  font-size: 1rem;
  color: var(--subtle-text-color);
}

.post-description {
  line-height: 1.5;
  color: var(--text-color);
}

.post-image-container {
  width: 200px;
  flex-shrink: 0;
}

.post-image {
  width: 100%;
  height: 150px;
  object-fit: cover;
  border-radius: 4px;
}

@media (max-width: 768px) {
  .post-content {
    flex-direction: column;
  }

  .post-image-container {
    width: 100%;
    order: -1;
    margin-bottom: 1rem;
  }
}
</style>