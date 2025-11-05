<!-- Generative AI was used to assist in the creation of this file.
  ChatGPT was asked to generate code to help integrate the Post service layer API calls.
  It also assisted with sorting/filtering logic-->
<script setup lang="ts">
import PostItem from "./PostItem.vue";
import { computed } from "vue";
import { usePostFilters } from "@/composables/usePostFilters";

// composable instance
const postFilters = usePostFilters();

// toggle sort order
function toggleSortOrder() {
  postFilters.filters.value.sortOrder =
    postFilters.filters.value.sortOrder === "Desc" ? "Asc" : "Desc";
  postFilters.fetchPosts();
}

// dynamic icon depending on order
const sortOrderIcon = computed(() =>
  postFilters.filters.value.sortOrder === "Desc"
    ? "pi pi-sort-amount-down"
    : "pi pi-sort-amount-up",
);

// dynamic label for sort button
const sortButtonLabel = computed(() => {
  switch (postFilters.currentSort.value) {
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
  <!-- Sort toggle button -->
  <div class="sort-order-container">
    <button class="sort-order-button" @click="toggleSortOrder">
      <i :class="sortOrderIcon" class="sort-icon" />
      {{ sortButtonLabel }}
    </button>
  </div>

  <!-- Post list -->
  <div class="post-list" data-testid="post-list">
    <div v-if="postFilters.loading.value" class="loading-state">
      <i class="pi pi-spin pi-spinner"></i>
      <p>Loading posts...</p>
    </div>

    <div v-else-if="postFilters.error.value" class="error-state">
      <i class="pi pi-times-circle"></i>
      <p>{{ postFilters.error.value }}</p>
    </div>

    <div v-else-if="postFilters.posts.value.length === 0" class="empty-message">
      <i class="pi pi-inbox"></i>
      <p>No posts found</p>
      <p class="empty-subtitle">Try adjusting your filters or check back later</p>
    </div>

    <router-link
      v-else
      v-for="post in postFilters.posts.value"
      :key="post.id"
      :to="`/posts/${post.id}`"
      class="post-link"
    >
      <PostItem :post="post" />
    </router-link>
  </div>
</template>

<style scoped>
.sort-dropdown-container :deep(.dropdown-menu) {
  right: auto;
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
