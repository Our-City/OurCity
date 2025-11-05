<!-- Generative AI was used to assist in the creation of this file.
  ChatGPT was asked to generate code to help integrate the Post service layer API calls-->
<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import type { Post } from "@/models/post";
import type { Media } from "@/models/media";
import { getMediaByPostId } from "@/api/mediaService";

const props = defineProps<{ post: Post }>();

// fetch post media
const media = ref<Media[]>([]);
const isLoading = ref(false);

onMounted(async () => {
  try {
    isLoading.value = true;
    const res = await getMediaByPostId(props.post.id);
    console.log("Fetched media for post", props.post.id, res);
    media.value = res;
  } catch (error) {
    console.error("Failed to fetch post media:", error);
  } finally {
    isLoading.value = false;
  }
});

// author name (uses authorName if available)
const authorUsername = computed(() => {
  if (props.post.authorName && props.post.authorName.trim()) {
    return props.post.authorName;
  }
  return `User #${props.post.authorId}`;
});

// comment count from domain model
const commentCount = computed(() => props.post.commentCount ?? 0);

// first image from fetched media
const postImage = computed(() => media.value[0]?.url || null);
</script>

<template>
  <div class="post-card">
    <div class="post-card-left">
      <div class="post-author-date">
        {{ authorUsername }}
      </div>
      <h1 class="post-title">
        {{ post.title }}
      </h1>
      <div class="post-tags">
        {{ post.location }}
      </div>
      <div class="post-votes-comments">
        <i class="pi pi-sort-alt"></i>
        <div class="post-number-stats">{{ post.voteCount }}</div>
        <i class="pi pi-comments"></i>
        <div class="post-number-stats">{{ commentCount }}</div>
      </div>
    </div>

    <div v-if="postImage && !isLoading" class="post-card-right">
      <img :src="postImage" alt="Post Image" class="post-image" />
    </div>
    <div v-else-if="isLoading" class="post-card-right">
      <i class="pi pi-spin pi-spinner"></i>
    </div>
  </div>
</template>

<style scoped>
.post-card {
  display: flex;
  width: 100%;
  max-width: 100%;
  height: 9.5rem;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
  border-radius: 1rem;
  padding: 1.25rem;
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.05);
  box-sizing: border-box;
  overflow: hidden;
  gap: 1rem;
}

.post-card-left {
  display: flex;
  flex-direction: column;
  width: 83%;
  min-width: 0;
  justify-content: space-between;
  overflow: hidden;
}

.post-author-date {
  font-size: 1rem;
  color: var(--tertiary-text-color);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 100%;
  min-width: 0;
}

.post-title {
  font-size: clamp(1rem, 2vw, 2rem);
  font-weight: 800;
  color: var(--primary-text-color);
  white-space: nowrap;
  overflow: visible;
  text-overflow: ellipsis;
  max-width: 100%;
  min-width: 0;
  margin: 0;
  line-height: 1.2;
}

.post-tags {
  font-size: 1rem;
  color: var(--tertiary-text-color);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 100%;
  min-width: 0;
}

.post-votes-comments {
  display: flex;
  flex-direction: row;
  align-items: center;
  color: var(--tertiary-text-color);
  margin-top: 1rem;
  gap: 0.5rem;
}

.post-votes-comments .pi {
  font-size: 1rem;
  vertical-align: middle;
}

.post-number-stats {
  font-size: 1rem;
  margin-right: 1rem;
}

.post-card-right {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 17%;
}

.post-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  aspect-ratio: 1 / 1;
  border: 0.1rem solid var(--border-color);
  border-radius: 1rem;
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.05);
}
</style>
