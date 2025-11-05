<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax
  Also assisted with filtering.-->
<script setup lang="ts">
import { onMounted } from "vue";
import { useRouter } from "vue-router";
import { usePostFilters } from "@/composables/usePostFilters";

interface Props {
  view?: "home" | "profile";
}

const props = withDefaults(defineProps<Props>(), { view: "home" });
const router = useRouter();
const { tags, currentFilter, setSort, setFilter, reset, fetchTags } = usePostFilters();

function goToHome() {
  reset();
  router.push("/");
}

onMounted(fetchTags);
</script>

<template>
  <div :class="['side-bar', `toolbar--${props.view}`]" data-testid="sidebar">
    <button class="home-button" @click="goToHome"><i class="pi pi-home"></i> Home</button>

    <button
      @click="
        () => {
          reset();
          setSort('popular');
        }
      "
    >
      <i class="pi pi-chart-line"></i> Popular
    </button>
    <button
      @click="
        () => {
          reset();
          setSort('recent');
        }
      "
    >
      <i class="pi pi-clock"></i> Recent
    </button>

    <div class="sidebar-divider"></div>

    <!-- Optional: add (WIP) if you want -->
    <h4 class="filter-title">Filter by tag (WIP)</h4>

    <button
      class="tag-button all-tags"
      :class="{ active: currentFilter === 'all' }"
      @click="setFilter('all')"
    >
      <i class="pi pi-list"></i> All
    </button>

    <!-- scrollable tag list -->
    <div class="tag-list disabled">
      <button
        v-for="tag in tags"
        :key="tag.id"
        :class="['tag-button', { active: currentFilter === tag.id }]"
        @click="setFilter(tag.id)"
      >
        <i class="pi pi-tag"></i>
        <span class="tag-name">{{ tag.name }}</span>
      </button>
    </div>

    <div class="sidebar-divider"></div>

    <a
      href="https://github.com/Our-City/OurCity"
      target="_blank"
      rel="noopener noreferrer"
      class="github-link"
    >
      <i class="pi pi-github"></i> Visit on GitHub
    </a>
  </div>
</template>

<style scoped>
.side-bar {
  display: flex;
  flex-direction: column;
  align-items: stretch;
  width: 18rem;
  padding: 1rem;
  height: 100vh;
  background: var(--primary-background-color);
  border-right: 1px solid var(--border-color);
  box-sizing: border-box;
}

/* buttons */
.side-bar button {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  width: 100%;
  text-align: left;
  background: none;
  border: none;
  color: var(--primary-text-color);
  font-size: 1rem;
  padding: 0.4rem 0.6rem;
  border-radius: 0.4rem;
  transition: background 0.15s ease;
}

.side-bar button:hover {
  background: rgba(25, 118, 210, 0.08);
  cursor: pointer;
}

.sidebar-divider {
  margin: 1rem 0;
  height: 1px;
  background-color: var(--border-color);
}

.filter-title {
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--primary-text-color);
  margin: 0.5rem 0;
}

/* scrollable tag area */
.tag-list {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  overflow-y: auto;
  flex-grow: 1;
  padding-right: 0.3rem;
}

/* ✅ disable interactions + scroll */
.tag-list.disabled {
  pointer-events: none;
  overflow: hidden;
  opacity: 0.6; /* optional visual cue, remove if undesired */
}

.tag-list::-webkit-scrollbar {
  width: 0.35rem;
}
.tag-list::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: 0.5rem;
}

/* Tag buttons */
.tag-button {
  display: flex;
  align-items: center;
  gap: 0.45rem;
  background: transparent;
  border: none;
  color: var(--primary-text-color);
  font-size: 0.9rem;
  text-align: left;
  padding: 0.3rem 0.5rem;
  border-radius: 0.4rem;
  transition: background 0.15s ease-in-out;
  width: 100%;
}

.tag-button:hover {
  background: rgba(25, 118, 210, 0.08);
}

.tag-button.active {
  background: var(--accent-color, #1976d2);
  color: #fff;
  font-weight: 500;
}

.tag-name {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
