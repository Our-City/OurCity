<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax -->
<script setup lang="ts">
import "@/assets/styles/smooth-transition.css";

const props = defineProps<{ show: boolean; imageUrl?: string; title?: string }>();
const emit = defineEmits(["close"]);
</script>

<template>
  <transition name="smooth-transition">
    <div v-if="props.show" class="modal-layout" @click="emit('close')">
      <button class="close-modal-btn" @click="emit('close')">&times;</button>
      <div class="modal-content" @click.stop>
        <img :src="props.imageUrl" :alt="props.title" class="modal-image" />
      </div>
    </div>
  </transition>
</template>

<style scoped>
.modal-layout {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.95);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 3;
}
.close-modal-btn {
  position: absolute;
  top: 2rem;
  right: 2rem;
  background: none;
  border: none;
  font-size: 4rem;
  cursor: pointer;
  color: var(--secondary-text-color);
}
.close-modal-btn:hover {
  color: var(--negative-color, #d9534f);
}
.modal-image {
  max-width: 85vw;
  max-height: 85vh;
}
</style>