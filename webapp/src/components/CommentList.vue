<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  Also assisted with vote handling logic. -->
<script setup lang="ts">
import CommentItem from "./CommentItem.vue";
import type { Comment } from "@/models/comment";

const props = defineProps<{ comments: Comment[] }>();
const emit = defineEmits<{ (e: "updated", updated: Comment): void }>();
</script>

<template>
  <div class="comments-list">
    <CommentItem
      v-for="comment in props.comments"
      :key="comment.id"
      :comment="comment"
      @updated="emit('updated', $event)"
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
