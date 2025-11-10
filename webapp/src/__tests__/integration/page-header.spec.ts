import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { ref } from "vue";

// Mock the usePostFilters composable before importing the component
vi.mock("@/composables/usePostFilters", () => {
  const searchTerm = ref("");
  const reset = vi.fn();
  const fetchPosts = vi.fn();

  return {
    usePostFilters: () => ({
      searchTerm,
      reset,
      fetchPosts,
    }),
  };
});

// Stub primevue input to avoid plugin config access at import time
vi.mock("primevue/inputtext", () => ({
  default: {
    props: ["modelValue"],
    emits: ["update:modelValue"],
    template: `<input :value="modelValue" @input="$emit('update:modelValue', $event.target.value)" />`,
  },
}));

import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import PageHeader from "@/components/PageHeader.vue";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import { User } from "@/models/user";

describe("PageHeader - integration", () => {
  let router: ReturnType<typeof createRouter>;

  beforeEach(() => {
    setActivePinia(createPinia());

    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", name: "home", component: { template: "<div/>" } },
        { path: "/login", name: "login", component: { template: "<div/>" } },
        { path: "/register", name: "register", component: { template: "<div/>" } },
        { path: "/create-post", name: "create-post", component: { template: "<div/>" } },
        { path: "/profile", name: "profile", component: { template: "<div/>" } },
      ],
    });
  });

  afterEach(() => {
    vi.resetAllMocks();
  });

  it("shows login/signup when not authenticated and navigates on click", async () => {
    const auth = useAuthStore();
    auth.user = null;

    const wrapper = mount(PageHeader, {
      global: {
        plugins: [router],
        stubs: {
          Toolbar: {
            template: `<div><slot name="start"/><slot name="center"/><slot name="end"/></div>`,
          },
        },
      },
    });

    const login = wrapper.find(".login-button");
    const signup = wrapper.find(".signup-button");
    expect(login.exists()).toBe(true);
    expect(signup.exists()).toBe(true);

    const pushSpy = vi.spyOn(router, "push");
    await login.trigger("click");
    expect(pushSpy).toHaveBeenCalledWith("/login");

    await signup.trigger("click");
    expect(pushSpy).toHaveBeenCalledWith("/register");
  });

  it("updates shared searchTerm when typing in the search input", async () => {
    const postFilters = (await import("@/composables/usePostFilters")).usePostFilters();

    const wrapper = mount(PageHeader, {
      global: {
        plugins: [router],
        stubs: {
          Toolbar: {
            template: `<div><slot name="start"/><slot name="center"/><slot name="end"/></div>`,
          },
        },
      },
    });

    const input = wrapper.find(".search-input");
    expect(input.exists()).toBe(true);
    await input.setValue("hello city");

    expect(postFilters.searchTerm.value).toBe("hello city");
  });

  it("clicking title resets filters and navigates home", async () => {
    const postFilters = (await import("@/composables/usePostFilters")).usePostFilters();
    const resetSpy = postFilters.reset;

    const wrapper = mount(PageHeader, {
      global: {
        plugins: [router],
        stubs: {
          Toolbar: { template: `<div><slot name="start"/></div>` },
        },
      },
    });

    const pushSpy = vi.spyOn(router, "push");
    await wrapper.find(".app-title").trigger("click");
    // wait for the async handler to finish (router.push then reset)
    await new Promise((r) => setTimeout(r, 0));

    expect(pushSpy).toHaveBeenCalledWith("/");
    expect(resetSpy).toHaveBeenCalled();
  });

  it("shows create post and account dropdown when authenticated and supports profile/logout actions", async () => {
  const auth = useAuthStore();
  // simulate logged in
  const u = { id: "u1", username: "me", isAdmin: false, isBanned: false, createdAt: new Date(), updatedAt: new Date() } as unknown as User;
  auth.user = u;
    // spy on logoutUser
    auth.logoutUser = vi.fn(async () => {});

    const postFilters = (await import("@/composables/usePostFilters")).usePostFilters();
    const fetchSpy = postFilters.fetchPosts;

    // Stub Dropdown to render dropdown slot content directly so clicks invoke handlers
    const DropdownStub = {
      props: ["buttonClass"],
      template: `<div><slot name="button"/><div class="dropdown-content"><slot name="dropdown" :close="() => {}"/></div></div>`,
    };

    const wrapper = mount(PageHeader, {
      global: {
        plugins: [router],
        stubs: {
          Toolbar: {
            template: `<div><slot name="start"/><slot name="center"/><slot name="end"/></div>`,
          },
          Dropdown: DropdownStub,
        },
      },
    });

    const createBtn = wrapper.find(".create-post-button");
    expect(createBtn.exists()).toBe(true);

    const pushSpy = vi.spyOn(router, "push");
    await createBtn.trigger("click");
    expect(pushSpy).toHaveBeenCalledWith("/create-post");

    // Click view profile list item inside dropdown
    const view = wrapper.find("li");
    expect(view.exists()).toBe(true);
    // first li is View Profile (per template)
    await view.trigger("click");
    await new Promise((r) => setTimeout(r, 0));
    expect(pushSpy).toHaveBeenCalledWith("/profile");

    // find the logout li - it's the second li
    const items = wrapper.findAll("li");
    const logout = items[1];
    await logout.trigger("click");
    // wait for async logout -> router.push -> fetchPosts
    await new Promise((r) => setTimeout(r, 0));

    expect(auth.logoutUser).toHaveBeenCalled();
    // logout should navigate to home and refresh posts
    expect(pushSpy).toHaveBeenCalledWith("/");
    expect(fetchSpy).toHaveBeenCalled();
  });
});
