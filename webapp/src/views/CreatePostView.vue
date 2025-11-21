<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  ChatGPT was asked to generate code to help integrate the service layer API calls.
  e.g., loading Tags using API, etc.
  Copilot assisted with error handling.-->
<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useRouter } from "vue-router";

import PageHeader from "@/components/PageHeader.vue";
import SideBar from "@/components/SideBar.vue";
import Form from "@/components/utils/FormCmp.vue";
import MultiSelect from "@/components/utils/MultiSelect.vue";
import MapPicker from "@/components/MapPicker.vue";
import LocationAutocomplete from "@/components/LocationAutocomplete.vue";
import { useToast } from "primevue/usetoast";

import InputText from "primevue/inputtext";
import Textarea from "primevue/textarea";

import { useAuthStore } from "@/stores/authenticationStore";

import { createPost } from "@/api/postService";
import { uploadMedia } from "@/api/mediaService";
import { getTags } from "@/api/tagService";
import { resolveErrorMessage } from "@/utils/error";

import type { Post } from "@/models/post";
import type { Tag } from "@/models/tag";
import { PostVisibility } from "@/types/enums";

const router = useRouter();
const auth = useAuthStore();
const toast = useToast();

// Form data
const formData = ref({
  title: "",
  location: "",
  description: "",
  tags: [] as Tag[],
  images: [] as File[],
  locationData: null as {
    latitude: number;
    longitude: number;
    name: string;
  } | null,
});

// Form state
const isSubmitting = ref(false);
const errors = ref<Record<string, string>>({});

const titleTouched = ref(false);
const locationTouched = ref(false);
const descriptionTouched = ref(false);

// Available tags for multiselect (mocked for now, ideally fetched from API once available)
const availableTags = ref<Tag[]>([]);
const isLoadingTags = ref(false);
const tagError = ref<string | null>(null);

const locationValidationError = ref<string | null>(null);

// Handle location errors from either component
function handleLocationError(error: string | null) {
  locationValidationError.value = error;
}

async function loadTags() {
  try {
    isLoadingTags.value = true;
    tagError.value = null;
    availableTags.value = await getTags();
  } catch (err) {
    console.error("Failed to load tags:", err);
    tagError.value = "Failed to load tags from server.";
  } finally {
    isLoadingTags.value = false;
  }
}

onMounted(loadTags);

// Computed properties
const isFormValid = computed(() => {
  return formData.value.title.trim() && formData.value.description.trim();
});

const imagePreviewUrls = computed(() => {
  return formData.value.images.map((file) => URL.createObjectURL(file));
});

// Computed properties for showing errors only after touch
const showTitleError = computed(() => {
  return titleTouched.value && errors.value.title;
});

const showLocationError = computed(() => {
  return locationTouched.value && errors.value.location;
});

const showDescriptionError = computed(() => {
  return descriptionTouched.value && errors.value.description;
});

// Form handlers
const handleSubmit = async (event: Event) => {
  event.preventDefault();

  titleTouched.value = true;
  locationTouched.value = true;
  descriptionTouched.value = true;
  validateForm();

  if (!isFormValid.value) {
    return;
  }

  isSubmitting.value = true;
  errors.value = {};

  try {
    if (!auth.user) {
      throw new Error("You must be logged in to create a post.");
    }

    // create post object
    const newPost: Post = {
      id: "", // backend will assign this
      authorId: auth.user.id,
      title: formData.value.title.trim(),
      location: formData.value.locationData?.name || formData.value.location.trim() || undefined,
      latitude: formData.value.locationData?.latitude,
      longitude: formData.value.locationData?.longitude,
      description: formData.value.description.trim(),
      tags: formData.value.tags,
      createdAt: new Date(),
      updatedAt: new Date(),
      upvoteCount: 0,
      downvoteCount: 0,
      voteCount: 0,
      visibility: PostVisibility.PUBLISHED,
      commentCount: 0,
      voteStatus: 0,
      isDeleted: false,
    };

    const createdPost = await createPost(newPost);

    // upload media if any images selected
    if (formData.value.images.length > 0) {
      const uploadPromises = formData.value.images.map((file) => uploadMedia(file, createdPost.id));
      await Promise.all(uploadPromises);
    }

    toast.add({
      severity: "secondary",
      summary: "Your post has been published successfully.",
      life: 4000,
    });

    // redirect to the newly created post's page
    router.push(`/posts/${createdPost.id}`);
  } catch (error: unknown) {
    console.error("Error creating post:", error);
    errors.value.submit = resolveErrorMessage(error, "Failed to create post. Please try again.");
  } finally {
    isSubmitting.value = false;
  }
};

const handleReset = () => {
  formData.value = {
    title: "",
    location: "",
    description: "",
    tags: [],
    images: [],
    locationData: null,
  };
  errors.value = {};
  titleTouched.value = false;
  locationTouched.value = false;
  descriptionTouched.value = false;
};

