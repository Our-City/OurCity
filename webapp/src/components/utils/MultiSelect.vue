<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";

interface Props {
  modelValue: string[];
  options: string[];
  placeholder?: string;
  disabled?: boolean;
  maxSelected?: number;
  searchable?: boolean;
  maxHeight?: string;
}

interface Emits {
  "update:modelValue": [value: string[]];
  change: [value: string[]];
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: "Select options...",
  disabled: false,
  maxSelected: undefined,
  searchable: true,
  maxHeight: "200px",
});

const emit = defineEmits<Emits>();

const isOpen = ref(false);
const searchQuery = ref("");
const multiSelectRef = ref<HTMLElement>();
const searchInputRef = ref<HTMLInputElement>();

// Computed properties
const selectedValues = computed({
  get: () => props.modelValue,
  set: (value) => {
    emit("update:modelValue", value);
    emit("change", value);
  },
});

const filteredOptions = computed(() => {
  if (!props.searchable || !searchQuery.value) {
    return props.options;
  }

  return props.options.filter((option) =>
    option.toLowerCase().includes(searchQuery.value.toLowerCase()),
  );
});

const isMaxSelected = computed(() => {
  return props.maxSelected !== undefined && selectedValues.value.length >= props.maxSelected;
});

// Methods
const toggleDropdown = () => {
  if (props.disabled) return;

  isOpen.value = !isOpen.value;

  if (isOpen.value && props.searchable) {
    // Focus search input when dropdown opens
    setTimeout(() => {
      searchInputRef.value?.focus();
    }, 100);
  }
};

const closeDropdown = () => {
  isOpen.value = false;
  searchQuery.value = "";
};

const selectOption = (option: string) => {
  if (props.disabled) return;

  const currentValues = [...selectedValues.value];
  const index = currentValues.indexOf(option);

  if (index > -1) {
    // Remove if already selected
    currentValues.splice(index, 1);
  } else {
    // Add if not selected and not at max
    if (!isMaxSelected.value) {
      currentValues.push(option);
    }
  }

  selectedValues.value = currentValues;
};

const removeOption = (option: string, event?: Event) => {
  if (event) {
    event.stopPropagation();
  }

  if (props.disabled) return;

  const currentValues = [...selectedValues.value];
  const index = currentValues.indexOf(option);

  if (index > -1) {
    currentValues.splice(index, 1);
    selectedValues.value = currentValues;
  }
};

const clearAll = (event: Event) => {
  event.stopPropagation();
  if (props.disabled) return;

  selectedValues.value = [];
};

const isSelected = (option: string) => {
  return selectedValues.value.includes(option);
};

// Click outside handler
const handleClickOutside = (event: Event) => {
  if (multiSelectRef.value && !multiSelectRef.value.contains(event.target as Node)) {
    closeDropdown();
  }
};

// Lifecycle
onMounted(() => {
  document.addEventListener("click", handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener("click", handleClickOutside);
});
</script>

<template>
  <div
    ref="multiSelectRef"
    class="multiselect"
    :class="{
      'multiselect--open': isOpen,
      'multiselect--disabled': disabled,
      'multiselect--has-value': selectedValues.length > 0,
    }"
  >
    <!-- Main trigger button -->
    <button type="button" class="multiselect__trigger" :disabled="disabled" @click="toggleDropdown">
      <div class="multiselect__content">
        <!-- Selected items display -->
        <div v-if="selectedValues.length > 0" class="multiselect__selected">
          <span v-for="item in selectedValues.slice(0, 3)" :key="item" class="multiselect__tag">
            {{ item }}
            <button
              type="button"
              class="multiselect__tag-remove"
              @click="removeOption(item, $event)"
            >
              ×
            </button>
          </span>
          <span v-if="selectedValues.length > 3" class="multiselect__more">
            +{{ selectedValues.length - 3 }} more
          </span>
        </div>

        <!-- Placeholder -->
        <span v-else class="multiselect__placeholder">
          {{ placeholder }}
        </span>
      </div>

      <!-- Actions -->
      <div class="multiselect__actions">
        <button
          v-if="selectedValues.length > 0"
          type="button"
          class="multiselect__clear"
          @click="clearAll"
        >
          ×
        </button>
        <i class="multiselect__arrow" :class="{ 'multiselect__arrow--up': isOpen }"> ▼ </i>
      </div>
    </button>

    <!-- Dropdown -->
    <div v-show="isOpen" class="multiselect__dropdown" :style="{ maxHeight: maxHeight }">
      <!-- Search input -->
      <div v-if="searchable" class="multiselect__search">
        <input
          ref="searchInputRef"
          v-model="searchQuery"
          type="text"
          class="multiselect__search-input"
          placeholder="Search options..."
          @click.stop
        />
      </div>

      <!-- Options list -->
      <div class="multiselect__options">
        <div
          v-for="option in filteredOptions"
          :key="option"
          class="multiselect__option"
          :class="{
            'multiselect__option--selected': isSelected(option),
            'multiselect__option--disabled': !isSelected(option) && isMaxSelected,
          }"
          @click="selectOption(option)"
        >
          <div class="multiselect__option-content">
            <span class="multiselect__option-text">{{ option }}</span>
            <i v-if="isSelected(option)" class="multiselect__option-check"> ✓ </i>
          </div>
        </div>

        <!-- No results -->
        <div v-if="filteredOptions.length === 0" class="multiselect__no-results">
          No options found
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.multiselect {
  position: relative;
  width: 100%;
}

