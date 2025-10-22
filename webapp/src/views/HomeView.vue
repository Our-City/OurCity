<script setup lang="ts">
import { ref } from "vue";
import Card from "primevue/card";
import PageHeader from "@/components/PageHeader.vue";

import { mockPosts } from "@/data/mockData.ts";

const posts = ref(mockPosts);

</script>

<template>
  <div>
    <div class="page-header-cmp">
      <PageHeader />
    </div>
    <div class="home-page-layout">
      <div class="temp-sidebar">
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
      </div>
    </div>
  </div>
</template>

<style scoped>
.home-page-layout {
  display: flex; 
  height: 100vh;
}

.temp-sidebar {
  display: flex; 
  justify-content: center;
  align-items: center; 
  width: 20rem; 
  height: 100vw; 
  background: var(--test-color); 
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
}

.create-post-button:hover {
  background: var(--primary-background-color-hover);
  cursor: pointer;
}
</style>