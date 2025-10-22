<script setup lang="ts">
import { ref } from "vue";
import PageHeader from "@/components/PageHeader.vue";

import { mockPosts } from "@/data/mockData.ts";

const posts = ref(mockPosts);

</script>

<template>
  <div>
    <div class="page-header-cmp">
      <PageHeader />
    </div>
    <div class="temp-sidebar">
      Temporary Sidebar Placeholder
    </div>
  </div>
</template>

<style scoped>
.temp-sidebar {
  display: flex; 
  justify-content: center;
  align-items: center; 
  width: 20rem; 
  height: 100vw; 
  background: var(--test-color); 
}
</style>