const validateForm = () => {
  errors.value = {};

  if (!formData.value.title.trim()) {
    errors.value.title = "Title is required";
  }

  if (!formData.value.description.trim()) {
    errors.value.description = "Description is required";
  } else if (formData.value.description.trim().length < 10) {
    errors.value.description = "Description must be at least 10 characters";
  }
};

const handleFileUpload = (event: Event) => {
  const target = event.target as HTMLInputElement;
  if (target.files) {
    const newFiles = Array.from(target.files);

    // Validate file types and sizes
    const validFiles = newFiles.filter((file) => {
      if (!file.type.startsWith("image/")) {
        return false;
      }
      if (file.size > 5 * 1024 * 1024) {
        // 5MB limit
        return false;
      }
      return true;
    });

    // Limit total images to 5
    const totalImages = formData.value.images.length + validFiles.length;
    if (totalImages > 5) {
      alert("You can only attach up to 5 images.");
      // Only add up to 5 images
      const allowedFiles = validFiles.slice(0, 5 - formData.value.images.length);
      formData.value.images = [...formData.value.images, ...allowedFiles];
    } else {
      formData.value.images = [...formData.value.images, ...validFiles];
    }

    // Clear the input
    target.value = "";
  }
};

const removeImage = (index: number) => {
  formData.value.images.splice(index, 1);
};

// Handle location selected from autocomplete
function handleLocationSelected(location: { name: string; latitude: number; longitude: number }) {
  // Clear error on successful selection
  locationValidationError.value = null;
  
  formData.value.locationData = {
    name: location.name,
    latitude: location.latitude,
    longitude: location.longitude,
  };
  
  formData.value.location = location.name;
}

// Handle manual changes to location text field
function handleLocationTextChange(value: string) {
  formData.value.location = value;
  
  if (!value.trim() && formData.value.locationData) {
    formData.value.locationData = null;
    // Clear error when field is cleared
    locationValidationError.value = null;
  }
}

watch(
  () => formData.value.locationData,
  (newLocationData) => {
    if (newLocationData?.name && newLocationData.name !== formData.value.location) {
      formData.value.location = newLocationData.name;
      // Clear error when valid location is set from map
      locationValidationError.value = null;
    } else if (!newLocationData) {
      // Clear error when location data is cleared
      locationValidationError.value = null;
    }
  },
);
</script>

