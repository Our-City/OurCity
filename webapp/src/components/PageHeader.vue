<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import InputText from "primevue/inputtext";
import ToolBar from "primevue/toolbar";
import SplitButton from "primevue/splitbutton";

const router = useRouter();
const searchQuery = ref("");

const account_dropdown = [
  {
      label: 'View Profile',
      icon: 'pi pi-user',
      command: () => {
          handleViewProfile();
      }
  },
  {
      label: 'Log Out',
      icon: 'pi pi-sign-out',
      command: () => {
          handleLogout();
      }
  }
];

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

</script>

<template>
  <div class="page-header">
    <ToolBar>
      <template #start>
        <h1 class="app-title" @click="goToHome">
          OurCity
        </h1>
        <button class="home-button" @click="goToHome">Home</button>
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
      <template #center>
      </template>
      <template #end>
        <button v-if="!isLoggedIn()" class="login-button" @click="handleLogin">Login</button>
        <button v-if="!isLoggedIn()" class="signup-button" @click="handleSignUp">Sign Up</button>
        <SplitButton
          v-if="isLoggedIn()"
          class="account-button"
          icon="pi pi-user"
          @click="handleViewProfile"
          :model="account_dropdown"
          :pt="{ menu: { class: 'account-menu' } }"
        />
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

.home-button {
  align-items: center;
  justify-content: center;
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  font-size: 1.25rem;
  padding: 1rem 0.5rem;
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
  margin-left: 5rem;
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

.login-button {
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

.login-button:hover {
  background: var(--primary-background-color-hover);
  cursor: pointer;
}

.account-button {
  margin-right: 1rem;
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

/* Dropdown menu styling */
.account-button :deep(.p-menu) {
  background: var(--primary-background-color);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.75rem;
  padding: 0.5rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
  min-width: 12rem;
}

/* Menu items */
.account-button :deep(.p-menuitem-link) {
  color: var(--primary-text-color);
  padding: 0.75rem 1rem;
  border-radius: 0.5rem;
  transition: background 0.2s, color 0.2s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.account-button :deep(.p-menuitem-link:hover) {
  background: var(--primary-background-color-hover);
  cursor: pointer;
}

/* Menu item icons */
.account-button :deep(.p-menuitem-icon) {
  color: var(--primary-text-color);
  font-size: 1rem;
}

/* Menu item text */
.account-button :deep(.p-menuitem-text) {
  color: var(--primary-text-color);
  font-size: 1rem;
}
</style>