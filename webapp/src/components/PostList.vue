<!-- Generative AI was used to assist in the creation of this file.
  ChatGPT was asked to generate code to help integrate the Post service layer API calls.
  It also assisted with sorting/filtering logic-->
<script setup lang="ts">
import PostItem from "./PostItem.vue";
import { computed } from "vue";
import type { Post } from "@/models/post";

interface Props {
  posts: Post[];
  loading?: boolean;
  error?: string | null;
  sortOrder?: "Asc" | "Desc";
  currentSort?: "popular" | "recent" | "nearby";
  showSortControls?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  error: null,
  sortOrder: "Desc",
  currentSort: "recent",
  showSortControls: true,
});

const emit = defineEmits<{
  toggleSort: [];
}>();

// filter out deleted posts
const visiblePosts = computed(() => props.posts.filter((post) => !post.isDeleted));

// dynamic icon depending on order
const sortOrderIcon = computed(() =>
  props.sortOrder === "Desc" ? "pi pi-sort-amount-down" : "pi pi-sort-amount-up",
);

// dynamic label for sort button
const sortButtonLabel = computed(() => {
  switch (props.currentSort) {
    case "popular":
      return "Popular";
    case "recent":
      return "Recent";
    default:
      return "Sort by: Recent";
  }
});
</script>

<template>
  <div class="post-list-page">
    <!-- Sort toggle button -->
    <div v-if="showSortControls" class="sort-order-container">
      <button class="sort-order-button" @click="emit('toggleSort')">
        <i :class="sortOrderIcon" class="sort-icon" />
        {{ sortButtonLabel }}
      </button>
    </div>

    <!-- Post list -->
    <div class="post-list" data-testid="post-list">
      <div v-if="loading" class="loading-state">
        <i class="pi pi-spin pi-spinner"></i>
        <p>Loading posts...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <i class="pi pi-times-circle"></i>
        <p>{{ error }}</p>
      </div>

      <div v-else-if="visiblePosts.length === 0" class="empty-message">
        <i class="pi pi-inbox"></i>
        <p>No posts found</p>
        <p class="empty-subtitle">Try adjusting your filters or check back later</p>
      </div>

      <router-link
        v-else
        v-for="post in visiblePosts"
        :key="post.id"
        :to="`/posts/${post.id}`"
        class="post-link"
      >
        <PostItem :post="post" />
      </router-link>
    </div>
  </div>
</template>

<style scoped>
.post-list-page {
  padding-bottom: 3rem;
}

.sort-order-container {
  padding: 0.25rem 1rem;
  display: flex;
  justify-content: flex-end;
}

.sort-order-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  border: 1px solid var(--border-color);
  border-radius: 0.5rem;
  padding: 0.5rem 1rem;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s;
}

.sort-order-button:hover {
  background: var(--primary-background-color-hover);
}

.sort-icon {
  font-size: 1rem;
}

.post-list {
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  max-width: 100%;
  overflow: hidden;
}

.post-link {
  text-decoration: none;
  max-width: 100%;
  display: block;
}

.loading-state,
.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 2rem;
  color: var(--primary-text-color);
}

.loading-state i,
.error-state i {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.error-state {
  color: var(--primary-text-color);
}

.empty-message {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 2rem;
  color: var(--primary-text-color);
  opacity: 0.6;
}

.empty-message i {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.empty-message p {
  margin: 0.25rem 0;
  font-size: 1.25rem;
}

.empty-subtitle {
  font-size: 1rem;
  opacity: 0.8;
}
</style>
