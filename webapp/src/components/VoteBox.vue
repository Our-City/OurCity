<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax -->
<script setup lang="ts">
import VoteButton from "primevue/button";

const props = defineProps<{ votes: number }>();

const emit = defineEmits<{
  (e: "upvote"): void;
  (e: "downvote"): void;
}>();

function upvote() {
  emit("upvote");
}

function downvote() {
  emit("downvote");
}
</script>

<template>
  <div class="vote-section">
    <div class="vote-box">
      <VoteButton icon="pi pi-arrow-up" class="vote-btn upvote" @click.prevent="upvote" />
      <div class="vote-count">{{ props.votes }}</div>
      <VoteButton icon="pi pi-arrow-down" class="vote-btn downvote" @click.prevent="downvote" />
    </div>
  </div>
</template>

<style scoped>
.vote-section {
  display: flex;
  justify-content: left;
}
.vote-box {
  display: flex;
  align-items: center;
  border: none;
  border-radius: 3rem;
}
.vote-btn {
  width: 28px;
  height: 28px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  border-radius: 4px;
  transition:
    background-color 150ms ease,
    color 150ms ease,
    transform 120ms ease;
}
.vote-btn.p-button {
  background: transparent !important;
  border: none !important;
  box-shadow: none !important;
  color: var(--subtle-text-color) !important;
}
.vote-btn.p-button .pi {
  font-size: 0.9rem;
}
.vote-btn.p-button:hover {
  color: var(--text-color) !important;
  transform: translateY(-1px);
}

.vote-btn.upvote.p-button:hover {
  color: var(--secondary-color) !important;
}

.vote-btn.downvote.p-button:hover {
  color: var(--danger-color) !important;
}
.vote-count {
  font-weight: bold;
  font-size: 1rem;
  margin: 0.5rem 0.25rem;
  color: var(--text-color);
}
</style>