<script setup lang="ts">
import { useRouter } from "vue-router"; 
import { ref, onMounted } from "vue";
import PageHeader from "@/components/PageHeader.vue";
import ImageGalleria from "@/components/ImageGalleria.vue";

const images = ref();

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
    }
  ];
});
</script>

<template>
  <div class="page-header">
    <PageHeader />
  </div>
  <div class="post-detail-layout">
    <div class="side-bar">
      Temporary Sidebar Placeholder 
    </div>
    <div class="post-detail-body">
      <div class="post-detail-content-layout">
        <div class="post-list">

          <div class="post-tags">
            Tags Placeholder
          </div>
          <h1 class="post-title">
            Exploring the City Streets
          </h1>
          <div class="post-author-date">
            @a_real_prof • 6 days ago
          </div>

          <div v-if="images" class="post-images">
            <ImageGalleria :images="images" />
          </div>

          <div class="post-description">
            I had so much fun last weekend around Downtown! I don't really understand why people are always saying that there's nothing to do in Winnipeg. Especially in the Exchange district, I feel like I could literally just spend hours walking around and trying new things.
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
}

.side-bar {
  display: flex; 
  justify-content: center;
  align-items: center; 
  width: 20rem; 
  height: 100%; 
  background: var(--primary-background-color); 
  border: 0.1rem solid var(--border-color);
}

.post-detail-body {
  flex: 1;
  background: var(--primary-background-color);
}

.post-detail-content-layout {
  display: flex; 
  gap: 1rem;
  margin: 1rem 1.5rem 1rem 1rem;
}

.post-list {
  border-radius: 1rem;
  width: 100%;
}

.post-tags {
  font-size: 1.5rem;
}

.post-title {
  font-size: 4rem;
}

.post-author-date {
  font-size: 1.5rem;
}

.post-images {
  width: 100%;
  aspect-ratio: 4/3;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 1rem 0;
  padding: 1rem 0;
}

.post-description {
  font-size: 1.5rem;
}

.map-overview {
  display: flex; 
  flex-direction: column;
  justify-content: center;
  align-items: center; 
  gap: 0.5rem;
  width: 40rem; 
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
  to { transform: rotate(360deg); }
}

</style>
