<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import InputText from "primevue/inputtext";
import Dropdown from "./utils/Dropdown.vue";
import Toolbar from "./utils/Toolbar.vue";

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
  loggedIn.value = true;
}

function handleSignUp(): void {
  // router.push("/signup");
}

function handleLogout(): void {
  // router.push("/logout");
  loggedIn.value = false;
}

function handleViewProfile(): void {
  router.push("/profile");
}

function toggleAccountMenu(event: Event): void {
  isDropdownVisible.value = !isDropdownVisible.value;
}
</script>

<template>
  <Toolbar variant="header">
    <template #start>
      <h1 class="app-title" @click="goToHome">
        OurCity
      </h1>
      <button class="home-button" @click="goToHome">Home</button>
    </template>
    
    <template #center>
      <div class="search-container w-full">
        <span class="p-input-icon-left">
          <i class="pi pi-search" />
          <InputText
            v-model="searchQuery"
            placeholder="Search..."
            class="search-input"
          />
        </span>
      </div>
    </template>
    
    <template #end>
      <button v-if="!isLoggedIn()" class="login-button" @click="handleLogin">Login</button>
      <button v-if="!isLoggedIn()" class="signup-button" @click="handleSignUp">Sign Up</button>
      <Dropdown 
        v-if="isLoggedIn()"
        button-class="account-button"
      >
        <template #button>
          <i class="pi pi-user" />
          <i class="pi pi-angle-down" />
        </template>
        <template #dropdown="{ close }">
          <ul>
            <li @click="handleViewProfile(); close()">
              <i class="pi pi-user"></i> View Profile
            </li>
            <li @click="handleLogout(); close()">
              <i class="pi pi-sign-out"></i> Log Out
            </li>
          </ul>
        </template>
      </Dropdown>
    </template>
  </Toolbar>
</template>

<style scoped>
.app-title {
  height: 100%;
  display: flex;
  align-items: center;
  font-size: 2.5rem;
  padding: 0rem 1rem 0rem 3rem;
  cursor: pointer;
}

.search-container {
  display: flex;
  align-items: center;
  background: var(--secondary-background-color);
  height: 2rem;
  max-width: 500px;
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

.account-button {
  display: flex;
  gap: 0.25rem;
  align-items: center;
}

.signup-button {
  background: var(--neutral-color);
  color: var(--secondary-text-color);
}

.signup-button:hover {
  background: var(--neutral-color-hover);
}
</style>