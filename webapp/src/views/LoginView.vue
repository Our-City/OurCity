<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import Form from "@/components/utils/Form.vue";
import InputText from "primevue/inputtext";

const router = useRouter();

// Form data
const formData = ref({
  email: '',
  username: '',
  password: ''
});

// Form state
const isSubmitting = ref(false);
const errors = ref<Record<string, string>>({});
const showPassword = ref(false);

// Computed properties
const isFormValid = computed(() => {
  return formData.value.email.trim() && 
         formData.value.username.trim() && 
         formData.value.password.trim();
});

// Form handlers
const handleSubmit = async (event: Event) => {
  event.preventDefault();
  
  if (!isFormValid.value) {
    validateForm();
    return;
  }

  isSubmitting.value = true;
  errors.value = {};

  try {
    // Simulate API call for login
    const loginData = {
      email: formData.value.email.trim(),
      username: formData.value.username.trim(),
      password: formData.value.password
    };

    // Here you would make the actual API call
    console.log('Logging in with:', loginData);

    // Simulate delay
    await new Promise(resolve => setTimeout(resolve, 1000));

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
    email: '',
    username: '',
    password: ''
  };
  errors.value = {};
};

const validateForm = () => {
  errors.value = {};

  // Email validation
  if (!formData.value.email.trim()) {
    errors.value.email = 'Email is required';
  } else if (!isValidEmail(formData.value.email.trim())) {
    errors.value.email = 'Please enter a valid email address';
  }

  // Username validation
  if (!formData.value.username.trim()) {
    errors.value.username = 'Username is required';
  } else if (formData.value.username.trim().length < 3) {
    errors.value.username = 'Username must be at least 3 characters';
  } else if (formData.value.username.trim().length > 20) {
    errors.value.username = 'Username must be no more than 20 characters';
  } else if (!/^[a-zA-Z0-9_]+$/.test(formData.value.username.trim())) {
    errors.value.username = 'Username can only contain letters, numbers, and underscores';
  }

  // Password validation
  if (!formData.value.password.trim()) {
    errors.value.password = 'Password is required';
  } else if (formData.value.password.length < 6) {
    errors.value.password = 'Password must be at least 6 characters';
  }
};

const isValidEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
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

        <!-- Email Field -->
        <div class="form-field">
          <label class="form-label form-label--required" for="email">Email Address</label>
          <InputText
            id="email"
            v-model="formData.email"
            type="email"
            class="form-input"
            placeholder="Enter your email address"
            :class="{ 'p-invalid': errors.email }"
            autocomplete="email"
            required
          />
          <div v-if="errors.email" class="form-error">{{ errors.email }}</div>
        </div>

        <!-- Username Field -->
        <div class="form-field">
          <label class="form-label form-label--required" for="username">Username</label>
          <InputText
            id="username"
            v-model="formData.username"
            class="form-input"
            placeholder="Enter your username"
            :class="{ 'p-invalid': errors.username }"
            autocomplete="username"
            maxlength="20"
            required
          />
          <div v-if="errors.username" class="form-error">{{ errors.username }}</div>
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
              :class="{ 'invalid': errors.password }"
              placeholder="Enter your password"
              autocomplete="current-password"
              required
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
          <div v-if="errors.password" class="form-error">{{ errors.password }}</div>
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
  background: linear-gradient(135deg, var(--primary-background-color, #007bff) 0%, var(--secondary-background-color, #6c757d) 100%);
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
    color: var(--primary-color);
  text-align: center;
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

.login-button {
  width: 100%;
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
  transition: color 0.2s ease, background-color 0.2s ease;
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
  .login-page {
    padding: 1rem;
  }
  
  .login-container {
    max-width: 100%;
  }
}

</style>