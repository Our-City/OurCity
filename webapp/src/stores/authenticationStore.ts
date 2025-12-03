/*
  webapp/src/stores/authenticationStore.ts

  This file contains the Pinia store for managing user authentication state.
  This store acts as the single source of truth for user authentication status across the frontend application/components.
*/

import { defineStore } from "pinia";
import { ref, computed } from "vue";
import { login, logout, me } from "@/api/authenticationService";
import { canViewAdminDashboard } from "@/api/authorizationService";
import { getCurrentUser } from "@/api/userService";
import type { User } from "@/models/user";

export const useAuthStore = defineStore("auth", () => {
  const user = ref<User | null>(null);
  const loading = ref(false);
  const isAdmin = ref(false);
  const isAuthenticated = computed(() => !!user.value);

  async function loginUser(username: string, password: string) {
    loading.value = true;
    try {
      user.value = await login(username, password);
      await checkAdminStatus();
    } catch (error) {
      console.error("Login failed:", error);
      throw error;
    } finally {
      loading.value = false;
    }
  }

  async function restoreSession() {
    try {
      user.value = await me();
      await checkAdminStatus();
    } catch {
      user.value = null;
      isAdmin.value = false;
    }
  }

  async function checkAdminStatus() {
    try {
      isAdmin.value = await canViewAdminDashboard();
    } catch {
      isAdmin.value = false;
    }
  }

  async function fetchCurrentUser() {
    loading.value = true;
    try {
      user.value = await getCurrentUser();
      await checkAdminStatus();
    } catch (error) {
      console.error("Failed to fetch current user:", error);
      throw error;
    } finally {
      loading.value = false;
    }
  }

  async function logoutUser() {
    await logout();
    user.value = null;
    isAdmin.value = false;
  }

  return {
    user,
    isAuthenticated,
    isAdmin,
    loading,
    loginUser,
    restoreSession,
    fetchCurrentUser,
    logoutUser,
  };
});
