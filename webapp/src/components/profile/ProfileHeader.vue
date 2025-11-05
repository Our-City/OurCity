<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  It also assisted with error handling.-->
<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { updateCurrentUser } from "@/api/userService";
import { useAuthStore } from "@/stores/authenticationStore";
import { resolveErrorMessage } from "@/utils/error";
import { useToast } from "primevue/usetoast";

interface Props {
  username?: string;
}

const router = useRouter();
const auth = useAuthStore();
const toast = useToast();

const props = withDefaults(defineProps<Props>(), {
  username: "Username",
});

const isEditingUsername = ref(false);
const editedUsername = ref(props.username);
const isSaving = ref(false);
const errorMessage = ref<string | null>(null);

// handlers
function handleEditUsername() {
  isEditingUsername.value = true;
  editedUsername.value = props.username;
  errorMessage.value = null;
}

async function saveUsername() {
  const newUsername = editedUsername.value.trim();

  // basic validation
  if (!newUsername) {
    errorMessage.value = "Username cannot be empty.";
    return;
  }
  if (newUsername.length < 3 || newUsername.length > 20) {
    errorMessage.value = "Username must be between 3 and 20 characters.";
    return;
  }
  if (!/^[a-zA-Z0-9_]+$/.test(newUsername)) {
    errorMessage.value = "Username can only contain letters, numbers, and underscores.";
    return;
  }

  // only send update if it actually changed
  if (newUsername === props.username) {
    isEditingUsername.value = false;
    return;
  }

  isSaving.value = true;
  errorMessage.value = null;

  try {
    const updatedUser = await updateCurrentUser({ username: newUsername });

    // update local auth store
    auth.user = updatedUser;

    toast.add({ severity: "secondary", summary: "Username updated successfully.", life: 4000 });

    isEditingUsername.value = false;
  } catch (err: unknown) {
    console.error("Failed to update username:", err);
    errorMessage.value = resolveErrorMessage(err, "Failed to update username. Please try again.");
  } finally {
    isSaving.value = false;
  }
}

function cancelEdit() {
  isEditingUsername.value = false;
  editedUsername.value = props.username;
  errorMessage.value = null;
}

function handleKeyDown(event: KeyboardEvent) {
  if (event.key === "Enter") {
    saveUsername();
  } else if (event.key === "Escape") {
    cancelEdit();
  }
}

function handleCreatePost(): void {
  router.push("/create-post");
}
</script>

<template>
  <div class="profile-header">
    <div class="profile-content">
      <!-- Username display -->
      <div v-if="!isEditingUsername" class="username-display">
        <h1 class="profile-username">{{ username }}</h1>
      </div>

      <!-- Edit mode -->
      <div v-else class="username-edit">
        <input
          v-model="editedUsername"
          type="text"
          class="username-input"
          @keydown="handleKeyDown"
          :disabled="isSaving"
          autofocus
        />
        <div class="edit-actions">
          <button class="save-button" @click="saveUsername" :disabled="isSaving">
            <i class="pi pi-check"></i>
            {{ isSaving ? "Saving..." : "Save" }}
          </button>
          <button class="cancel-button" @click="cancelEdit" :disabled="isSaving">
            <i class="pi pi-times"></i>
            Cancel
          </button>
        </div>

        <div v-if="errorMessage" class="form-error">{{ errorMessage }}</div>
      </div>

      <!-- Profile actions -->
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
  padding: 1.5rem;
  background: var(--primary-background-color);
  border-bottom: 1px solid var(--neutral-color);
}

.username-input {
  font-size: 1.5rem;
  padding: 0.5rem;
  border-radius: 0.5rem;
  width: 250px;
}

.edit-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.form-error {
  color: var(--error-color);
  font-size: 0.9rem;
  margin-top: 0.5rem;
}

.profile-username {
  font-size: 2rem;
}

.profile-actions {
  margin-top: 1rem;
  display: flex;
  gap: 1rem;
}
</style>
