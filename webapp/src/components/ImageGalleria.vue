<script setup lang="ts">
import { ref } from "vue";
import ImageModal from "@/components/ImageModal.vue";

const props = defineProps<{
  images: Array<{
    src: string;
    alt: string;
    title: string;
  }>;
}>();

const imgIndex = ref(0);
const showImageModal = ref(false);

function nextImage() {
  imgIndex.value = (imgIndex.value + 1) % props.images.length;
}

function prevImage() {
  imgIndex.value =
    (((imgIndex.value - 1) % props.images.length) + props.images.length) % props.images.length;
}

function openImageModal() {
  showImageModal.value = true;
}

function closeImageModal() {
  showImageModal.value = false;
}
</script>

<template>
  <div class="post-image-container">
    <div class="image-hover-container">
      <img
        :src="images[imgIndex].src"
        :alt="images[imgIndex].alt"
        class="post-image"
        @click="openImageModal()"
      />
      <div class="image-zoom-icon">
        <i class="pi pi-arrow-up-right-and-arrow-down-left-from-center"></i>
      </div>
      <button
        v-if="images"
        class="image-btn image-prev"
        @click="prevImage()"
        aria-label="Previous image"
      >
        <i class="pi pi-chevron-left"></i>
      </button>
      <button
        v-if="images"
        class="image-btn image-next"
        @click="nextImage()"
        aria-label="Next image"
      >
        <i class="pi pi-chevron-right"></i>
      </button>
    </div>
    <div class="galleria-indicators">
      <div
        v-for="(img, idx) in images"
        :key="idx"
        :class="['indicator', { active: idx === imgIndex }]"
      ></div>
    </div>
  </div>
  <ImageModal
    :show="showImageModal"
    :imageUrl="images[imgIndex].src"
    :title="images[imgIndex].title"
    @close="closeImageModal"
  />
</template>

<style scoped>
.post-image-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 100%;
  position: relative;
}
.image-hover-container {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 90%;
  cursor: pointer;
}
.image-hover-container:hover .image-zoom-icon {
  opacity: 1;
}
.post-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 2rem;
  display: block;
}
.image-zoom-icon {
  position: absolute;
  right: 1rem;
  bottom: 1rem;
  font-size: 1rem;
  color: var(--secondary-text-color);
  background: rgba(0, 0, 0, 0.4);
  border-radius: 50%;
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.2s;
}
.image-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(0, 0, 0, 0.4);
  border: none;
  border-radius: 50%;
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--secondary-text-color);
  cursor: pointer;
  z-index: 2;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.2s;
}
.image-hover-container:hover .image-btn {
  opacity: 1;
  pointer-events: auto;
}
.image-prev {
  left: 1rem;
}
.image-next {
  right: 1rem;
}

.galleria-indicators {
  display: flex;
  justify-content: center;
  margin-top: 1rem;
  height: 5%;
}
.indicator {
  width: 0.75rem;
  height: 0.75rem;
  border-radius: 50%;
  background: var(--border-color);
  margin: 0 0.25rem;
  transition: background 0.2s;
}
.indicator.active {
  width: 0.75rem;
  height: 0.75rem;
  border-radius: 50%;
  background: var(--neutral-color);
  margin: 0 0.25rem;
  transition: background 0.2s;
}
</style>
