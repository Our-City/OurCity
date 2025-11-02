<script setup lang="ts">
import { ref } from "vue";
import PageHeader from "@/components/PageHeader.vue";
import PostList from "@/components/PostList.vue";
import { mockPosts } from "@/data/mockData.ts";
import ProfileHeader from "@/components/profile/ProfileHeader.vue";
import ProfileToolbar from "@/components/profile/ProfileToolbar.vue";
import SideBar from "@/components/SideBar.vue";

const posts = ref(mockPosts);
</script>

<template>
  <div>
    <div class="page-header">
      <PageHeader />
    </div>
    <div class="profile-page-layout">
      <SideBar view="profile" />
      <div class="profile-page-body">
        <ProfileHeader username="JohnDoe" />
        <ProfileToolbar />
        <div class="profile-page-content-layout">
          <div class="post-list">
            <PostList :posts="posts" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
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
