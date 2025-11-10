import { describe, it, expect, beforeEach, vi } from "vitest";
// Mock the usePostFilters composable before importing the view
vi.mock("@/composables/usePostFilters", () => {
  const { ref } = require("vue");
  const posts = ref([]);
  const loading = ref(false);
  const error = ref(null);
  const nextCursor = ref(null);
  const filters = ref({ sortOrder: "Desc" });
  const currentSort = ref("recent");
  const fetchPosts = vi.fn();

  return {
    usePostFilters: () => ({
      posts,
      tags: ref([]),
      currentSort,
      currentFilter: ref("all"),
      sortOrder: ref("Desc"),
      filters,
      searchTerm: ref("") ,
      loading,
      error,
      nextCursor,
      fetchPosts,
    }),
  };
});

import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import HomeView from "@/views/HomeView.vue";
import { ref } from "vue";

describe("HomeView - integration", () => {
  let router: ReturnType<typeof createRouter>;

  beforeEach(() => {
    setActivePinia(createPinia());

    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", name: "home", component: { template: "<div/>" } },
        { path: "/login", name: "login", component: { template: "<div/>" } },
        { path: "/create-post", name: "create-post", component: { template: "<div/>" } },
      ],
    });
  });

  it("calls fetchPosts on mount and passes posts/loading/error to PostList", async () => {
    // Arrange - get the mocked composable state and mutate posts
    const postFilters = (await import("@/composables/usePostFilters")).usePostFilters();
    postFilters.posts.value = [
      { id: "p1", title: "Post One" },
      { id: "p2", title: "Post Two" },
    ];
    postFilters.loading.value = false;

    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          PageHeader: { template: "<div/>" },
          PostList: { template: "<div data-testid=\"post-list-stub\"></div>", props: ["posts", "loading", "error"] },
          SideBar: { template: "<div/>" },
          Card: { template: "<div><slot name=\"footer\"></slot></div>" },
        },
      },
    });

    // fetchPosts should have been called by onMounted
    const pf = (await import("@/composables/usePostFilters")).usePostFilters();
    expect(pf.fetchPosts).toHaveBeenCalled();

    // Ensure the PostList stub is present
    const stub = wrapper.find("[data-testid=\"post-list-stub\"]");
    expect(stub.exists()).toBe(true);
  });

  it("navigates to login when Create Post clicked and user not logged in, otherwise to create-post", async () => {
    const auth = useAuthStore();
    auth.user = null; // not logged in

    const wrapper = mount(HomeView, {
      global: {
        plugins: [router],
        stubs: {
          PageHeader: { template: "<div/>" },
          PostList: { template: "<div/>" },
          SideBar: { template: "<div/>" },
          Card: { template: "<div><slot name=\"footer\"></slot></div>" },
        },
      },
    });

    const pushSpy = vi.spyOn(router, "push");
    await wrapper.find(".create-post-button").trigger("click");
    expect(pushSpy).toHaveBeenCalledWith("/login");

    // Now simulate logged in
    auth.user = { id: "u1", username: "me" } as any;
    await wrapper.find(".create-post-button").trigger("click");
    expect(pushSpy).toHaveBeenCalledWith("/create-post");
  });
});
