<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.-->
<script setup lang="ts">
import { ref, onUnmounted } from "vue";

interface Props {
  buttonClass?: string;
  dropdownClass?: string;
}

const props = withDefaults(defineProps<Props>(), {
  buttonClass: "dropdown-button",
  dropdownClass: "dropdown-menu",
});

const isDropdownVisible = ref(false);
const dropdownRef = ref<HTMLElement | null>(null);

const toggleDropdown = () => {
  isDropdownVisible.value = !isDropdownVisible.value;
};

const closeDropdown = () => {
  isDropdownVisible.value = false;
};

// --- Scoped click-outside handler ---
const handleClickOutside = (event: MouseEvent) => {
  const target = event.target as Node;
  if (dropdownRef.value && !dropdownRef.value.contains(target)) {
    closeDropdown();
  }
};

const handleDropdownToggle = () => {
  toggleDropdown();
  if (isDropdownVisible.value) {
    document.addEventListener("click", handleClickOutside);
  } else {
    document.removeEventListener("click", handleClickOutside);
  }
};

onUnmounted(() => {
  document.removeEventListener("click", handleClickOutside);
});
</script>

<template>
  <div class="dropdown-container" ref="dropdownRef">
    <button :class="props.buttonClass" @click="handleDropdownToggle">
      <slot name="button" class="dropdown-button">
        <i class="pi pi-user" />
        <i class="pi pi-angle-down" />
      </slot>
    </button>

    <div v-show="isDropdownVisible" :class="props.dropdownClass">
      <!-- Slot passes down the close function -->
      <slot name="dropdown" :close="closeDropdown">
        <ul>
          <li><i class="pi pi-user"></i> Default Item 1</li>
          <li><i class="pi pi-cog"></i> Default Item 2</li>
        </ul>
      </slot>
    </div>
  </div>
</template>
