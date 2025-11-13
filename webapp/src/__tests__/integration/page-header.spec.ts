/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { ref } from "vue";
import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import PageHeader from "@/components/PageHeader.vue";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import { User } from "@/models/user";

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

vi.mock("primevue/inputtext", () => ({
  default: {
    props: ["modelValue"],
    emits: ["update:modelValue"],
    template: `<input :value="modelValue" @input="$emit('update:modelValue', $event.target.value)" />`,
  },
}));

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
    await new Promise((r) => setTimeout(r, 0));

    expect(pushSpy).toHaveBeenCalledWith("/");
    expect(resetSpy).toHaveBeenCalled();
  });

  it("shows create post and account dropdown when authenticated and supports profile/logout actions", async () => {
    const auth = useAuthStore();
    const u = {
      id: "u1",
      username: "me",
      isAdmin: false,
      isBanned: false,
      createdAt: new Date(),
      updatedAt: new Date(),
    } as unknown as User;
    auth.user = u;
    auth.logoutUser = vi.fn(async () => {});

    const postFilters = (await import("@/composables/usePostFilters")).usePostFilters();
    const fetchSpy = postFilters.fetchPosts;

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

    const view = wrapper.find("li");
    expect(view.exists()).toBe(true);
    await view.trigger("click");
    await new Promise((r) => setTimeout(r, 0));
    expect(pushSpy).toHaveBeenCalledWith("/profile");

    const items = wrapper.findAll("li");
    const logout = items[1];
    await logout.trigger("click");
    await new Promise((r) => setTimeout(r, 0));

    expect(auth.logoutUser).toHaveBeenCalled();
    expect(pushSpy).toHaveBeenCalledWith("/");
    expect(fetchSpy).toHaveBeenCalled();
  });
});
