<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  ChatGPT was asked to generate code to help integrate the Pinia authentication store.
  e.g., for fetching current user information via /me, etc. -->
<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import PageHeader from "@/components/PageHeader.vue";
import PostList from "@/components/PostList.vue";
import ProfileHeader from "@/components/profile/ProfileHeader.vue";
import ProfileToolbar from "@/components/profile/ProfileToolbar.vue";
import SideBar from "@/components/SideBar.vue";

import { getPostById, getBookmarks } from "@/api/postService";
import { resolveErrorMessage } from "@/utils/error";

import type { Post } from "@/models/post";
import { useAuthStore } from "@/stores/authenticationStore";

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const user = computed(() => authStore.user);

const posts = ref<Post[]>([]);
const bookmarkedPosts = ref<Post[]>([]);
const activeTab = ref<"posts" | "bookmarks">("posts");
const loading = ref(false);
const error = ref<string | null>(null);

const displayedPosts = computed(() => {
  return activeTab.value === "posts" ? posts.value : bookmarkedPosts.value;
});

async function fetchProfileData() {
  loading.value = true;
  error.value = null;

  try {
    await authStore.fetchCurrentUser();

    if (authStore.user?.posts?.length) {
      const fetchedPosts = await Promise.all(authStore.user.posts.map((id) => getPostById(id)));
      posts.value = fetchedPosts.sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime());
    } else {
      posts.value = [];
    }
  } catch (err: unknown) {
    console.error("Failed to load profile:", err);
    error.value = resolveErrorMessage(err, "Could not load your profile information.");
  } finally {
    loading.value = false;
  }
}

async function fetchBookmarks() {
  loading.value = true;
  error.value = null;

  try {
    const result = await getBookmarks({});
    bookmarkedPosts.value = result.items;
  } catch (err: unknown) {
    console.error("Failed to load bookmarks:", err);
    error.value = resolveErrorMessage(err, "Could not load your bookmarked posts.");
  } finally {
    loading.value = false;
  }
}

function handleTabChange(tab: "posts" | "bookmarks") {
  activeTab.value = tab;

  // Update URL query parameter
  router.replace({ query: { tab } });

  if (tab === "bookmarks" && bookmarkedPosts.value.length === 0) {
    fetchBookmarks();
  }
}

// Initialize active tab from URL query on mount
onMounted(() => {
  const tabFromQuery = route.query.tab as "posts" | "bookmarks" | undefined;
  if (tabFromQuery === "bookmarks") {
    activeTab.value = "bookmarks";
    fetchBookmarks();
  } else {
    activeTab.value = "posts";
  }

  fetchProfileData();
});

// Watch for query changes (e.g., when navigating back)
watch(
  () => route.query.tab,
  (newTab) => {
    if (newTab === "bookmarks" || newTab === "posts") {
      activeTab.value = newTab;
      if (newTab === "bookmarks" && bookmarkedPosts.value.length === 0) {
        fetchBookmarks();
      }
    }
  },
);
</script>

<template>
  <div class="profile-page">
    <div class="page-header">
      <PageHeader />
    </div>

    <div class="profile-page-layout">
      <SideBar view="profile" />

      <div class="profile-page-body">
        <div v-if="loading" class="loading-state">Loading profile...</div>
        <div v-else-if="error" class="error-state">{{ error }}</div>

        <template v-else-if="user">
          <ProfileHeader :username="user?.username" />
          <ProfileToolbar :active-tab="activeTab" @tab-change="handleTabChange" />

          <div class="profile-page-content">
            <PostList
              v-if="displayedPosts.length"
              :posts="displayedPosts"
              :loading="false"
              :show-sort-controls="false"
            />
            <div v-else class="no-posts-message">
              <i class="pi pi-inbox"></i>
              <p v-if="activeTab === 'posts'">You haven't created any posts yet.</p>
              <p v-else>You haven't bookmarked any posts yet.</p>
            </div>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<style scoped>
.profile-page {
  padding: 1rem;
  height: 100vh;
  overflow: hidden;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}

.profile-page-layout {
  display: flex;
  height: 100vh;
}

.profile-page-body {
  flex: 1;
  background: var(--primary-background-color);
  overflow-y: auto;
  overflow-x: hidden;
}

.profile-page-content {
  margin: 1rem 1.5rem 1rem 1rem;
}

.no-posts-message {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 2rem;
  color: var(--primary-text-color);
  opacity: 0.6;
}

.no-posts-message i {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.no-posts-message p {
  margin: 0.25rem 0;
  font-size: 1.25rem;
}

.loading-state,
.error-state {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 3rem 2rem;
  color: var(--primary-text-color);
  font-size: 1.25rem;
}

.error-state {
  color: var(--error-color, #d32f2f);
}
</style>
