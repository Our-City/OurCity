<script setup lang="ts">
import { ref, onMounted } from "vue";
import TextArea from "primevue/textarea";
import PageHeader from "@/components/PageHeader.vue";
import SideBar from "@/components/SideBar.vue";
import ImageGalleria from "@/components/ImageGalleria.vue";
import VoteBox from "@/components/VoteBox.vue";
import CommentList from "@/components/CommentList.vue";
import { mockComments } from "@/data/mockData.ts";

const images = ref();
const comments = ref(mockComments);

const commentText = ref("");

function submitComment() {}

function handleUpvote() {}

function handleDownvote() {}

onMounted(() => {
  images.value = [
    {
      src: "https://images.unsplash.com/photo-1591658522986-9eb791d2a89a?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1652",
      alt: "Image 1 Description",
      title: "Image 1 Title",
    },
    {
      src: "https://images.unsplash.com/photo-1598235742333-7e89b0104119?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1632",
      alt: "Image 3 Description",
      title: "Image 3 Title",
    },
    {
      src: "https://images.unsplash.com/photo-1591658523269-4f48b4210c58?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1675",
      alt: "Image 2 Description",
      title: "Image 2 Title",
    },
    {
      src: "https://images.unsplash.com/photo-1574294116096-dc5d5015b42a?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=870",
      alt: "Image 4 Description",
      title: "Image 4 Title",
    },
  ];
});
</script>

<template>
  <div class="page-header">
    <PageHeader />
  </div>
  <div class="post-detail-layout">
    <div class="side-bar">
      <SideBar view="home" />
    </div>
    <div class="post-detail-body">
      <div class="post-detail-content-layout">
        <div class="post-content">
          <div class="post-card">
            <div class="post-tags">Tags Placeholder</div>
            <h1 class="post-title">Exploring the City Streets</h1>
            <div class="post-author">@username · 6 days ago</div>

            <div class="post-images">
              <div v-if="images" class="post-images">
                <ImageGalleria :images="images" />
              </div>
            </div>

            <div class="post-description">
              Post description Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum
              mattis vehicula sagittis. Nunc ut suscipit justo. Praesent ut enim vel elit fringilla
              iaculis quis at est. Nullam augue nunc, auctor eget dolor quis, sodales vehicula erat.
              In hac habitasse platea dictumst. Curabitur id diam a est lobortis sollicitudin eget
              eget dui. Praesent mattis ullamcorper purus a pellentesque. Phasellus quis risus ut
              ligula hendrerit finibus. Pellentesque interdum ligula nec egestas varius. Fusce erat
              nibh, suscipit et aliquam nec, ornare ac lectus. Fusce eu erat scelerisque, luctus
              ante a, volutpat leo. Donec congue nibh odio, eget viverra odio commodo ac. Fusce
              eleifend tincidunt convallis. Suspendisse potenti. Curabitur molestie dolor ac
              suscipit consequat. Proin eu lacus nec tortor sodales semper. Nulla diam tellus,
              posuere quis lectus id, malesuada rhoncus tellus. Duis libero metus, mollis sed
              aliquam rhoncus, suscipit eu dui. Aliquam vel elit porttitor diam bibendum rutrum. Nam
              posuere purus ligula, sed fermentum sapien condimentum non. Aenean rutrum sagittis
              eros, at dapibus augue ullamcorper vitae. Cras porttitor vulputate erat non faucibus.
              Nunc volutpat, augue fringilla commodo aliquam, elit neque fermentum dolor, tempus
              tempus nisi odio sed ipsum.
            </div>
            <div class="post-footer">
              <div class="post-voting">
                <VoteBox :votes="318" @upvote="handleUpvote" @downvote="handleDownvote" />
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
              ></TextArea>
              <button class="comment-submit-button" @click="submitComment">Submit</button>
            </div>
            <CommentList :props="comments" />
          </div>
        </div>
        <div class="map-overview">
          Map Overview Coming Soon
          <div class="spinner"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
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
