<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  ChatGPT was asked to generate code to help integrate the Post service layer API calls.
  e.g, loading posts, creating new posts, etc.-->
<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import TextArea from "primevue/textarea";
import PageHeader from "@/components/PageHeader.vue";
import SideBar from "@/components/SideBar.vue";
import ImageGalleria from "@/components/ImageGalleria.vue";
import VoteBox from "@/components/VoteBox.vue";
import CommentList from "@/components/CommentList.vue";

import { getPostById, voteOnPost } from "@/api/postService";
import { getMediaByPostId } from "@/api/mediaService";
import { getCommentsByPostId, createComment } from "@/api/commentService";

import type { Post } from "@/models/post";
import type { Media } from "@/models/media";
import type { Comment } from "@/models/comment";
import { VoteType } from "@/types/enums";
import { useAuthStore } from "@/stores/authenticationStore";

const route = useRoute();
const postId = route.params.id as string;

const post = ref<Post | null>(null);
const images = ref<Media[]>([]);
const comments = ref<Comment[]>([]);
const commentText = ref("");
const isSubmitting = ref(false);
const isLoading = ref(true);
const errorMessage = ref<string | null>(null);

const auth = useAuthStore();

// fetch the post, its media, and its comments
async function loadPostData() {
  try {
    isLoading.value = true;

    post.value = await getPostById(postId);
    images.value = await getMediaByPostId(postId);
    const { items } = await getCommentsByPostId(postId);
    comments.value = items;
  } catch (err) {
    console.error("Failed to load post details:", err);
    errorMessage.value = "Failed to load post details.";
  } finally {
    isLoading.value = false;
  }
}

// submit a new comment on the post
async function submitComment() {
  // replace with authorization endpoint
  if (!auth.user) {
    alert("You must be logged in to comment.");
    return;
  }

  const text = commentText.value.trim();
  if (!text) return;

  isSubmitting.value = true;

  try {
    const newComment: Comment = {
      id: "",
      authorId: auth.user.id,
      postId,
      content: text,
      authorName: auth.user.username,
      upvoteCount: 0,
      downvoteCount: 0,
      voteCount: 0,
      voteStatus: 0,
      isDeleted: false,
      createdAt: new Date(),
      updatedAt: new Date(),
    };

    const created = await createComment(postId, newComment);

    window.location.reload();

    // Optimistically prepend
    comments.value.unshift(created);
    commentText.value = "";
  } catch (err) {
    console.error("Failed to create comment:", err);
  } finally {
    isSubmitting.value = false;
  }
}

// handle voting on the post
async function handleVote(voteType: VoteType) {
  if (!post.value) return;
  try {
    post.value = await voteOnPost(post.value.id, voteType);
  } catch (err) {
    console.error("Vote failed:", err);
  }
}

onMounted(loadPostData);
</script>

<template>
  <div class="post-detail">
    <div class="page-header">
      <PageHeader />
    </div>

    <div class="post-detail-layout">
      <div class="side-bar">
        <SideBar view="home" />
      </div>

      <div class="post-detail-body">
        <div v-if="isLoading" class="loading-state">Loading post...</div>
        <div v-else-if="errorMessage" class="error-state">{{ errorMessage }}</div>

        <div v-else-if="post" class="post-detail-content-layout">
          <div class="post-content">
            <div class="post-card">
              <div class="post-tags">
                <span v-for="tag in post.tags" :key="tag.id" class="tag-pill">
                  {{ tag.name }}
                </span>
              </div>

              <h1 class="post-title">{{ post.title }}</h1>
              <div class="post-author">
                @{{ post.authorName }} ·
                {{ post.createdAt.toLocaleDateString() }}
              </div>

              <div v-if="post.location" class="post-location">
                {{ post.location }}
              </div>

              <div v-if="images.length" class="post-images">
                <ImageGalleria
                  :images="
                    images.map((m) => ({
                      src: m.url,
                    }))
                  "
                />
              </div>

              <div class="post-description">
                {{ post.description }}
              </div>

              <div class="post-footer">
                <div class="post-voting">
                  <VoteBox :votes="post.voteCount" :userVote="post.voteStatus" @vote="handleVote" />
                </div>
              </div>
            </div>

            <div class="comment-card">
              <h1 class="comment-header">Comments ({{ comments.length }})</h1>

              <div class="comment-input-container">
                <TextArea
                  class="comment-input"
                  v-model="commentText"
                  placeholder="Add your thoughts here..."
                  rows="3"
                  :disabled="isSubmitting"
                ></TextArea>

                <button
                  class="comment-submit-button"
                  :disabled="isSubmitting || !commentText.trim()"
                  @click="submitComment"
                >
                  {{ isSubmitting ? "Submitting..." : "Submit" }}
                </button>
              </div>

              <CommentList :comments="comments" />
            </div>
          </div>

          <!-- Sidebar -->
          <div class="map-overview">
            Map Overview Coming Soon
            <div class="spinner"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.post-detail {
  padding: 1rem;
}

.post-detail-layout {
  display: flex;
  height: 100vh;
  overflow: hidden;
}

.post-detail-body {
  flex: 1;
  background: var(--secondary-background-color);
  min-width: 0;
  overflow-y: auto;
  overflow-x: hidden;
}

.post-detail-content-layout {
  display: flex;
  gap: 1rem;
  margin: 1rem 1.5rem 1rem 1rem;
  padding: 1rem 0 0 1rem;
}

.post-content {
  display: flex;
  flex: 1;
  flex-direction: column;
}

.post-card {
  border-radius: 1rem;
  width: 100%;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
  padding: 4rem 4rem;
}

.post-tags {
  font-size: 1.25rem;
  color: var(--tertiary-text-color);
}

.post-title {
  font-size: 3rem;
}

.post-author {
  font-size: 1.25rem;
  color: var(--tertiary-text-color);
}

.post-location {
  font-size: 1.25rem;
  color: var(--tertiary-text-color);
}

.post-images {
  width: 100%;
  aspect-ratio: 4/3;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 2rem;
}

.post-description {
  font-size: 1.25rem;
  margin-bottom: 2rem;
}

.post-footer {
  display: flex;
  justify-content: flex-start;
  align-items: center;
}

.comment-card {
  border-radius: 1rem;
  width: 100%;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
  margin-top: 2rem;
  padding: 2rem 2rem;
  font-size: 1.5rem;
}

.comment-header {
  padding: 2rem 2rem 0 2rem;
  font-size: 2rem;
}

.comment-input-container {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.comment-input {
  padding: 1rem;
  margin: 1rem 2rem;
  border-radius: 1rem;
  resize: vertical;
  box-sizing: border-box;
  width: calc(100% - 4rem);
  border: 0.1rem solid var(--border-color);
}

.comment-submit-button {
  margin-right: 2rem;
  color: var(--secondary-text-color);
  background: var(--neutral-color);
}

.comment-submit-button:hover {
  background: var(--neutral-color-hover);
}

.map-overview {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
  width: 30rem;
  height: 65rem;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
}

.spinner {
  margin-top: 1rem;
  width: 4rem;
  height: 4rem;
  border: 0.25rem solid #ccc;
  border-top: 0.25rem solid #1976d2;
  border-radius: 100%;
  animation: spin 1.5s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
