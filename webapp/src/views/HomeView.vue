<script setup lang="ts">
import { ref } from "vue";
import Card from "primevue/card";
import PageHeader from "@/components/PageHeader.vue";
import PostList from "@/components/PostList.vue";
import { mockPosts } from "@/data/mockData.ts";

const posts = ref(mockPosts);

</script>

<template>
  <div>
    <div class="page-header">
      <PageHeader />
    </div>
    <div class="home-page-layout">
      <div class="side-bar">
        Temporary Sidebar Placeholder 
      </div>
      <div class="home-page-body">
        <Card class="create-post-card">
          <template #title>
            <h1 class="create-post-title">
              A community for Winnipeg residents
            </h1>
          </template>
          <template #content>
            <div class="create-post-description">
              Discuss city improvements and life in Winnipeg
            </div>
          </template>
          <template #footer>
            <button class="create-post-button">Create Post</button>
          </template>
        </Card>
        <div class="home-page-content-layout">
          <div class="post-list">

            <PostList :posts="mockPosts"/>
          </div>
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
.home-page-layout {
  display: flex; 
  height: 100vh;
}

.side-bar {
  display: flex; 
  justify-content: center;
  align-items: center; 
  width: 20rem; 
  height: 100%; 
  background: var(--primary-background-color); 
  border: 0.1rem solid var(--border-color);
}

.home-page-body {
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
  transition: background 0.2s, color 0.2s;
}

.create-post-button:hover {
  background: var(--primary-background-color-hover);
  cursor: pointer;
}

.home-page-content-layout {
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
  width: 30rem; 
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
  to { transform: rotate(360deg); }
}
</style>