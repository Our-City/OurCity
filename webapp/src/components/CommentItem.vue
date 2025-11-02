<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax -->
<script setup lang="ts">
// import CommentList from "./CommentList.vue";
import type { CommentResponseDto } from "@/types/comments.ts";
import VoteBox from "@/components/VoteBox.vue";

defineProps<{ props: CommentResponseDto }>();
</script>

<template>
  <div class="comment-item">
    <div class="comment-author">{{ props.authorId }}</div>
    <div class="comment-text">{{ props.content }}</div>
    <VoteBox class="comment-votes" :votes="props.votes ?? 0" />
    <!-- <div v-if="props.replies && props.replies.length > 0" class="comment-replies">
      <CommentList :comments="props.replies" />
    </div> -->
  </div>
</template>

<style scoped>
.comment-item {
  padding: 1rem;
  margin-left: 1rem;
}
.comment-author {
  font-weight: bold;
  color: var(--neutral-color);
  margin-bottom: 0.5rem;
}
.comment-text {
  font-size: 1rem;
  color: var(--primary-text-color);
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
