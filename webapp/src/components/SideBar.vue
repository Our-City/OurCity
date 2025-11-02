<script setup lang="ts">
import { useRouter } from "vue-router";
import { usePostFilters } from "@/composables/usePostFilters";

interface Props {
  view?: "home" | "profile";
}

const router = useRouter();
const { setSort, setFilter, reset } = usePostFilters();

const props = withDefaults(defineProps<Props>(), {
  view: "home",
});

function goToHome(): void {
  reset();
  router.push("/");
}

function handleSort(sortType: "popular" | "nearby" | "recent") {
  setSort(sortType);
}

function handleFilter(filterType: "recreational" | "infrastructure" | "all") {
  setFilter(filterType);
}
</script>

<template>
  <div :class="['side-bar', `toolbar--${props.view}`]">
    <button class="home-button" @click="goToHome"><i class="pi pi-home"></i> Home</button>
    <button
      class="popular-button"
      @click="
        reset();
        handleSort('popular');
      "
    >
      <i class="pi pi-chart-line"></i> Popular
    </button>
    <button
      class="nearby-button"
      @click="
        reset();
        handleSort('nearby');
      "
    >
      <i class="pi pi-map-marker"></i> Nearby
    </button>
    <button
      class="recreational-button"
      @click="
        reset();
        handleFilter('recreational');
      "
    >
      <i class="pi pi-sun"></i> Recreational
    </button>
    <button
      class="infrastructure-button"
      @click="
        reset();
        handleFilter('infrastructure');
      "
    >
      <i class="pi pi-building"></i> Infrastructure
    </button>

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
  justify-content: flex-start;
  align-items: flex-start;
  width: 20rem;
  padding: 1rem;
  height: 100%;
  background: var(--primary-background-color);
}

.side-bar button {
  width: 100%;
  text-align: left;
  display: flex;
  gap: 0.7rem;
}

.sidebar-divider {
  width: 100%;
  height: 1px;
  background-color: var(--neutral-color);
  margin: 1rem 0;
}

.github-link {
  width: 100%;
  text-align: left;
  display: flex;
  align-items: center;
  gap: 0.7rem;
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  border: none;
  border-radius: 0.75rem;
  padding: 0.5rem 1rem;
  font-size: 1.25rem;
  transition:
    background 0.2s,
    color 0.2s;
  text-decoration: none;
  cursor: pointer;
}

.github-link:hover {
  background: var(--primary-background-color-hover);
}

.github-link i {
  font-size: 1rem;
}
</style>
