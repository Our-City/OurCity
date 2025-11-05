<!-- Generative AI was used to assist in the creation of this file.
  ChatGPT was asked to generate code to help integrate the Post service layer API calls.-->
<script setup lang="ts">
import { computed } from "vue";
import PostItem from "./PostItem.vue";
import type { Post } from "@/models/post";
import { usePostFilters } from "@/composables/usePostFilters";
import Dropdown from "./utils/DropdownMenu.vue";

const props = defineProps<{
  posts: Post[];
}>();

// composable that manages sorting/filtering state
const { setSort, currentSort, currentFilter } = usePostFilters();

// display label for current sort
const sortLabel = computed(() => {
  switch (currentSort.value) {
    case "popular":
      return "Popular";
    case "nearby":
      return "Nearby";
    case "recent":
    default:
      return "Recent";
  }
});

// apply filtering and sorting logic
const filteredAndSortedPosts = computed(() => {
  let result = [...props.posts];

  // filter by tag (or category substitute)
  if (currentFilter.value !== "all") {
    result = result.filter((post) =>
      post.tags.some(
        (tag) => tag.name.toLowerCase() === currentFilter.value.toLowerCase()
      )
    );
  }

  // sort based on currentSort
  switch (currentSort.value) {
    case "popular":
      result.sort((a, b) => b.upvoteCount - a.upvoteCount);
      break;
    case "nearby":
      // TODO: Add geolocation-based sorting once we have user coordinates
      break;
    case "recent":
    default:
      result.sort(
        (a, b) => b.createdAt.getTime() - a.createdAt.getTime()
      );
      break;
  }

  return result;
});
</script>

<template>
  <div class="sort-dropdown-container">
    <Dropdown button-class="sort-posts-button">
      <template #button>
        <i class="pi pi-sort-amount-down" />
        {{ sortLabel }}
      </template>
      <template #dropdown="{ close }">
        <ul>
          <li @click="setSort('popular'); close()">Popular</li>
          <li @click="setSort('recent'); close()">Recent</li>
          <li @click="setSort('nearby'); close()">Nearby</li>
        </ul>
      </template>
    </Dropdown>
  </div>

  <div class="post-list">
    <div v-if="filteredAndSortedPosts.length === 0" class="empty-message">
      <i class="pi pi-inbox"></i>
      <p>No posts found</p>
      <p class="empty-subtitle">Try adjusting your filters or check back later</p>
    </div>

    <router-link
      v-else
      v-for="post in filteredAndSortedPosts"
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
