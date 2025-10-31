<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import Form from "@/components/utils/Form.vue";
import InputText from "primevue/inputtext";

const router = useRouter();

// Form data
const formData = ref({
  username: '',
  password: ''
});

// Form state
const isSubmitting = ref(false);
const errors = ref<Record<string, string>>({});
const showPassword = ref(false);

const usernameTouched = ref(false);
const passwordTouched = ref(false);

// Computed properties
const isFormValid = computed(() => {
  return formData.value.username.trim() && 
         formData.value.password.trim();
});

// Computed properties for showing errors only after touch
const showUsernameError = computed(() => {
  return usernameTouched.value && errors.value.username;
});

const showPasswordError = computed(() => {
  return passwordTouched.value && errors.value.password;
});

// Form handlers
const handleSubmit = async (event: Event) => {
  event.preventDefault();
  
  // Mark all fields as touched on submit
  usernameTouched.value = true;
  passwordTouched.value = true;
  
  if (!isFormValid.value) {
    validateForm();
    return;
  }

  isSubmitting.value = true;
  errors.value = {};

  try {
    // Simulate API call for login
    const loginData = {
      username: formData.value.username.trim(),
      password: formData.value.password
    };

    // Here you would make the actual API call
    console.log('Logging in with:', loginData);

    // Simulate success - redirect to home
    router.push('/');
  } catch (error) {
    console.error('Login error:', error);
    errors.value.submit = 'Invalid credentials. Please try again.';
  } finally {
    isSubmitting.value = false;
  }
};

const handleReset = () => {
  formData.value = {
    username: '',
    password: ''
  };
  errors.value = {};
  usernameTouched.value = false;
  passwordTouched.value = false;
};

const validateForm = () => {
  errors.value = {};

  // Username validation
  if (!formData.value.username.trim()) {
    errors.value.username = 'Username is required';
  }

  // Password validation
  if (!formData.value.password.trim()) {
    errors.value.password = 'Password is required';
  } else if (formData.value.password.length < 6) {
    errors.value.password = 'Password must be at least 6 characters';
  }
};

const navigateToRegister = () => {
  router.push('/register');
};

const navigateToForgotPassword = () => {
//   router.push('/forgot-password');
};

const togglePasswordVisibility = () => {
  showPassword.value = !showPassword.value;
};

const handleCancel = () => {
  router.push('/');
};
</script>

<template>
  <div class="login-page">
    <div class="login-container">
      <Form 
        variant="card" 
        width="narrow"
        title="Welcome Back"
        subtitle="Sign in to your OurCity account"
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
            type="text"
            class="form-input"
            placeholder="Enter your username"
            :class="{ 'p-invalid': showUsernameError }"
            autocomplete="username"
            @blur="usernameTouched = true; validateForm();"
          />
          <div v-if="showUsernameError" class="form-error">{{ errors.username }}</div>
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
              :class="{ 'invalid': showPasswordError }"
              placeholder="Enter your password"
              autocomplete="current-password"
              @blur="passwordTouched = true; validateForm();"
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

        <!-- Additional Options -->
        <template #between>
          <div class="login-options">
            <button 
              type="button" 
              class="link-button"
              @click="navigateToForgotPassword"
            >
              Forgot your password?
            </button>
          </div>
        </template>

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
            class="form-button form-button--primary login-button"
            :disabled="loading || !isFormValid"
          >
            {{ loading ? 'Signing In...' : 'Sign In' }}
          </button>
        </template>

        <!-- Footer -->
        <template #footer>
          <div class="login-footer">
            <p>
              Don't have an account? 
              <button 
                type="button" 
                class="link-button"
                @click="navigateToRegister"
              >
                Sign up here
              </button>
            </p>
          </div>
        </template>
      </Form>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--neutral-color) 0%, var(--secondary-background-color) 100%);
  padding: 2rem;
}

.login-container {
  width: 100%;
  max-width: 400px;
}

.login-options {
  display: flex;
  justify-content: flex-end;
  margin-top: 0.5rem;
}

.login-footer {
  color: var(--primary-text-color);
  text-align: center;
}

.link-button {
  background: none;
  border: none;
  color: var(--link-color);
  cursor: pointer;
  text-decoration: underline;
  font-size: inherit;
  padding: 0;
  margin: 0;
  transition: color 0.2s ease;
}

.link-button:hover {
  color: var(--link-color-hover);
}

.login-button {
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
  color: var(--tertiary-text-color);
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 0.25rem;
  transition: color 0.2s ease, background-color 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.password-toggle:hover {
  color: var(--primary-text-color);
  background: var(--primary-background-color-hover);
}

.password-toggle:focus {
  outline: 2px solid var(--neutral-color);
  outline-offset: 2px;
}

.password-input.invalid {
  border-color: var(--error-color);
}

/* Remove PrimeVue Password component styles */
:deep(.p-inputtext.p-invalid) {
  border-color: var(--error-color);
}

/* Responsive design */
@media (max-width: 768px) {
  .login-page {
    padding: 1rem;
  }
  
  .login-container {
    max-width: 100%;
  }
}

</style>