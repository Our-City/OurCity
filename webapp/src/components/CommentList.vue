<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax -->
<script setup lang="ts">
import { ref } from "vue";
import CommentItem from "./CommentItem.vue";
import type { Comment } from "@/models/comment";

const props = defineProps<{
  comments: Comment[];
}>();

// Make a local copy to allow optimistic updates
const localComments = ref<Comment[]>([...props.comments]);

function handleUpdated(updated: Comment) {
  const index = localComments.value.findIndex((c) => c.id === updated.id);
  if (index !== -1) {
    localComments.value[index] = updated;
  }
}

function handleDeleted(id: string) {
  localComments.value = localComments.value.filter((c) => c.id !== id);
}
</script>

<template>
  <div class="comments-list">
    <CommentItem
      v-for="comment in localComments"
      :key="comment.id"
      :comment="comment"
      @updated="handleUpdated"
      @deleted="handleDeleted"
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
