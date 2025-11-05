<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  It also assisted with error handling.-->
<script setup lang="ts">
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
