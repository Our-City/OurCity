import { createRouter, createWebHistory } from "vue-router";

import HomeView from "@/views/HomeView.vue";
import PageDetailView from "@/views/PageDetailView.vue";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "home", component: HomeView },
    { path: "/posts/:id", name: "post-detail", component: PageDetailView },
  ],
});

export default router;
