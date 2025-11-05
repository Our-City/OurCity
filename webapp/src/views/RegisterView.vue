<!-- Generative AI was used to assist in the creation of this file.
  ChatGPT was asked to generate code to help integrate the User service layer API calls.
  Also assisted with integrating the Pinia authentication store.-->
<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/authenticationStore";
import { createUser } from "@/api/userService";
import Form from "@/components/utils/FormCmp.vue";
import InputText from "primevue/inputtext";

const router = useRouter();

// Form data
const formData = ref({
  username: "",
  password: "",
  confirmPassword: "",
});

// Form state
const isSubmitting = ref(false);
const errors = ref<Record<string, string>>({});
const showPassword = ref(false);
const showConfirmPassword = ref(false);

const usernameTouched = ref(false);
const passwordTouched = ref(false);
const confirmPasswordTouched = ref(false);

// Computed properties
const isFormValid = computed(() => {
  return (
    formData.value.username.trim() &&
    formData.value.password.trim() &&
    formData.value.confirmPassword.trim()
  );
});

// Computed properties for showing errors only after touch
const showUsernameError = computed(() => {
  return usernameTouched.value && errors.value.username;
});

const showPasswordError = computed(() => {
  return passwordTouched.value && errors.value.password;
});

const showConfirmPasswordError = computed(() => {
  return confirmPasswordTouched.value && errors.value.confirmPassword;
});

// form handlers
const handleSubmit = async (event: Event) => {
  event.preventDefault();
  usernameTouched.value = true;
  passwordTouched.value = true;
  confirmPasswordTouched.value = true;

  if (!isFormValid.value) {
    validateForm();
    return;
  }

  isSubmitting.value = true;
  errors.value = {};

  try {
    // request user creation
    const username = formData.value.username.trim();
    const password = formData.value.password;

    const newUser = await createUser(username, password);
    console.log("User registered successfully:", newUser);

    // auto login after registration
    try {
      const auth = useAuthStore();
      await auth.loginUser(username, password);
    } catch (e) {
      console.warn("Auto-login failed:", e);
    }

    // re-route to home after successful registration
    router.push("/");
  } catch (error: any) {
    console.error("Registration error:", error);
    errors.value.submit = error?.response?.data?.error || error.message || "Registration failed.";
  } finally {
    isSubmitting.value = false;
  }
};

const handleReset = () => {
  formData.value = {
    username: "",
    password: "",
    confirmPassword: "",
  };
  errors.value = {};
  usernameTouched.value = false;
  passwordTouched.value = false;
  confirmPasswordTouched.value = false;
};

const validateForm = () => {
  errors.value = {};

  // Username validation
  if (!formData.value.username.trim()) {
    errors.value.username = "Username is required";
  } else if (formData.value.username.trim().length < 3) {
    errors.value.username = "Username must be at least 3 characters";
  } else if (formData.value.username.trim().length > 20) {
    errors.value.username = "Username must be no more than 20 characters";
  } else if (!/^[a-zA-Z0-9_]+$/.test(formData.value.username.trim())) {
    errors.value.username = "Username can only contain letters, numbers, and underscores";
  }

  // Password validation
  if (!formData.value.password) {
    errors.value.password = "Password is required";
  } else if (formData.value.password.length < 6) {
    errors.value.password = "Password must be at least 6 characters";
  }

  // Confirm Password validation
  if (!formData.value.confirmPassword) {
    errors.value.confirmPassword = "Please confirm your password";
  } else if (formData.value.password !== formData.value.confirmPassword) {
    errors.value.confirmPassword = "Passwords do not match";
  }
};

const togglePasswordVisibility = () => {
  showPassword.value = !showPassword.value;
};

const toggleConfirmPasswordVisibility = () => {
  showConfirmPassword.value = !showConfirmPassword.value;
};

const handleCancel = () => {
  router.push("/");
};
</script>

