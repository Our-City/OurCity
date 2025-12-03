<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  ChatGPT was asked to generate code to help integrate the Post service layer API calls.
  e.g, loading posts, creating new posts, etc.
  Also assisted with handling comment updates from child CommentList. -->
<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import TextArea from "primevue/textarea";
import PageHeader from "@/components/PageHeader.vue";
import SideBar from "@/components/SideBar.vue";
import ImageGalleria from "@/components/ImageGalleria.vue";
import VoteBox from "@/components/VoteBox.vue";
import CommentList from "@/components/CommentList.vue";
import Dropdown from "@/components/utils/DropdownMenu.vue";
import { useToast } from "primevue/usetoast";
import MapDisplay from "@/components/MapDisplay.vue";
import { removePostalCode } from "@/utils/locationFormatter";

import { getPostById, voteOnPost, deletePost } from "@/api/postService";
import { getMediaByPostId } from "@/api/mediaService";
import { getCommentsByPostId, createComment } from "@/api/commentService";

import type { Post } from "@/models/post";
import type { Media } from "@/models/media";
import type { Comment } from "@/models/comment";
import { VoteType } from "@/types/enums";
import { useAuthStore } from "@/stores/authenticationStore";

const route = useRoute();
const router = useRouter();
const postId = route.params.id as string;
const toast = useToast();

const post = ref<Post | null>(null);
const images = ref<Media[]>([]);
const comments = ref<Comment[]>([]);
const commentText = ref("");
const isSubmitting = ref(false);
const isLoading = ref(true);
const errorMessage = ref<string | null>(null);

const auth = useAuthStore();

const canMutate = computed(() => {
  return post.value?.canMutate ?? false;
});

// fetch the post, its media, and its comments
async function loadPostData() {
  try {
    isLoading.value = true;

    post.value = await getPostById(postId);

    // Check if post is deleted
    if (post.value.isDeleted) {
      errorMessage.value = "Post not found";
      isLoading.value = false;
      return;
    }

    images.value = await getMediaByPostId(postId);
    const { items } = await getCommentsByPostId(postId);
    comments.value = items;
  } catch (err) {
    console.error("Failed to load post details:", err);

    // handle 404 specifically
    if (
      err instanceof Error &&
      (err.message?.includes("not found") || err.message?.includes("404"))
    ) {
      errorMessage.value = "Post not found";
    } else {
      errorMessage.value = "Failed to load post details.";
    }
  } finally {
    isLoading.value = false;
  }
}

// Computed property for formatted location
const formattedLocation = computed(() => {
  if (!post.value?.location) return "";
  return removePostalCode(post.value.location);
});

// submit a new comment on the post
async function submitComment() {
  if (!auth.user) {
    router.push("/login");
    return;
  }

  const text = commentText.value.trim();
  if (!text) return;

  isSubmitting.value = true;

  try {
    const created = await createComment(postId, { content: text } as Comment);

    // add the server response to the beginning of the comments array
    comments.value = [created, ...comments.value];
    commentText.value = "";

    // increment the comment count on the post
    if (post.value) {
      post.value = { ...post.value, commentCount: post.value.commentCount + 1 };
    }

    toast.add({ severity: "success", summary: "Comment created successfully.", life: 3000 });
  } catch (err) {
    console.error("Failed to create comment:", err);
    alert("Failed to submit comment. Please try again.");
  } finally {
    isSubmitting.value = false;
  }
}

// handle updated comment from CommentList
function handleCommentUpdated(updated: Comment) {
  const idx = comments.value.findIndex((c) => c.id === updated.id);
  if (idx !== -1) {
    // create new array to ensure reactivity
    comments.value = [...comments.value.slice(0, idx), updated, ...comments.value.slice(idx + 1)];
  }
}

// handle deleted comment from CommentList
function handleCommentDeleted(commentId: string) {
  comments.value = comments.value.filter((c) => c.id !== commentId);

  // decrement the comment count on the post
  if (post.value) {
    post.value = { ...post.value, commentCount: post.value.commentCount - 1 };
  }

  toast.add({ severity: "success", summary: "Comment deleted successfully.", life: 3000 });
}

