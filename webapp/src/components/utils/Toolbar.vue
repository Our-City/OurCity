<script setup lang="ts">
interface Props {
  variant?: 'header' | 'standard'
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'standard'
})
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
  height: 100%;
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
</style>