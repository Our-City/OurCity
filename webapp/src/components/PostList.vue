<script setup lang="ts">
import { computed } from "vue";
import PostItem from "./PostItem.vue";
import type { PostResponseDto } from "@/types/posts";
import { usePostFilters } from "@/composables/usePostFilters";
import Dropdown from "./utils/Dropdown.vue";

const props = defineProps<{
  posts: PostResponseDto[];
}>();

const {  setSort, currentSort, currentFilter } = usePostFilters();

const sortLabel = computed(() => {
  switch (currentSort.value) {
    case 'popular':
      return 'Popular';
    case 'nearby':
      return 'Nearby';
    case 'recent':
    default:
      return 'Recent';
  }
});

const filteredAndSortedPosts = computed(() => {
  let result = [...props.posts];

  if (currentFilter.value !== 'all') {
    result = result.filter(post => post.category?.toLowerCase() === currentFilter.value);
  }

  switch (currentSort.value) {
    case 'popular':
      result.sort((a, b) => (b.upvotes || 0) - (a.upvotes || 0));
      break;
    case 'nearby':
      // TODO: Implement location-based sorting
      // For now, just return as-is
      break;
    case 'recent':
    default:
      result.sort((a, b) => new Date(b.createdAt || 0).getTime() - new Date(a.createdAt || 0).getTime());
      break;
  }

  return result;
});
</script>

<template>
  <Dropdown 
    button-class="sort-posts-button"
  >
    <template #button>
      <i class="pi pi-sort-amount-down" />
      {{ sortLabel }}
    </template>
    <template #dropdown="{ close }">
      <ul>
        <li @click="setSort('popular'); close()">
          Popular
        </li>
        <li @click="setSort('recent'); close()">
          Recent
        </li>
        <li @click="setSort('nearby'); close()">
          Nearby
        </li>
      </ul>
    </template>

  </Dropdown>
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

.post-list {
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.post-link {
  text-decoration: none;
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