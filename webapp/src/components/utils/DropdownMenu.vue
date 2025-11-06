<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  It also assisted with error handling.-->
<script setup lang="ts">
import { ref } from "vue";

interface Props {
  buttonClass?: string;
  dropdownClass?: string;
}

const props = withDefaults(defineProps<Props>(), {
  buttonClass: "dropdown-button",
  dropdownClass: "dropdown-menu",
});

const isDropdownVisible = ref(false);

const toggleDropdown = () => {
  isDropdownVisible.value = !isDropdownVisible.value;
};

const closeDropdown = () => {
  isDropdownVisible.value = false;
};

// Close dropdown when clicking outside
const handleClickOutside = (event: Event) => {
  const target = event.target as HTMLElement;
  const dropdown = document.querySelector(".dropdown-container");
  if (dropdown && !dropdown.contains(target)) {
    closeDropdown();
  }
};

// Add click outside listener when dropdown is open
const handleDropdownToggle = () => {
  toggleDropdown();
  if (isDropdownVisible.value) {
    document.addEventListener("click", handleClickOutside);
  } else {
    document.removeEventListener("click", handleClickOutside);
  }
};

// Cleanup listener when component unmounts
import { onUnmounted } from "vue";
onUnmounted(() => {
  document.removeEventListener("click", handleClickOutside);
});
</script>

<template>
  <div class="dropdown-container">
    <button :class="props.buttonClass" @click="handleDropdownToggle">
      <slot name="button" class="dropdown-button">
        <!-- Default button content -->
        <i class="pi pi-user" />
        <i class="pi pi-angle-down" />
      </slot>
    </button>
    <div v-show="isDropdownVisible" :class="props.dropdownClass">
      <slot name="dropdown" :close="closeDropdown">
        <!-- Default dropdown content -->
        <ul>
          <li><i class="pi pi-user"></i> Default Item 1</li>
          <li><i class="pi pi-cog"></i> Default Item 2</li>
        </ul>
      </slot>
    </div>
  </div>
</template>

<style scoped>
.dropdown-container {
  position: relative;
  display: inline-block;
}

.dropdown-button {
  background: none;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem;
  border-radius: 0.25rem;
  transition: background-color 0.2s ease;
}

.dropdown-button:hover {
  background: var(--primary-background-color-hover, rgba(0, 0, 0, 0.05));
}

.dropdown-menu {
  position: absolute;
  box-sizing: border-box;
  top: 100%;
  right: 0;
  background: var(--primary-background-color);
  border: 1px solid var(--border-color, #ccc);
  border-radius: 0.5rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  min-width: 150px;
  z-index: 1000;
  margin-top: 0.25rem;
}

.dropdown-menu :deep(ul) {
  list-style: none;
  padding: 0;
  margin: 0;
}

.dropdown-menu :deep(li) {
  padding: 0.75rem 1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--primary-text-color);
  transition: background-color 0.2s ease;
}

.dropdown-menu :deep(li:hover) {
  background: var(--primary-background-color-hover, rgba(0, 0, 0, 0.05));
}

.dropdown-menu :deep(li:first-child) {
  border-top-left-radius: 0.5rem;
  border-top-right-radius: 0.5rem;
}

.dropdown-menu :deep(li:last-child) {
  border-bottom-left-radius: 0.5rem;
  border-bottom-right-radius: 0.5rem;
}
</style>