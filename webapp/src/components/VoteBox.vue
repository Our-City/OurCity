<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax 
  ChatGPT was asked to help with handling the voting logic and emits.-->
<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/authenticationStore";
import { VoteType } from "@/types/enums";

const router = useRouter();
const auth = useAuthStore();

const isLoggedIn = computed(() => auth.isAuthenticated);

const props = defineProps<{
  votes: number;
  userVote?: VoteType; // -1 | 0 | 1 (optional)
}>();

const emit = defineEmits<{
  (e: "vote", voteType: VoteType): void;
}>();

function handleUpvote() {
  if (!isLoggedIn.value) {
    router.push("/login");
    return;
  }

  let newVoteType: VoteType;

  if (props.userVote === VoteType.UPVOTE) {
    // user is removing their upvote
    newVoteType = VoteType.NOVOTE;
  } else {
    // user is upvoting (either from no vote or from downvote)
    newVoteType = VoteType.UPVOTE;
  }

  emit("vote", newVoteType);
}

function handleDownvote() {
  if (!isLoggedIn.value) {
    router.push("/login");
    return;
  }

  let newVoteType: VoteType;

  if (props.userVote === VoteType.DOWNVOTE) {
    // User is removing their downvote
    newVoteType = VoteType.NOVOTE;
  } else {
    // User is downvoting (either from no vote or from upvote)
    newVoteType = VoteType.DOWNVOTE;
  }

  emit("vote", newVoteType);
}
</script>

<template>
  <div class="vote-box">
    <button
      class="vote-btn upvote"
      :class="{ active: props.userVote === VoteType.UPVOTE }"
      @click="handleUpvote"
    >
      <i class="pi pi-arrow-up" />
    </button>

    <span class="vote-count">{{ votes }}</span>

    <button
      class="vote-btn downvote"
      :class="{ active: props.userVote === VoteType.DOWNVOTE }"
      @click="handleDownvote"
    >
      <i class="pi pi-arrow-down" />
    </button>
  </div>
</template>

<style scoped>
.vote-box {
  display: flex;
  align-items: center;
  border-radius: 3rem;
}

.vote-btn {
  width: 32px;
  height: 32px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  border: none;
  border-radius: 4px;
  background: transparent;
  cursor: pointer;
  color: var(--primary-text-color);
  transition:
    background-color 150ms ease,
    color 150ms ease,
    transform 120ms ease;
}

.vote-btn:hover {
  transform: translateY(-1px);
}

.vote-btn.upvote:hover {
  color: var(--positive-color);
}

.vote-btn.upvote.active {
  color: var(--positive-color);
}

.vote-btn.downvote:hover {
  color: var(--negative-color);
}

.vote-btn.downvote.active {
  color: var(--negative-color);
}

.vote-btn .pi {
  font-size: 0.9rem;
}

.vote-count {
  font-weight: bold;
  font-size: 0.9rem;
  min-width: 2rem;
  text-align: center;
  color: var(--text-color);
}
</style>
