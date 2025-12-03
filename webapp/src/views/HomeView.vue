<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  ChatGPT was asked to generate code to help integrate the Post service layer API calls.
  e.g. loading posts, mounting, etc.-->
<script setup lang="ts">
import { onMounted, onUnmounted, computed, ref } from "vue";
import Card from "primevue/card";
import PageHeader from "@/components/PageHeader.vue";
import PostList from "@/components/PostList.vue";
import SideBar from "@/components/SideBar.vue";
import MapOverview from "@/components/MapOverview.vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/authenticationStore";
import { usePostFilters } from "@/composables/usePostFilters";

const router = useRouter();
const auth = useAuthStore();
const postFilters = usePostFilters();
const scrollContainer = ref<HTMLElement | null>(null);
const isLoadingMore = ref(false);
let savedScrollPosition = 0;

function handleCreatePost(): void {
  if (isLoggedIn.value) {
    router.push("/create-post");
  } else {
    router.push("/login");
  }
}

function toggleSortOrder() {
  postFilters.filters.value.sortOrder =
    postFilters.filters.value.sortOrder === "Desc" ? "Asc" : "Desc";
  postFilters.fetchPosts();
}

async function loadMorePosts() {
  if (isLoadingMore.value || !postFilters.nextCursor.value || postFilters.loading.value) {
    return;
  }

  isLoadingMore.value = true;

  // Save the current scroll position and height before loading
  if (scrollContainer.value) {
    savedScrollPosition = scrollContainer.value.scrollTop;
  }

  await postFilters.fetchPosts(true);

  // Use requestAnimationFrame to restore scroll position smoothly
  requestAnimationFrame(() => {
    if (scrollContainer.value) {
      scrollContainer.value.scrollTop = savedScrollPosition;
    }
    requestAnimationFrame(() => {
      isLoadingMore.value = false;
    });
  });
}

function handleScroll() {
  if (!scrollContainer.value) return;

  const { scrollTop, scrollHeight, clientHeight } = scrollContainer.value;
  const scrolledToBottom = scrollHeight - scrollTop - clientHeight < 200;

  if (scrolledToBottom) {
    loadMorePosts();
  }
}

onMounted(() => {
  postFilters.fetchPosts();

  if (scrollContainer.value) {
    scrollContainer.value.addEventListener("scroll", handleScroll);
  }
});

onUnmounted(() => {
  if (scrollContainer.value) {
    scrollContainer.value.removeEventListener("scroll", handleScroll);
  }
});

const isLoggedIn = computed(() => auth.isAuthenticated);
</script>

<template>
  <div class="home-page">
    <div class="page-header">
      <PageHeader />
    </div>

    <div class="home-page-layout">
      <div class="side-bar">
        <SideBar view="home" />
      </div>

      <div class="home-page-body" ref="scrollContainer">
        <Card class="create-post-card">
          <template #title>
            <h1 class="create-post-title">A community for Winnipeg residents</h1>
          </template>
          <template #content>
            <div class="create-post-description">
              Discuss city improvements and life in Winnipeg
            </div>
          </template>
          <template #footer>
            <button class="create-post-button" @click="handleCreatePost">Create Post</button>
          </template>
        </Card>

        <div class="home-page-content-layout">
          <div class="post-list">
            <PostList
              :posts="postFilters.posts.value"
              :loading="postFilters.loading.value"
              :error="postFilters.error.value"
              :sort-order="postFilters.filters.value.sortOrder"
              :current-sort="postFilters.currentSort.value"
              @toggle-sort="toggleSortOrder"
            />

            <div
              v-if="isLoadingMore || (postFilters.nextCursor.value && postFilters.loading.value)"
              class="loading-more-container"
            >
              <i class="pi pi-spin pi-spinner"></i>
              <span>Loading more posts...</span>
            </div>
          </div>

          <div class="map-overview">
            <div class="map-overview-header">
              <i class="pi pi-map"></i>
              <h3>Map Overview</h3>
            </div>
            <MapOverview :posts="postFilters.posts.value" height="calc(100% - 60px)" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.home-page {
  padding: 1rem;
  height: 100vh;
  overflow: hidden;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}

.home-page-layout {
  display: flex;
  height: calc(100vh - 2rem);
  overflow: hidden;
}

.home-page-body {
  flex: 1;
  background: var(--primary-background-color);
  min-width: 0;
  overflow-y: auto;
  overflow-x: hidden;
}

.create-post-card {
  background: var(--neutral-color);
  margin: 1rem 1.5rem 1rem 1rem;
  border-radius: 1rem;
  padding: 1.75rem 10rem 2.5rem 4rem;
}

.create-post-title {
  font-size: 3rem;
  font-weight: 800;
  color: var(--secondary-text-color);
  margin-bottom: 0.5rem;
}

.create-post-description {
  font-size: 1.25rem;
  color: var(--secondary-text-color);
  margin-bottom: 1rem;
}

.create-post-button {
  justify-content: center;
  align-items: center;
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  font-size: 1rem;
  font-weight: 600;
  border: none;
  border-radius: 0.75rem;
  padding: 0.75rem 2rem;
  transition:
    background 0.2s,
    color 0.2s;
}

.create-post-button:hover {
  background: var(--primary-background-color-hover);
  cursor: pointer;
}

.home-page-content-layout {
  display: flex;
  gap: 1rem;
  margin: 1rem 1.5rem 1rem 1rem;
  min-width: 0;
  overflow: visible;
}

.post-list {
  border-radius: 1rem;
  flex: 1;
  max-width: 100%;
  min-width: 60%;
  overflow: visible;
}

.map-overview {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  width: 40rem;
  height: 50rem;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
  border-radius: 1rem;
  padding: 1.5rem;
  position: sticky;
  top: 1rem;
  overflow: hidden;
}

.map-overview-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid var(--border-color);
  flex-shrink: 0;
}

.map-overview-header i {
  font-size: 1.5rem;
  color: var(--primary-color, #3b82f6);
}

.map-overview-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--primary-text-color);
}

.loading-more-container {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.75rem;
  padding: 2rem 0;
  color: var(--secondary-text-color);
  font-size: 1rem;
}

.loading-more-container i {
  font-size: 1.25rem;
}
</style>