// handle voting on the post
async function handleVote(voteType: VoteType) {
  if (!post.value) return;
  try {
    const updated = await voteOnPost(post.value.id, voteType);

    // update the post with the new vote data
    // voteCount is calculated in the mapper as upvoteCount - downvoteCount
    post.value = {
      ...post.value,
      voteCount: updated.voteCount,
      upvoteCount: updated.upvoteCount,
      downvoteCount: updated.downvoteCount,
      voteStatus: updated.voteStatus,
      updatedAt: updated.updatedAt,
    };
  } catch (err) {
    console.error("Vote failed:", err);
  }
}

async function handleShare() {
  try {
    const origin = typeof window !== "undefined" ? window.location.origin : "";
    const path =
      route?.fullPath ??
      (typeof window !== "undefined"
        ? window.location.pathname + window.location.search + window.location.hash
        : "");
    const url = `${origin}${path}`;

    if (navigator.clipboard && navigator.clipboard.writeText) {
      await navigator.clipboard.writeText(url);
    } else {
      // fallback for older browsers
      const ta = document.createElement("textarea");
      ta.value = url;
      ta.setAttribute("readonly", "");
      ta.style.position = "absolute";
      ta.style.left = "-9999px";
      document.body.appendChild(ta);
      ta.select();
      document.execCommand("copy");
      document.body.removeChild(ta);
    }

    toast.add({ severity: "secondary", summary: "Link copied to clipboard.", life: 3000 });
  } catch (err) {
    console.error("Failed to copy link:", err);
    alert("Failed to copy link to clipboard");
  }
}

async function handleReport() {
  if (!auth.user) {
    router.push("/login");
    return;
  }

  // implement report api call
}

async function handleDelete() {
  // ensure user can mutate as per backend authorization
  if (!canMutate.value) {
    return;
  }

  if (!confirm("Are you sure you want to delete this post?")) {
    return;
  }

  try {
    await deletePost(postId);
    toast.add({
      severity: "success",
      summary: "Post deleted successfully",
      life: 3000,
    });
    router.push("/");
  } catch (err) {
    console.error("Failed to delete post:", err);
    toast.add({
      severity: "error",
      summary: "Failed to delete post",
      detail: "Please try again later.",
      life: 5000,
    });
  }
}

