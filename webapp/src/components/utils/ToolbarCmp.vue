<script setup lang="ts">
interface Props {
  variant?: "header" | "standard";
}

const props = withDefaults(defineProps<Props>(), {
  variant: "standard",
});
</script>

<template>
  <div :class="['toolbar', `toolbar--${props.variant}`]">
    <div class="toolbar__start">
      <slot name="start"></slot>
    </div>
    <div class="toolbar__center">
      <slot name="center"></slot>
    </div>
    <div class="toolbar__end">
      <slot name="end"></slot>
    </div>
  </div>
</template>

<style scoped>
.toolbar {
  display: flex;
  gap: 1rem;
  width: 100%;
  height: fit-content;
}

.toolbar--header {
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  padding: 0.5rem 0.5rem;
  border-top-left-radius: 1rem;
  border-top-right-radius: 1rem;
}

.toolbar--standard {
  background: var(--secondary-background-color, #f5f5f5);
  color: var(--primary-text-color);
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
}

.toolbar__start,
.toolbar__end {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-grow: 0;
}

.toolbar__center {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-grow: 1;
}

/* Button styles for standard toolbar */
.toolbar--standard :deep(button) {
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  font-size: 1rem;
  padding: 0.5rem 1rem;
  border: 1px solid rgba(0, 0, 0, 0.1);
  border-radius: 0.5rem;
  transition:
    background 0.2s,
    color 0.2s,
    box-shadow 0.2s;
  cursor: pointer;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.toolbar--standard :deep(button:hover) {
  background: var(--primary-background-color-hover);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.15);
}

.toolbar--standard :deep(button:active) {
  transform: translateY(1px);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}
</style>
