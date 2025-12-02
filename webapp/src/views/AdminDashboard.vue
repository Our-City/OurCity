<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  -->
<script setup lang="ts">
import { onMounted, ref } from "vue";
import PageHeader from "@/components/PageHeader.vue";
import SideBar from "@/components/SideBar.vue";
import { CContainer, CRow, CCol } from "@coreui/vue";
import PostActivity from "@/components/admin/PostActivity.vue";
import { canViewAdminDashboard } from "@/api/authorizationService";

const isAuthorized = ref<boolean | null>(null);
const isLoading = ref(true);

onMounted(async () => {
  try {
    isAuthorized.value = await canViewAdminDashboard();
  } catch (error) {
    console.error("Failed to check admin authorization:", error);
    isAuthorized.value = false;
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <div class="admin-page">
    <div class="page-header">
      <PageHeader />
    </div>

    <div class="admin-page-layout">
      <div class="side-bar">
        <SideBar view="home" />
      </div>

      <div class="admin-page-body">
        <div v-if="isLoading" class="loading-container">
          <div class="spinner"></div>
        </div>

        <div v-else-if="!isAuthorized" class="unauthorized-container">
          <div class="unauthorized-message">
            <i class="pi pi-lock" style="font-size: 3rem; margin-bottom: 1rem"></i>
            <h2>Access Denied</h2>
            <p>You do not have permission to view the admin dashboard.</p>
          </div>
        </div>

        <div v-else class="admin-page-content-layout">
          <h2>Admin Dashboard</h2>
          <br />
          <CContainer fluid>
            <CRow>
              <CCol>
                <PostActivity />
              </CCol>
            </CRow>
          </CContainer>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.admin-page {
  padding: 1rem;
  height: 100vh;
  overflow: hidden;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}

.admin-page-layout {
  display: flex;
  height: calc(100vh - 2rem);
  overflow: hidden;
}

.admin-page-body {
  flex: 1;
  background: var(--primary-background-color);
  min-width: 0;
  overflow-y: auto;
  overflow-x: hidden;
}

.admin-page-content-layout {
  gap: 1rem;
  margin: 1rem 1.5rem 1rem 1rem;
  min-width: 0;
  overflow: visible;
}

.spinner {
  margin-top: 1rem;
  width: 4rem;
  height: 4rem;
  border: 0.25rem solid #ccc;
  border-top: 0.25rem solid #1976d2;
  border-radius: 100%;
  animation: spin 1.5s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.loading-container,
.unauthorized-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  width: 100%;
}

.unauthorized-message {
  text-align: center;
  padding: 2rem;
  color: var(--tertiary-text-color);
}

.unauthorized-message h2 {
  margin-bottom: 0.5rem;
  color: var(--primary-text-color);
}

.unauthorized-message p {
  font-size: 1.1rem;
}

/* General card styling for all admin dashboard cards */
:deep(.card) {
  margin-bottom: 1rem;
  border: none;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  border-radius: 0.5rem;
}

:deep(.card-body) {
  padding: 1.5rem;
}

:deep(.card-header) {
  padding: 1rem 1.5rem;
  font-weight: 600;
}
</style>
