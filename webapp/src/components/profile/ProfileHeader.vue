<script setup lang="ts">
</script>

<template>
  <div class="profile-header">
    <span><i class="pi pi-user" /><h1 class="profile-username">Username</h1></span>

  </div>
</template>

<style scoped>
.profile-header {
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  padding: 1rem;
  border-bottom: 1px solid var(--border-color, #ccc);
}

.profile-username {
  font-size: 1.5rem;
  font-weight: bold;
}
</style>