<template>
  <div class="register-page">
    <div class="register-container">
      <Form
        variant="card"
        width="narrow"
        title="Sign Up"
        subtitle="Create your OurCity account"
        :loading="isSubmitting"
        @submit="handleSubmit"
        @reset="handleReset"
      >
        <!-- Error display -->
        <div v-if="errors.submit" class="form-section form-section--before">
          <div class="form-error">{{ errors.submit }}</div>
        </div>

        <!-- Username Field -->
        <div class="form-field">
          <label class="form-label form-label--required" for="username">Username</label>
          <InputText
            id="username"
            v-model="formData.username"
            class="form-input"
            placeholder="Enter your username"
            :class="{ 'p-invalid': showUsernameError }"
            autocomplete="username"
            maxlength="20"
            @blur="
              usernameTouched = true;
              validateForm();
            "
          />
          <div v-if="showUsernameError" class="form-error">{{ errors.username }}</div>
          <div class="form-help">{{ formData.username.length }}/20 characters</div>
        </div>

        <!-- Password Field -->
        <div class="form-field">
          <label class="form-label form-label--required" for="password">Password</label>
          <div class="password-input-container">
            <input
              id="password"
              v-model="formData.password"
              :type="showPassword ? 'text' : 'password'"
              class="form-input password-input"
              :class="{ invalid: showPasswordError }"
              placeholder="Enter your password"
              autocomplete="new-password"
              @blur="
                passwordTouched = true;
                validateForm();
              "
            />
            <button
              type="button"
              class="password-toggle"
              @click="togglePasswordVisibility"
              :aria-label="showPassword ? 'Hide password' : 'Show password'"
            >
              <i :class="showPassword ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
            </button>
          </div>
          <div v-if="showPasswordError" class="form-error">{{ errors.password }}</div>
        </div>

        <!-- Confirm Password Field -->
        <div class="form-field">
          <label class="form-label form-label--required" for="confirmPassword"
            >Confirm Password</label
          >
          <div class="password-input-container">
            <input
              id="confirmPassword"
              v-model="formData.confirmPassword"
              :type="showConfirmPassword ? 'text' : 'password'"
              class="form-input password-input"
              :class="{ invalid: showConfirmPasswordError }"
              placeholder="Confirm your password"
              autocomplete="new-password"
              @blur="
                confirmPasswordTouched = true;
                validateForm();
              "
            />
            <button
              type="button"
              class="password-toggle"
              @click="toggleConfirmPasswordVisibility"
              :aria-label="showConfirmPassword ? 'Hide password' : 'Show password'"
            >
              <i :class="showConfirmPassword ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
            </button>
          </div>
          <div v-if="showConfirmPasswordError" class="form-error">{{ errors.confirmPassword }}</div>
        </div>

        <!-- Form Actions -->
        <template #actions="{ loading }">
          <button
            type="button"
            class="form-button form-button--outline"
            @click="handleCancel"
            :disabled="loading"
          >
            Cancel
          </button>
          <button
            type="submit"
            class="form-button form-button--primary register-button"
            :disabled="loading || !isFormValid"
          >
            {{ loading ? "Signing Up..." : "Sign Up" }}
          </button>
        </template>
      </Form>
    </div>
  </div>
</template>

<style scoped>
.register-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(
    135deg,
    var(--neutral-color) 0%,
    var(--secondary-background-color) 100%
  );
  padding: 2rem;
}

.register-container {
  width: 100%;
  max-width: 400px;
}

.link-button {
  background: none;
  border: none;
  color: var(--primary-color, #007bff);
  cursor: pointer;
  text-decoration: underline;
  font-size: inherit;
  padding: 0;
  margin: 0;
  transition: color 0.2s ease;
}

.link-button:hover {
  color: var(--primary-color-hover, #0056b3);
}

.register-button {
  flex: 1;
  justify-content: center;
}

/* Custom password input styling */
.password-input-container {
  position: relative;
  display: flex;
  align-items: center;
}

.password-input {
  padding-right: 3rem !important;
  width: 100%;
}

.password-toggle {
  position: absolute;
  right: 0.75rem;
  background: none;
  border: none;
  color: var(--text-color, #666666);
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 0.25rem;
  transition:
    color 0.2s ease,
    background-color 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.password-toggle:hover {
  color: var(--primary-text-color, #333333);
  background: var(--primary-background-color-hover, rgba(0, 0, 0, 0.05));
}

.password-toggle:focus {
  outline: 2px solid var(--primary-color, #007bff);
  outline-offset: 2px;
}

.password-input.invalid {
  border-color: var(--error-color, #dc3545);
}

/* Remove PrimeVue Password component styles */
:deep(.p-inputtext.p-invalid) {
  border-color: var(--error-color, #dc3545);
}

/* Responsive design */
@media (max-width: 768px) {
  .register-page {
    padding: 1rem;
  }

  .register-container {
    max-width: 100%;
  }
}
</style>
