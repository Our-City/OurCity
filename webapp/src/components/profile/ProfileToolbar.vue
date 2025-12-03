<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  It also assisted with error handling.-->
<script setup lang="ts">
import Toolbar from "../utils/ToolbarCmp.vue";

const props = defineProps<{
  activeTab: "posts" | "bookmarks";
}>();

const emit = defineEmits<{
  tabChange: [tab: "posts" | "bookmarks"];
}>();

function handlePosts() {
  emit("tabChange", "posts");
}

function handleBookmarks() {
  emit("tabChange", "bookmarks");
}
</script>

<template>
  <Toolbar variant="standard">
    <template #start>
      <button
        class="tab-button"
        :class="{ active: activeTab === 'posts' }"
        @click="handlePosts"
      >
        Posts
      </button>
      <button
        class="tab-button"
        :class="{ active: activeTab === 'bookmarks' }"
        @click="handleBookmarks"
      >
        Bookmarks
      </button>
    </template>
  </Toolbar>
</template>

<style scoped>
.tab-button {
  padding: 0.75rem 1.5rem;
  background: transparent;
  border: none;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  color: var(--tertiary-text-color);
  transition: all 0.2s;
  border-bottom: 2px solid transparent;
}

.tab-button:hover {
  color: var(--primary-text-color);
}

.tab-button.active {
  color: var(--primary-color, #3b82f6);
  border-bottom-color: var(--primary-color, #3b82f6);
}
</style>