<template>
  <div class="create-post">
    <div class="page-header">
      <PageHeader />
    </div>
    <div class="create-post-page-layout">
      <div class="side-bar">
        <SideBar view="home" />
      </div>
      <div class="create-post-page-body">
        <div class="create-post-container">
          <Form
            variant="card"
            width="wide"
            title="Create New Post"
            subtitle="Share what you'd like to see in your city"
            :loading="isSubmitting"
            @submit="handleSubmit"
            @reset="handleReset"
          >
            <!-- Error display -->
            <div v-if="errors.submit" class="form-section form-section--before">
              <div class="form-error">{{ errors.submit }}</div>
            </div>

            <!-- Title Field -->
            <div class="form-field">
              <label class="form-label form-label--required" for="title">Post Title</label>
              <InputText
                id="title"
                v-model="formData.title"
                class="form-input"
                placeholder="Enter a descriptive title for your post"
                :class="{ 'p-invalid': showTitleError }"
                maxlength="50"
                @blur="
                  titleTouched = true;
                  validateForm();
                "
              />
              <div v-if="showTitleError" class="form-error">{{ errors.title }}</div>
              <div class="form-help">{{ formData.title.length }}/50 characters</div>
            </div>

            <!-- Location Validation Error Banner - Enhanced -->
            <div v-if="locationValidationError" class="location-error-banner">
              <div class="error-icon-container">
                <i class="pi pi-exclamation-triangle"></i>
              </div>
              <div class="error-content">
                <h4 class="error-title">Location Outside Winnipeg</h4>
                <p class="error-message">{{ locationValidationError }}</p>
              </div>
            </div>

            <!-- Location Field -->
            <div class="form-field">
              <label class="form-label" for="location">Location</label>
              
              <LocationAutocomplete
                :model-value="formData.location"
                placeholder="e.g., Downtown Winnipeg, University of Manitoba"
                @update:model-value="handleLocationTextChange"
                @location-selected="handleLocationSelected"
                @location-error="handleLocationError"
              />
              
              <div class="form-help">
                Start typing to search for a location {{ formData.location.length }}/150 characters (optional)
              </div>
            </div>

            <div class="form-field">
              <label class="form-label">Select Location on Map (Optional)</label>
              <MapPicker 
                v-model="formData.locationData" 
                height="400px"
                @location-error="handleLocationError"
              />
              <div class="form-help">
                Click on the map to select a location for your post
              </div>
            </div>

            <!-- Description Field -->
            <div class="form-field">
              <label class="form-label form-label--required" for="description">Description</label>
              <Textarea
                id="description"
                v-model="formData.description"
                class="form-textarea"
                placeholder="Describe what's happening, share your thoughts, or ask a question..."
                :class="{ 'p-invalid': showDescriptionError }"
                rows="6"
                maxlength="500"
                @blur="
                  descriptionTouched = true;
                  validateForm();
                "
              />
              <div v-if="showDescriptionError" class="form-error">{{ errors.description }}</div>
              <div class="form-help">{{ formData.description.length }}/500 characters</div>
            </div>

            <!-- Tags Field -->
            <div class="form-field">
              <label class="form-label" for="tags">Tags</label>

              <div v-if="isLoadingTags" class="form-help">Loading tags...</div>
              <div v-else-if="tagError" class="form-error">{{ tagError }}</div>

              <MultiSelect
                v-else
                id="tags"
                v-model="formData.tags"
                :options="availableTags"
                optionLabel="name"
                placeholder="Select relevant tags"
                :max-selected="5"
                :searchable="true"
              />

              <div class="form-help">
                Choose tags that best describe your post (max 5, optional)
              </div>
            </div>

            <!-- Image Upload Field -->
            <div class="form-field">
              <label class="form-label" for="images">Images</label>
              <div class="form-file-upload">
                <input
                  id="images"
                  type="file"
                  class="form-file-input"
                  multiple
                  accept="image/*"
                  @change="handleFileUpload"
                />
                <div class="form-file-button">
                  <i class="pi pi-upload"></i>
                  Choose Images
                </div>
              </div>
              <div class="form-help">
                Upload images to illustrate your post (max 5MB each, optional)
              </div>

              <!-- Image previews -->
              <div v-if="formData.images.length > 0" class="form-file-preview">
                <div
                  v-for="(image, index) in formData.images"
                  :key="index"
                  class="form-file-preview-item"
                >
                  <img
                    :src="imagePreviewUrls[index]"
                    :alt="`Preview ${index + 1}`"
                    class="form-file-preview-image"
                  />
                  <button
                    type="button"
                    class="form-file-preview-remove"
                    @click="removeImage(index)"
                  >
                    ×
                  </button>
                </div>
              </div>
            </div>

            <!-- Form Actions -->
            <template #actions="{ loading }">
              <button
                type="button"
                class="form-button form-button--outline"
                @click="$router.push('/')"
                :disabled="loading"
              >
                Cancel
              </button>
              <button type="reset" class="form-button form-button--secondary" :disabled="loading">
                Clear Form
              </button>
              <button type="submit" class="form-button form-button--primary" :disabled="loading">
                {{ loading ? "Creating Post..." : "Create Post" }}
              </button>
            </template>

            <!-- Footer -->
            <template #footer>
              <p>
                Your post will be visible to all users in the community. Please follow community
                guidelines.
              </p>
            </template>
          </Form>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.create-post {
  padding: 1rem;
}

.create-post-page-layout {
  display: flex;
  height: 100vh;
  overflow: hidden;
}

.create-post-page-body {
  flex: 1;
  background: var(--primary-background-color);
  min-width: 0;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 2rem;
}

.create-post-container {
  max-width: 800px;
  margin: 0 auto;
}

/* Custom styles for PrimeVue input components */
:deep(.p-inputtext.p-invalid) {
  border-color: var(--error-color, #dc3545);
}

:deep(.p-inputtextarea.p-invalid) {
  border-color: var(--error-color, #dc3545);
}

:deep(textarea.p-invalid) {
  border-color: var(--error-color, #dc3545) !important;
}

.form-textarea.p-invalid {
  border-color: var(--error-color, #dc3545) !important;
}

/* Responsive design */
@media (max-width: 768px) {
  .create-post-page-body {
    padding: 1rem;
  }

  .create-post-page-layout {
    flex-direction: column;
  }

  .side-bar {
    order: 2;
  }

  .create-post-page-body {
    order: 1;
  }
}

.location-error-banner {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  padding: 1rem 1.25rem;
  margin-bottom: 1.5rem;
  background: linear-gradient(135deg, #fff9e6 0%, #fffbf0 100%);
  border: 2px solid #ffc107;
  border-left: 5px solid #ff9800;
  border-radius: 0.75rem;
  box-shadow: 0 2px 8px rgba(255, 152, 0, 0.15);
  animation: slideDown 0.3s ease-out;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.error-icon-container {
  flex-shrink: 0;
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #ff9800;
  border-radius: 50%;
  box-shadow: 0 2px 4px rgba(255, 152, 0, 0.3);
}

.error-icon-container i {
  font-size: 1.25rem;
  color: white;
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
}

.error-content {
  flex: 1;
}

.error-title {
  margin: 0 0 0.5rem 0;
  font-size: 1rem;
  font-weight: 600;
  color: #e65100;
}

.error-message {
  margin: 0;
  font-size: 0.9rem;
  line-height: 1.5;
  color: #856404;
}

</style>
