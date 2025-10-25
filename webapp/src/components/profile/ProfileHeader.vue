<script setup lang="ts">
interface Props {
  username?: string;
}

const props = withDefaults(defineProps<Props>(), {
  username: 'Username'
});

function handleEditUsername() {
  // TODO: Implement edit username functionality
}

function handleCreatePost() {
  // TODO: Implement create post functionality
}
</script>

<template>
  <div class="profile-header">
    <div class="profile-content">
      <h1 class="profile-username">{{ username }}</h1>
      <div class="profile-actions">
        <button class="edit-username-button" @click="handleEditUsername">
          <i class="pi pi-pencil"></i>
          Edit Username
        </button>
        <button class="create-post-button" @click="handleCreatePost">
          <i class="pi pi-plus"></i>
          Create Post
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.profile-header {
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  padding: 1rem;
  border-bottom: 1px solid var(--border-color, #ccc);
}

.profile-content {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.profile-username {
  font-weight: bold;
  font-size: 2rem;
  margin: 0;
}

.profile-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.edit-username-button,
.create-post-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.5rem;
  font-size: 1rem;
  cursor: pointer;
  transition: background 0.2s, color 0.2s, transform 0.1s;
}

.edit-username-button {
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.edit-username-button:hover {
  background: var(--primary-background-color-hover);
}

.create-post-button {
  background: var(--neutral-color);
  color: var(--secondary-text-color);
}

.create-post-button:hover {
  background: var(--neutral-color-hover);
}

.edit-username-button:active,
.create-post-button:active {
  transform: translateY(1px);
}
</style>