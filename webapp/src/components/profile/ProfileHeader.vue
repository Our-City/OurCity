<script setup lang="ts">
import { ref } from 'vue';

interface Props {
  username?: string;
}

const props = withDefaults(defineProps<Props>(), {
  username: 'Username'
});

const emit = defineEmits<{
  updateUsername: [username: string]
}>();

const isEditingUsername = ref(false);
const editedUsername = ref(props.username);

function handleEditUsername() {
  isEditingUsername.value = true;
  editedUsername.value = props.username;
}

function saveUsername() {
  if (editedUsername.value.trim()) {
    emit('updateUsername', editedUsername.value.trim());
    isEditingUsername.value = false;
  }
}

function cancelEdit() {
  isEditingUsername.value = false;
  editedUsername.value = props.username;
}

function handleKeyDown(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    saveUsername();
  } else if (event.key === 'Escape') {
    cancelEdit();
  }
}

function handleCreatePost() {
  // TODO: Implement create post functionality
}
</script>

<template>
  <div class="profile-header">
    <div class="profile-content">
      <div v-if="!isEditingUsername" class="username-display">
        <h1 class="profile-username">{{ username }}</h1>
      </div>
      <div v-else class="username-edit">
        <input
          v-model="editedUsername"
          type="text"
          class="username-input"
          @keydown="handleKeyDown"
          autofocus
        />
        <div class="edit-actions">
          <button class="save-button" @click="saveUsername">
            <i class="pi pi-check"></i>
            Save
          </button>
          <button class="cancel-button" @click="cancelEdit">
            <i class="pi pi-times"></i>
            Cancel
          </button>
        </div>
      </div>
      <div class="profile-actions">
        <button class="edit-username-button" @click="handleEditUsername" v-if="!isEditingUsername">
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

.username-edit {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.username-input {
  font-weight: bold;
  font-size: 2rem;
  padding: 0.5rem;
  border: 2px solid var(--neutral-color);
  border-radius: 0.5rem;
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  font-family: inherit;
}

.username-input:focus {
  outline: none;
  border-color: var(--neutral-color-hover);
}

.edit-actions {
  display: flex;
  gap: 0.5rem;
}

.save-button,
.cancel-button {
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

.save-button {
  background: var(--positive-color);
  color: var(--secondary-text-color);
}

.save-button:hover {
  background: #0ea472;
}

.cancel-button {
  background: var(--negative-color);
  color: var(--secondary-text-color);
}

.cancel-button:hover {
  background: #f76565;
}

.save-button:active,
.cancel-button:active {
  transform: translateY(1px);
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