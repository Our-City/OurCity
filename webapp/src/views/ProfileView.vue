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

import { getCurrentUser } from "@/api/userService";
import { getPostById } from "@/api/postService";
import { resolveErrorMessage } from "@/utils/error";

import type { Post } from "@/models/post";
import { useAuthStore } from "@/stores/authenticationStore";

const auth = useAuthStore();
const user = computed(() => auth.user);

const posts = ref<Post[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);

async function fetchProfileData() {
  loading.value = true;
  try {
    const currentUser = await getCurrentUser();
    if (auth.user) Object.assign(auth.user, currentUser);
    else auth.user = currentUser;

    if (currentUser.posts?.length) {
      const fetchedPosts = await Promise.all(currentUser.posts.map((id) => getPostById(id)));
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

          <div class="profile-page-content-layout">
            <div class="post-list">
              <PostList v-if="posts.length" :posts="posts" />
              <div v-else class="no-posts-message">
                <p>You haven’t created any posts yet.</p>
              </div>
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
  font-size: 1.5rem;
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

.profile-page-content-layout {
  display: flex;
  gap: 1rem;
  margin: 1rem 1.5rem 1rem 1rem;
}

.post-list {
  border-radius: 1rem;
  width: 100%;
}

.map-overview {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
  width: 20rem;
  height: 56.5vh;
  background: var(--test-color);
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
