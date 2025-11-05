<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  Also assisted with voting logic. -->
<script setup lang="ts">
import { ref } from "vue";
import VoteBox from "@/components/VoteBox.vue";
import { voteOnComment } from "@/api/commentService";
import { VoteType } from "@/types/enums";
import type { Comment } from "@/models/comment";
import { useAuthStore } from "@/stores/authenticationStore";

const props = defineProps<{
  comment: Comment;
}>();

const emit = defineEmits<{
  (e: "updated", updated: Comment): void;
}>();

const auth = useAuthStore();
const error = ref<string | null>(null);

async function handleVote(voteType: VoteType) {
  if (!auth.user) {
    alert("You must be logged in to vote on comments.");
    return;
  }

  try {
    // backend to register vote
    const updatedComment = await voteOnComment(props.comment.id, { voteType });

    // emit the updated comment object so the parent can refresh its local list
    emit("updated", updatedComment);
  } catch (err) {
    console.error("Failed to vote on comment:", err);
    error.value = "Failed to register vote. Please try again.";
  }
}
</script>

<template>
  <div class="comment-item">
    <div class="comment-header">
      <span class="comment-author">@{{ comment.authorName }}</span>
    </div>

    <div class="comment-date">
      <span class="comment-date">{{ comment.createdAt.toLocaleDateString() }}</span>
    </div>

    <div class="comment-body">
      <div class="comment-text">{{ comment.content }}</div>
    </div>

    <div class="comment-actions">
      <VoteBox :votes="comment.voteCount" :userVote="comment.voteStatus" @vote="handleVote" />
    </div>

    <div v-if="error" class="comment-error">{{ error }}</div>
  </div>
</template>

<style scoped>
.comment-item {
  padding: 1rem;
  margin-left: 1rem;
}
.comment-author {
  font-size: 1.25rem;
  color: var(--neutral-color);
  margin-bottom: 0.5rem;
}
.comment-text {
  font-size: 1rem;
  color: var(--primary-text-color);
  margin-bottom: 0.5rem;
}
.comment-date {
  font-size: 1rem;
  color: var(--tertiary-text-color);
  margin-bottom: 0.5rem;
}
.comment-votes {
  font-size: 1rem;
  margin-top: 1rem;
  width: 2rem;
  height: 1rem;
  color: var(--primary-text-color);
}
</style>
