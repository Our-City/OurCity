<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import InputText from "primevue/inputtext";
import ToolBar from "primevue/toolbar";

const router = useRouter();
const searchQuery = ref("");

const loggedIn = ref(false);

function isLoggedIn(): boolean {
  return loggedIn.value;
}

function goToHome(): void {
  router.push("/");
}

function handleLogin(): void {
  // router.push("/login");
}

function handleSignUp(): void {
  // router.push("/signup");
}

function handleLogout(): void {
  // router.push("/logout");
}

function handleAccount(): void {
  // router.push("/account");
}

const items = ref([
  {
    label: "Log In",
    icon: "pi pi-sign-in",
    command: handleLogin,
  },
  {
    label: "Sign Up",
    icon: "pi pi-user-plus",
    command: handleSignUp,
  },
]);

</script>

<template>
  <div class="page-header">
    <ToolBar>
      <template #start>
        <h1 class="app-title" @click="goToHome">
          OurCity
        </h1>
      </template>
      <template #center>
        <div class="center-bar">
          <button class="home-button" @click="goToHome">Home</button>
          <div class="search-container">
            <span class="p-input-icon-left">
              <i class="pi pi-search" />
              <InputText
                v-model="searchQuery"
                placeholder="Search..."
                class="search-input"
              />
            </span>
          </div>
        </div>
      </template>
      <template #end>
        <button v-if="!isLoggedIn()" class="login-button" @click="handleLogin">Login</button>
        <button v-if="!isLoggedIn()" class="signup-button" @click="handleSignUp">Sign Up</button>
        <button v-if="isLoggedIn()" class="logout-button" @click="handleLogout">Logout</button>
      </template>
    </ToolBar>
  </div>
</template>

<style scoped>
.page-header {
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  padding: 0.5rem 0.5rem;
  width: 100%;
  height: 100%;
  border-top-left-radius: 1rem;
  border-top-right-radius: 1rem;
}

.app-title {
  height: 100%;
  display: flex;
  align-items: center;
  font-size: 2.5rem;
  padding: 0rem 1rem 0rem 3rem;
}

.app-title:hover {
  cursor: pointer;
}

.center-bar {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  min-width: 35rem;
  max-width: 50rem;
  gap: 0.5rem;
  z-index: 1;
  transform: translate(-20%, 0);
}

.home-button {
  align-items: center;
  justify-content: center;
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  font-size: 1.25rem;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.75rem;
  transition: background 0.2s, color 0.2s;
}

.home-button:hover {
  background: var(--primary-background-color-hover);
  cursor: pointer;
}

.search-container {
  display: flex;
  align-items: center;
  background: var(--secondary-background-color);
  height: 2rem;
  border-radius: 3rem;
  padding-left: 1rem;
  padding-right: 1rem;
  flex: 1;
  min-width: 0;
}

.p-input-icon-left {
  display: flex;
  align-items: center;
  flex: 1;
  min-width: 0;
}

.search-input {
  background: var(--primary-background-color-hover);
  padding-left: 1rem;
  padding-right: 0.5rem;
  width: 100%;
}

.login-button, .logout-button {
  align-items: center;
  justify-content: center;
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  font-size: 1.25rem;
  padding: 0.5rem 1rem;
  margin-right: 1rem;
  border: none;
  border-radius: 0.75rem;
  transition: background 0.2s, color 0.2s;
}

.login-button:hover, .logout-button:hover {
  background: var(--primary-background-color-hover);
  cursor: pointer;
}

.signup-button{
  align-items: center;
  justify-content: center;
  background: var(--neutral-color);
  color: var(--secondary-text-color);
  font-size: 1.25rem;
  padding: 0.5rem 1rem;
  margin-right: 1rem;
  border: none;
  border-radius: 0.75rem;
  transition: background 0.2s, color 0.2s;
}
.signup-button:hover {
  background: var(--neutral-color-hover);
  cursor: pointer;
}
</style>