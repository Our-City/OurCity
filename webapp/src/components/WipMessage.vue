<script setup lang="ts">
import { defineProps } from "vue";

const props = defineProps({
  message: String,
  description: String,
});
</script>

<template>
  <div id="wip-msg-container">
    <h1 id="wip-msg">{{ props.message }}</h1>
    <h5 id="wip-description">{{ props.description }}</h5>
  </div>
</template>

<style>
#wip-msg-container {
  margin-top: 5rem;
  margin-bottom: 5rem;
  padding: 2rem;
  background-color: cornflowerblue;
}

#wip-msg {
  margin-bottom: 2rem;
}
</style>
