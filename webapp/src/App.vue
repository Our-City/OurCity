<!-- Where the JS/TS goes -->
<script setup lang="ts"></script>

<!-- Where the HTML goes -->
<template>
  <ToastCmp />
  <main>
    <router-view />
  </main>
</template>

<style>
* {
  margin: 0;
  box-sizing: border-box;
}

body {
  background: var(--secondary-background-color);
  min-height: 100vh;
}

main {
  margin: 0;
  padding: 0;
}
</style>

<!-- Where styling goes -->
<style>
/* Global styles - not scoped so they apply to the entire app */
* {
  font-family:
    "Inter",
    -apple-system,
    BlinkMacSystemFont,
    "Segoe UI",
    "Roboto",
    "Helvetica Neue",
    Arial,
    sans-serif;
}

/* Fallback for browsers that don't support variable fonts */
@supports not (font-variation-settings: normal) {
  * {
    font-family:
      -apple-system, BlinkMacSystemFont, "Segoe UI", "Roboto", "Helvetica Neue", Arial, sans-serif;
  }
}
</style>

<style scoped>
/* note: scoped means the styles only affect THIS component */
</style>
