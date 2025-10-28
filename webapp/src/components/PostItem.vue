<script setup lang="ts">
import Card from "primevue/card";
import { defineProps, defineEmits, computed } from "vue";
import type { PostResponseDto } from "@/types/posts";
import { mockUsers } from "@/data/mockData";

const props = defineProps<{post: PostResponseDto}>();

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

// Get the username from the author ID
const authorUsername = computed(() => {
  const author = mockUsers.find(user => user.id === props.post.authorId);
  if (!author) {
    return `User #${props.post.authorId}`;
  }
  // Use displayName if available, otherwise username, otherwise ID
  if (author.displayName && author.displayName.trim()) {
    return author.displayName;
  }
  if (author.username) {
    return `@${author.username}`;
  }
  return `User #${props.post.authorId}`;
});

// Compute the number of comments
const commentCount = computed(() => {
  return props.post.commentIds?.length || 0;
});

// Get the first image if available
const postImage = computed(() => {
  return props.post.images?.[0]?.url || null;
});

</script>

<template>
  <div class="post-card">
    <div class="post-card-left">
      <div class="post-author-date">
        {{ authorUsername }}
      </div>
      <h1 class="post-title">
        {{ post.title }}
      </h1>
      <div class="post-tags">
        {{ post.location }}
      </div>
      <div class="post-votes-comments">
        <i class="pi pi-sort-alt"></i>
        <div class="post-number-stats">{{ post.votes }}</div>
        <i class="pi pi-comments"></i>
        <div class="post-number-stats">{{ commentCount }}</div> 
      </div>
    </div>
    <div v-if="postImage" class="post-card-right">
      <img 
        :src="postImage" 
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
  max-width: 100%;
  height: 9.5rem;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
  border-radius: 1rem;
  padding: 1.25rem;
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.05);
  box-sizing: border-box;
  overflow: hidden;
  gap: 1rem;
}

.post-card-left {
  display: flex;
  flex-direction: column;
  width: 83%;
  min-width: 0;
  justify-content: space-between;
  overflow: hidden;
}

.post-author-date {
  font-size: 1rem;
  color: var(--tertiary-text-color);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 100%;
  min-width: 0;
}

.post-title {
  font-size: clamp(1rem, 2vw, 2rem);
  font-weight: 800;
  color: var(--primary-text-color);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 100%;
  min-width: 0;
  margin: 0;
}

.post-tags {
  font-size: 1rem;
  color: var(--tertiary-text-color);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 100%;
  min-width: 0;
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