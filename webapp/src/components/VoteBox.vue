<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax 
  ChatGPT was asked to help with handling the voting logic and emits.-->
<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/authenticationStore";

const router = useRouter();
const auth = useAuthStore();

const isLoggedIn = computed(() => auth.isAuthenticated);

const props = defineProps<{
  votes: number;
  userVote?: number; // -1 | 0 | 1 (optional)
}>();

const emit = defineEmits<{
  (e: "vote", voteType: number): void;
}>();

function handleUpvote() {
  if (!isLoggedIn.value) {
    router.push("/login");
    return;
  } else {
    const next = props.userVote === 1 ? 0 : 1;
    emit("vote", next);
  }
}

function handleDownvote() {
  if (!isLoggedIn.value) {
    router.push("/login");
    return;
  } else {
    const next = props.userVote === -1 ? 0 : -1;
    emit("vote", next);
  }
}
</script>

<template>
  <div class="vote-box">
    <button class="upvote-button" :class="{ active: userVote === 1 }" @click="handleUpvote">
      <i class="pi pi-arrow-up" />
    </button>

    <span class="vote-count">{{ votes }}</span>

    <button class="downvote-button" :class="{ active: userVote === -1 }" @click="handleDownvote">
      <i class="pi pi-arrow-down" />
    </button>
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
  color: var(--primary-text-color) !important;
}
.vote-btn.p-button .pi {
  font-size: 0.9rem;
}
.vote-btn.p-button:hover {
  color: var(--primary-text-color) !important;
  transform: translateY(-1px);
}

.vote-btn.upvote.p-button:hover {
  color: var(--positive-color) !important;
}

.vote-btn.downvote.p-button:hover {
  color: var(--negative-color) !important;
}
.vote-count {
  font-weight: bold;
  font-size: 1rem;
  margin: 0.5rem 0.25rem;
  color: var(--text-color);
}
</style>
