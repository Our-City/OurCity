<!-- Generative AI was used to assist in the creation of this file.
  ChatGPT was asked to generate code to help integrate the Pinia authenticationStore
  for global authentication in the PageHeader.-->
<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import InputText from "primevue/inputtext";
import Dropdown from "./utils/DropdownMenu.vue";
import Toolbar from "./utils/ToolbarCmp.vue";
import { usePostFilters } from "@/composables/usePostFilters";
import { useAuthStore } from "@/stores/authenticationStore";

const router = useRouter();
const { reset, searchTerm, fetchPosts } = usePostFilters(); // <-- shared searchTerm
const auth = useAuthStore();

async function goToHome(): Promise<void> {
  await router.push("/");
  reset();
}

function goToAdminDashboard(): void {
  router.push("/admin");
}

function handleLogin(): void {
  router.push("/login");
}

function handleSignUp(): void {
  router.push("/register");
}

async function handleLogout(): Promise<void> {
  await auth.logoutUser();
  await router.push("/");
  await fetchPosts(); // Refresh posts after logout
}

function handleViewProfile(): void {
  router.push("/profile");
}

function handleCreatePost(): void {
  router.push("/create-post");
}

const isLoggedIn = computed(() => auth.isAuthenticated);
const isAdmin = computed(() => auth.isAdmin);
console.log("User", auth.user)
</script>

<template>
  <div class="page-header">
    <Toolbar variant="header">
      <template #start>
        <h1 class="app-title" @click="goToHome">OurCity</h1>
      </template>

      <template #center>
        <div class="search-container w-full">
          <span class="p-input-icon-left">
            <i class="pi pi-search" />
            <InputText v-model="searchTerm" placeholder="Search posts..." class="search-input" />
          </span>
        </div>
      </template>

      <template #end>
        <button v-if="!isLoggedIn" class="login-button" @click="handleLogin">Login</button>
        <button v-if="!isLoggedIn" class="signup-button" @click="handleSignUp">Sign Up</button>

        <button v-if="isLoggedIn && isAdmin" class="admin-button" @click="goToAdminDashboard">
          <i class="pi pi-objects-column"></i>
          Admin
        </button>

        <button v-if="isLoggedIn" class="create-post-button" @click="handleCreatePost">
          <i class="pi pi-plus"></i>
          Create Post
        </button>

        <Dropdown v-if="isLoggedIn" button-class="account-button">
          <template #button>
            <i class="pi pi-user" />
            <i class="pi pi-angle-down" />
          </template>

          <template #dropdown="{ close }">
            <ul>
              <li
                @click="
                  handleViewProfile();
                  close();
                "
              >
                <i class="pi pi-user"></i> View Profile
              </li>
              <li
                @click="
                  handleLogout();
                  close();
                "
              >
                <i class="pi pi-sign-out"></i> Log Out
              </li>
            </ul>
          </template>
        </Dropdown>
      </template>
    </Toolbar>
  </div>
</template>

<style scoped>
.page-header {
  box-shadow: 0 4rem 6rem rgba(0, 0, 0, 0.075);
  position: relative;
  z-index: 10;
}

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