.multiselect__trigger {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  min-height: 2.5rem;
  padding: 0.5rem 0.75rem;
  background: var(--primary-background-color);
  border: 1px solid var(--neutral-color);
  border-radius: 0.375rem;
  cursor: pointer;
  transition:
    border-color 0.2s ease,
    box-shadow 0.2s ease;
  text-align: left;
}

.multiselect__trigger:hover:not(:disabled) {
  border-color: var(--link-color);
}

.multiselect--open .multiselect__trigger {
  border-color: var(--link-color);
  box-shadow: 0 0 0 0.125rem rgba(0, 123, 255, 0.25);
}

.multiselect--disabled .multiselect__trigger {
  background: var(--secondary-background-color);
  border-color: var(--neutral-color);
  color: var(--tertiary-text-color);
  cursor: not-allowed;
}

.multiselect__content {
  flex: 1;
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
  min-height: 1.5rem;
  align-items: center;
}

.multiselect__selected {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
  align-items: center;
}

.multiselect__tag {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.125rem 0.375rem;
  background: var(--link-color);
  color: white;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  max-width: 150px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.multiselect__tag-remove {
  background: none;
  border: none;
  color: inherit;
  cursor: pointer;
  padding: 0;
  font-size: 1rem;
  line-height: 1;
  margin-left: 0.125rem;
}

.multiselect__tag-remove:hover {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
}

.multiselect__more {
  font-size: 0.875rem;
  color: var(--tertiary-text-color);
  padding: 0.125rem 0.25rem;
}

.multiselect__placeholder {
  color: var(--tertiary-text-color);
  font-size: 1rem;
}

.multiselect__actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-left: 0.5rem;
}

.multiselect__clear {
  background: none;
  border: none;
  color: var(--tertiary-text-color);
  cursor: pointer;
  font-size: 1.25rem;
  line-height: 1;
  padding: 0;
  border-radius: 50%;
  width: 1.5rem;
  height: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.multiselect__clear:hover {
  background: var(--error-color);
  color: white;
}

.multiselect__arrow {
  color: var(--tertiary-text-color);
  font-size: 0.75rem;
  transition: transform 0.2s ease;
}

.multiselect__arrow--up {
  transform: rotate(180deg);
}

.multiselect__dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: var(--primary-background-color);
  border: 1px solid var(--neutral-color);
  border-radius: 0.375rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  margin-top: 0.25rem;
  overflow: hidden;
}

.multiselect__search {
  padding: 0.5rem;
  border-bottom: 1px solid var(--neutral-color);
}

.multiselect__search-input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid var(--neutral-color);
  border-radius: 0.25rem;
  font-size: 0.875rem;
  outline: none;
}

.multiselect__search-input:focus {
  border-color: var(--link-color);
  box-shadow: 0 0 0 0.125rem rgba(0, 123, 255, 0.25);
}

.multiselect__options {
  max-height: inherit;
  overflow-y: auto;
}

.multiselect__option {
  padding: 0.75rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
  border-bottom: 1px solid var(--neutral-color);
}

.multiselect__option:last-child {
  border-bottom: none;
}

.multiselect__option:hover:not(.multiselect__option--disabled) {
  background: var(--secondary-background-color);
}

.multiselect__option--selected {
  background: var(--secondary-background-color);
  color: var(--link-color);
}

.multiselect__option--disabled {
  color: var(--tertiary-text-color);
  cursor: not-allowed;
  opacity: 0.6;
}

.multiselect__option-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.multiselect__option-text {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.multiselect__option-check {
  color: var(--positive-color);
  font-weight: bold;
  margin-left: 0.5rem;
}

.multiselect__no-results {
  padding: 1rem;
  text-align: center;
  color: var(--tertiary-text-color);
  font-style: italic;
}

/* Scrollbar styling */
.multiselect__options::-webkit-scrollbar {
  width: 0.5rem;
}

.multiselect__options::-webkit-scrollbar-track {
  background: var(--secondary-background-color);
}

.multiselect__options::-webkit-scrollbar-thumb {
  background: var(--neutral-color);
  border-radius: 0.25rem;
}

.multiselect__options::-webkit-scrollbar-thumb:hover {
  background: var(--tertiary-text-color);
}
</style>
