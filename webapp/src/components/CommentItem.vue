<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  Also assisted with voting and deletion logic. -->
<script setup lang="ts">
import { ref } from "vue";
import VoteBox from "@/components/VoteBox.vue";
import { voteOnComment, deleteComment } from "@/api/commentService";
import { VoteType } from "@/types/enums";
import type { Comment } from "@/models/comment";
import { useAuthStore } from "@/stores/authenticationStore";

const props = defineProps<{
  comment: Comment;
}>();

const emit = defineEmits<{
  (e: "updated", updated: Comment): void;
  (e: "deleted", commentId: string): void;
}>();

const auth = useAuthStore();
const error = ref<string | null>(null);
const isDeleting = ref(false);

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

async function handleDelete() {
  if (!confirm("Are you sure you want to delete this comment?")) {
    return;
  }

  isDeleting.value = true;
  error.value = null;

  try {
    await deleteComment(props.comment.id);
    emit("deleted", props.comment.id);
  } catch (err) {
    console.error("Failed to delete comment:", err);
    error.value = "Failed to delete comment. Please try again.";
  } finally {
    isDeleting.value = false;
  }
}
</script>

<template>
  <div class="comment-item">
    <div class="comment-header">
      <div class="comment-header-left">
        <span class="comment-author">@{{ comment.authorName }}</span>
        <span class="comment-date">{{ comment.createdAt.toLocaleDateString() }}</span>
      </div>
      <button
        v-if="comment.canMutate"
        class="delete-button"
        :disabled="isDeleting"
        @click="handleDelete"
        title="Delete comment"
      >
        <i class="pi pi-trash"></i>
      </button>
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
  padding: 0.75rem;
  margin-left: 2rem;
  margin-right: 2rem;
}
.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.25rem;
}
.comment-header-left {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
.delete-button {
  background: none;
  border: none;
  color: var(--tertiary-text-color);
  cursor: pointer;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  transition: all 0.2s ease;
  font-size: 0.875rem;
}
.delete-button:hover {
  color: var(--error-color);
  background: rgba(248, 113, 113, 0.1);
}
.delete-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
.comment-author {
  font-size: 1.1rem;
  color: var(--neutral-color);
}
.comment-date {
  font-size: 1rem;
  color: var(--tertiary-text-color);
}
.comment-text {
  font-size: 1.1rem;
  color: var(--primary-text-color);
  margin-bottom: 0.25rem;
}
.comment-votes {
  font-size: 1rem;
  margin-top: 1rem;
  width: 2rem;
  height: 1rem;
  color: var(--primary-text-color);
}
</style>
