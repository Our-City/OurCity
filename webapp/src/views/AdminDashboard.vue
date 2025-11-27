<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  -->
<script setup lang="ts">
import { onMounted, computed } from "vue";
import PageHeader from "@/components/PageHeader.vue";
import SideBar from "@/components/SideBar.vue";
import { useAuthStore } from "@/stores/authenticationStore";
import { CContainer, CCard, CCardBody, CCardHeader, CRow, CCol } from "@coreui/vue";
import PostActivity from "@/components/admin/PostActivity.vue";
import TagBreakdown from "@/components/admin/Charts/TagBreakdown.vue";

const auth = useAuthStore();

onMounted(() => {
  // Any admin-specific data fetching can be done here
});

const isLoggedIn = computed(() => auth.isAuthenticated);
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
        <div class="admin-page-content-layout">
          <h2>Admin Dashboard</h2>
          <br/>
          <CContainer fluid>
            <CRow>
              <CCol>
                <PostActivity/>
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
