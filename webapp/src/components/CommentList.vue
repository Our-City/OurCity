<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax -->
<script setup lang="ts">
import CommentItem from "./CommentItem.vue";
import type { CommentResponseDto } from "@/types/comments.ts";

defineProps<{ props: CommentResponseDto[] }>();
</script>

<template>
  <div class="comments-list">
    <CommentItem
      v-for="comment in props"
      :props="comment"
    />
  </div>
</template>

<style scoped>
.comments-list {
  margin-top: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
</style>