async function handleBookmark() {
  if (!auth.user) {
    router.push("/login");
    return;
  }

  // implement bookmark api call
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
        <div v-if="isLoading" class="loading-state">
          <i class="pi pi-spin pi-spinner"></i>
          <p>Loading post...</p>
        </div>
        <div v-else-if="errorMessage" class="error-state">
          <div class="error-content">
            <i class="pi pi-exclamation-circle error-icon"></i>
            <h2 class="error-title">{{ errorMessage }}</h2>
            <p class="error-message">
              {{
                errorMessage === "Post not found"
                  ? "This post may have been removed or doesn't exist."
                  : "We encountered an issue loading this post. Please try again later."
              }}
            </p>
            <button class="error-button" @click="router.push('/')">
              <i class="pi pi-home"></i>
              Back to Home
            </button>
          </div>
        </div>

        <div v-else-if="post" class="post-detail-content-layout">
          <div class="post-content">
            <div class="post-card">
              <div class="post-header">
                <h1 class="post-title" data-testid="post-title">{{ post.title }}</h1>
                <Dropdown>
                  <template #button>
                    <i class="pi pi-ellipsis-v"></i>
                  </template>
                  <template #dropdown="{ close }">
                    <ul>
                      <li
                        @click="
                          handleShare();
                          close();
                        "
                      >
                        <i class="pi pi-share-alt"></i> Save
                      </li>
                      <li
                        @click="
                          handleBookmark();
                          close();
                        "
                      >
                        <i class="pi pi-bookmark"></i> Save
                      </li>
                      <li
                        @click="
                          handleReport();
                          close();
                        "
                      >
                        <i class="pi pi-flag"></i> Report
                      </li>
                      <li
                        v-if="canMutate"
                        @click="
                          handleDelete();
                          close();
                        "
                      >
                        <i class="pi pi-trash"></i> Delete
                      </li>
                    </ul>
                  </template>
                </Dropdown>
              </div>

              <div class="post-author" data-testid="post-author">
                @{{ post.authorName }} ·
                {{ post.createdAt.toLocaleDateString() }}
              </div>

              <div v-if="formattedLocation" class="post-location">
                {{ formattedLocation }}
              </div>

              <div class="post-tags">
                <span v-for="tag in post.tags" :key="tag.id" class="tag-pill">
                  {{ tag.name }}
                </span>
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

              <CommentList
                :comments="comments"
                @updated="handleCommentUpdated"
                @deleted="handleCommentDeleted"
              />
            </div>
          </div>

          <!-- Sidebar -->
          <div class="map-overview">
            <div class="map-overview-header">
              <i class="pi pi-map-marker"></i>
              <h3>Location</h3>
            </div>
            <MapDisplay
              :latitude="post.latitude"
              :longitude="post.longitude"
              :location-name="formattedLocation"
              height="500px"
              :zoom="15"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.post-detail {
  padding: 1rem;
  height: 100vh;
  overflow: hidden;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}

.post-detail-layout {
  display: flex;
  height: 100vh;
  overflow: hidden;
  padding-bottom: 5rem;
}

.post-detail-body {
  flex: 1;
  background: var(--secondary-background-color);
  min-width: 0;
  overflow-y: auto;
  overflow-x: hidden;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  gap: 1rem;
  color: var(--tertiary-text-color);
}

.loading-state i {
  font-size: 2.5rem;
}

.error-state {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  min-height: 60vh;
}

.error-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
  border-radius: 1rem;
  padding: 3rem 4rem;
  text-align: center;
  max-width: 500px;
}

.error-icon {
  font-size: 4rem;
  color: var(--error-color, #ef4444);
}

.error-title {
  font-size: 1.75rem;
  font-weight: 600;
  margin: 0;
  color: var(--primary-text-color);
}

.error-message {
  font-size: 1.1rem;
  color: var(--tertiary-text-color);
  margin: 0;
}

.error-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: var(--neutral-color);
  color: var(--secondary-text-color);
  border: none;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
  margin-top: 0.5rem;
}

.error-button:hover {
  background: var(--neutral-color-hover);
}

.error-button i {
  font-size: 1.1rem;
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
  padding: 4rem 5rem 3rem 5rem;
}

.post-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.75rem;
  margin-bottom: 1rem;
}

.tag-pill {
  display: inline-block;
  padding: 0.375rem 0.75rem;
  background: var(--secondary-background-color);
  color: var(--tertiary-text-color);
  border-radius: 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  border: 1px solid var(--border-color);
}

.post-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.post-title {
  font-size: 2.75rem;
  font-weight: 800;
}

.post-author {
  font-size: 1.1rem;
  color: var(--tertiary-text-color);
}

.post-location {
  font-size: 1.1rem;
  color: var(--tertiary-text-color);
}

.post-images {
  width: 100%;
  aspect-ratio: 4/3;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 2rem 1rem 2rem;
}

.post-description {
  font-size: 1.1rem;
  padding-top: 1rem;
  margin-bottom: 1rem;
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
  margin-bottom: 1.5rem;
  color: var(--secondary-text-color);
  background: var(--neutral-color);
}

.comment-submit-button:hover {
  background: var(--neutral-color-hover);
}

.map-overview {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  width: 40rem;
  height: 42rem;
  background: var(--primary-background-color);
  border: 0.1rem solid var(--border-color);
  border-radius: 1rem;
  padding: 1.5rem;
  position: sticky;
  top: 1rem;
  overflow: hidden; /* Prevent content overflow */
}

.map-overview-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid var(--border-color);
  flex-shrink: 0;
}

.map-overview-header i {
  font-size: 1.5rem;
  color: var(--primary-color, #3b82f6);
}

.map-overview-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--primary-text-color);
}
</style>
