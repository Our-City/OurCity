<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  ChatGPT was asked to generate code to help integrate the Pinia authentication store.
  e.g., for fetching current user information via /me, etc. -->
<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import PageHeader from "@/components/PageHeader.vue";
import PostList from "@/components/PostList.vue";
import ProfileHeader from "@/components/profile/ProfileHeader.vue";
import ProfileToolbar from "@/components/profile/ProfileToolbar.vue";
import SideBar from "@/components/SideBar.vue";

import { getPostById } from "@/api/postService";
import { resolveErrorMessage } from "@/utils/error";

import type { Post } from "@/models/post";
import { useAuthStore } from "@/stores/authenticationStore";

const authStore = useAuthStore();
const user = computed(() => authStore.user);

const posts = ref<Post[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);

async function fetchProfileData() {
  loading.value = true;
  error.value = null;

  try {
    await authStore.fetchCurrentUser();

    if (authStore.user?.posts?.length) {
      const fetchedPosts = await Promise.all(
        authStore.user.posts.map((id) => getPostById(id))
      );
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

onMounted(fetchProfileData);
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
          <ProfileToolbar />

          <div class="profile-page-content">
            <PostList
              v-if="posts.length"
              :posts="posts"
              :loading="false"
              :show-sort-controls="false"
            />
            <div v-else class="no-posts-message">
              <i class="pi pi-inbox"></i>
              <p>You haven't created any posts yet.</p>
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
}

.profile-page-layout {
  display: flex;
  height: 100vh;
}

.profile-page-body {
  flex: 1;
  background: var(--primary-background-color);
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
