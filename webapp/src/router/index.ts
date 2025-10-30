import { createRouter, createWebHistory } from "vue-router";

import HomeView from "@/views/HomeView.vue";
import PageDetailView from "@/views/PageDetailView.vue";
import ProfileView from "@/views/ProfileView.vue";
import CreatePostView from "@/views/CreatePostView.vue";
import LoginView from "@/views/LoginView.vue";
import RegisterView from "@/views/RegisterView.vue";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "home", component: HomeView },
    { path: "/login", name: "login", component: LoginView },
    { path: "/profile", name: "profile", component: ProfileView },
    { path: "/posts/:id", name: "post-detail", component: PageDetailView },
    { path: "/create-post", name: "create-post", component: CreatePostView },
    { path: "/register", name: "register", component: RegisterView },
  ],
});

export default router;
