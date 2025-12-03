/*
  src/composables/usePostFilters.ts

  Composable to manage post filters and sorting.
  AI assisted in the creation of this file.
    ChatGPT was asked to help with refactoring this to integrate the domain models and
    API services. This includes the filtering, sorting, and fetching logic.
*/
import { ref, watch, computed } from "vue";
import { getPosts } from "@/api/postService";
import { getTags } from "@/api/tagService";
import type { Post } from "@/models/post";
import type { Tag } from "@/models/tag";
import type { PostGetAllRequestDto } from "@/types/dtos/post";

export type SortType = "popular" | "recent" | "nearby";
export type FilterType = "all" | string; // "all" or any tag ID

// --- Reactive global state ---
const posts = ref<Post[]>([]);
const tags = ref<Tag[]>([]);
const currentSort = ref<SortType>("recent");
const currentFilter = ref<FilterType>("all");
const loading = ref(false);
const error = ref<string | null>(null);
const nextCursor = ref<string | null>(null);
const searchTerm = ref("");

// filter DTO
const filters = ref<PostGetAllRequestDto>({
  limit: 25,
  sortBy: "date",
  sortOrder: "Desc",
  tags: [],
  searchTerm: "",
});

// computed derived values
const sortOrder = computed(() => filters.value.sortOrder);

// fetch tags from API
async function fetchTags() {
  try {
    const res = await getTags();
    tags.value = res;
  } catch (err) {
    console.error("Failed to fetch tags:", err);
  }
}

// fetch posts based on current filters
async function fetchPosts(loadMore = false) {
  loading.value = true;
  error.value = null;

  try {
    // Always sync filters with searchTerm before calling API
    filters.value.searchTerm = searchTerm.value.trim() || undefined;

    // If loading more, use the nextCursor; otherwise, clear it for a fresh fetch
    if (loadMore && nextCursor.value) {
      filters.value.cursor = nextCursor.value;
    } else {
      filters.value.cursor = undefined;
    }

    const res = await getPosts(filters.value);
    // Filter out deleted posts from the results
    const filteredPosts = res.items.filter((post) => !post.isDeleted);

    // If loading more, append to existing posts; otherwise, replace
    if (loadMore) {
      posts.value = [...posts.value, ...filteredPosts];
    } else {
      posts.value = filteredPosts;
    }

    nextCursor.value = res.nextCursor ?? null;
  } catch (err) {
    console.error("Failed to fetch posts:", err);
    error.value = "Could not load posts.";
  } finally {
    loading.value = false;
  }
}

// sorting
function setSort(sort: SortType) {
  // if clicking same sort type again: toggle order
  if (currentSort.value === sort) {
    filters.value.sortOrder = filters.value.sortOrder === "Desc" ? "Asc" : "Desc";
  } else {
    // new sort type:  default to Desc
    currentSort.value = sort;
    filters.value.sortOrder = "Desc";
  }

  // assign sortBy based on sort type
  if (sort === "popular") {
    filters.value.sortBy = "votes";
  } else {
    filters.value.sortBy = "date";
  }

  fetchPosts();
}

// filtering by tag
function setFilter(filter: FilterType) {
  currentFilter.value = filter;

  if (filter === "all") {
    filters.value.tags = [];
  } else {
    filters.value.tags = [filter];
  }

  fetchPosts();
}

// reset filters
function reset() {
  currentSort.value = "recent";
  currentFilter.value = "all";
  searchTerm.value = "";
  filters.value = {
    limit: 25,
    sortBy: "date",
    sortOrder: "Desc",
    tags: [],
    searchTerm: "",
  };
  fetchPosts();
}

// watch searchTerm and automatically update filters + fetch
watch(searchTerm, () => {
  filters.value.searchTerm = searchTerm.value.trim() || undefined;
  fetchPosts();
});

export function usePostFilters() {
  return {
    posts,
    tags,
    currentSort,
    currentFilter,
    sortOrder,
    filters,
    searchTerm,
    loading,
    error,
    nextCursor,
    fetchPosts,
    fetchTags,
    setSort,
    setFilter,
    reset,
  };
}
