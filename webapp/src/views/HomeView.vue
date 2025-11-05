<!-- Generative AI was used to assist in the creation of this file.
  ChatGPT was asked to generate code to help integrate the Post service layer API calls.
  e.g. loading posts, mounting, etc.-->
<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import Card from "primevue/card";
import PageHeader from "@/components/PageHeader.vue";
import PostList from "@/components/PostList.vue";
import SideBar from "@/components/SideBar.vue";
import { useRouter } from "vue-router";
import { getPosts } from "@/api/postService";
import type { Post } from "@/models/post";
import { useAuthStore } from "@/stores/authenticationStore";

const router = useRouter();
const auth = useAuthStore();

const posts = ref<Post[]>([]);
const nextCursor = ref<string | null>(null);
const isLoading = ref(false);
const errorMessage = ref<string | null>(null);

async function loadPosts(initial = false) {
  if (isLoading.value) return;

  isLoading.value = true;
  errorMessage.value = null;

  try {
    const { items, nextCursor: cursor } = await getPosts(25, initial ? null : nextCursor.value);

    // replace or append
    posts.value = initial ? items : [...posts.value, ...items];
    nextCursor.value = cursor ?? null;
  } catch (err) {
    console.error("Failed to load posts:", err);
    errorMessage.value = "Failed to load posts. Please try again later.";
  } finally {
    isLoading.value = false;
  }
}

function handleCreatePost(): void {
  if (isLoggedIn.value) {
    router.push("/create-post");
  } else {
    router.push("/login");
  }
}

onMounted(() => {
  loadPosts(true);
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

      <div class="home-page-body">
        <!-- Create Post Card -->
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

        <!-- Main Content -->
        <div class="home-page-content-layout">
          <div class="post-list">
            <!-- Loading and error handling -->
            <div v-if="isLoading && posts.length === 0" class="loading-state">
              <i class="pi pi-spin pi-spinner"></i>
              <p>Loading posts...</p>
            </div>

            <div v-else-if="errorMessage" class="error-state">
              <i class="pi pi-times-circle"></i>
              <p>{{ errorMessage }}</p>
            </div>

            <template v-else>
              <PostList :posts="posts" />

              <!-- Load more -->
              <div v-if="nextCursor" class="load-more-container">
                <button class="load-more-button" @click="loadPosts()" :disabled="isLoading">
                  {{ isLoading ? "Loading..." : "Load More" }}
                </button>
              </div>
            </template>
          </div>

          <!-- Map placeholder -->
          <div class="map-overview">
            Map Overview Coming Soon
            <div class="spinner"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.home-page {
  padding: 1rem;
}

.home-page-layout {
  display: flex;
  height: 100vh;
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
  padding: 3rem 10rem 3rem 4rem;
}

.create-post-title {
  font-size: 4rem;
  color: var(--secondary-text-color);
  margin-bottom: 1rem;
}

.create-post-description {
  font-size: 1.5rem;
  color: var(--secondary-text-color);
  margin-bottom: 1.5rem;
}

.create-post-button {
  justify-content: center;
  align-items: center;
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  font-size: 1.1rem;
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
  min-width: 0;
  overflow: visible;
}

.map-overview {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
  width: 20rem;
  height: 65rem;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
}

.spinner {
  margin-top: 1rem;
  width: 4rem;
  height: 4rem;
  border: 0.25rem solid #ccc;
  border-top: 0.25rem solid #1976d2;
  border-radius: 100%;
  animation: spin 1.5s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
