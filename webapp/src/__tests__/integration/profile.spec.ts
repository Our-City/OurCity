/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";

// Mock API modules
vi.mock("@/api/postService", () => ({
  getPostById: vi.fn(),
}));

// Mock toast to avoid primevue global config access and allow assertions
const toastAdd = vi.fn();
vi.mock("primevue/usetoast", () => ({ useToast: () => ({ add: toastAdd }) }));

// Mock userService for ProfileHeader update flows
vi.mock("@/api/userService", () => ({
  updateCurrentUser: vi.fn(),
}));

import { mount } from "@vue/test-utils";
import { User } from "@/models/user";
import { createRouter, createMemoryHistory } from "vue-router";
import ProfileView from "@/views/ProfileView.vue";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import { getPostById } from "@/api/postService";
import ProfileHeader from "@/components/profile/ProfileHeader.vue";
import { updateCurrentUser } from "@/api/userService";

describe("ProfileView - integration", () => {
  let router: ReturnType<typeof createRouter>;

  beforeEach(() => {
    setActivePinia(createPinia());

    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", name: "home", component: { template: "<div/>" } },
        { path: "/create-post", name: "create-post", component: { template: "<div/>" } },
        { path: "/posts/:id", name: "post-detail", component: { template: "<div/>" } },
      ],
    });
  });

  afterEach(() => {
    vi.resetAllMocks();
  });

  it("loads posts for the current user and renders them", async () => {
    const auth = useAuthStore();

    // spy and mock fetchCurrentUser to set a user with posts
    vi.spyOn(auth, "fetchCurrentUser").mockImplementation(async () => {
      const u = {
        id: "u1",
        username: "me",
        posts: ["p1", "p2"],
        isAdmin: false,
        isBanned: false,
        createdAt: new Date(),
        updatedAt: new Date(),
      } as unknown as User;
      auth.user = u;
    });

    const fake1 = { id: "p1", title: "First", createdAt: new Date() };
    const fake2 = { id: "p2", title: "Second", createdAt: new Date() };
    (getPostById as unknown as vi.Mock).mockResolvedValueOnce(fake1).mockResolvedValueOnce(fake2);

    const wrapper = mount(ProfileView, {
      global: {
        plugins: [router],
        stubs: {
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
          // stub PostList to render a simple list so we can assert items
          PostList: {
            props: ["posts"],
            template: `<div><div v-for="p in posts" :key="p.id" class="profile-post-item">{{ p.title }}</div></div>`,
          },
          ProfileHeader: {
            template: `<div><button class="create-post-button">Create Post</button></div>`,
          },
          ProfileToolbar: { template: `<div/>` },
        },
      },
    });

    // allow onMounted to run
    await router.isReady();
    await new Promise((r) => setTimeout(r, 0));

    expect(getPostById).toHaveBeenCalledWith("p1");
    expect(getPostById).toHaveBeenCalledWith("p2");

    const items = wrapper.findAll(".profile-post-item");
    expect(items.length).toBe(2);
    expect(items[0].text()).toContain("First");
    expect(items[1].text()).toContain("Second");
  });

  it("shows no-posts message when user has no posts", async () => {
    const auth = useAuthStore();
    vi.spyOn(auth, "fetchCurrentUser").mockImplementation(async () => {
      const u = {
        id: "u2",
        username: "other",
        posts: [],
        isAdmin: false,
        isBanned: false,
        createdAt: new Date(),
        updatedAt: new Date(),
      } as unknown as User;
      auth.user = u;
    });

    const wrapper = mount(ProfileView, {
      global: {
        plugins: [router],
        stubs: {
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
          ProfileHeader: { template: `<div/>` },
          ProfileToolbar: { template: `<div/>` },
        },
      },
    });

    await router.isReady();
    await new Promise((r) => setTimeout(r, 0));

    const noPosts = wrapper.find(".no-posts-message");
    expect(noPosts.exists()).toBe(true);
    expect(noPosts.text()).toContain("You haven't created any posts yet.");
  });

  it("navigates to create-post when the profile header create button is clicked", async () => {
    const auth = useAuthStore();
    vi.spyOn(auth, "fetchCurrentUser").mockImplementation(async () => {
      const u = {
        id: "u3",
        username: "me",
        posts: [],
        isAdmin: false,
        isBanned: false,
        createdAt: new Date(),
        updatedAt: new Date(),
      } as unknown as User;
      auth.user = u;
    });

    // use the real ProfileHeader so the button exists; stub other heavy children
    const wrapper = mount(ProfileView, {
      global: {
        plugins: [router],
        stubs: {
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
          PostList: { template: `<div/>` },
          ProfileToolbar: { template: `<div/>` },
        },
      },
    });

    await router.isReady();
    await new Promise((r) => setTimeout(r, 0));

    const pushSpy = vi.spyOn(router, "push");

    const btn = wrapper.find(".create-post-button");
    expect(btn.exists()).toBe(true);
    await btn.trigger("click");

    expect(pushSpy).toHaveBeenCalledWith("/create-post");
  });

  // Additional ProfileHeader-focused integration tests
  describe("ProfileHeader - integration", () => {
    it("shows username and allows editing, save updates auth store and shows toast", async () => {
      const auth = useAuthStore();
      const u = {
        id: "u5",
        username: "oldname",
        isAdmin: false,
        isBanned: false,
        createdAt: new Date(),
        updatedAt: new Date(),
      } as unknown as User;
      auth.user = u;

      // mock successful update
      (updateCurrentUser as unknown as vi.Mock).mockResolvedValue({
        id: "u5",
        username: "newname",
      });

      const wrapper = mount(ProfileHeader, {
        props: { username: "oldname" },
        global: {
          plugins: [router],
        },
      });

      // username displayed
      expect(wrapper.find(".profile-username").text()).toContain("oldname");

      // click edit
      await wrapper.find(".edit-username-button").trigger("click");

      const input = wrapper.find(".username-input");
      expect(input.exists()).toBe(true);
      await input.setValue("newname");

      // click save
      await wrapper.find(".save-button").trigger("click");

      // wait async
      await new Promise((r) => setTimeout(r, 0));

      // auth store should be updated and toast called
      expect(auth.user?.username).toBe("newname");
      expect(toastAdd).toHaveBeenCalled();
    });

    it("validates username input and shows error messages", async () => {
      const auth = useAuthStore();
      const u = {
        id: "u6",
        username: "me",
        isAdmin: false,
        isBanned: false,
        createdAt: new Date(),
        updatedAt: new Date(),
      } as unknown as User;
      auth.user = u;

      const wrapper = mount(ProfileHeader, {
        props: { username: "me" },
        global: { plugins: [router] },
      });

      await wrapper.find(".edit-username-button").trigger("click");
      const input = wrapper.find(".username-input");

      // empty username
      await input.setValue("");
      await wrapper.find(".save-button").trigger("click");
      await new Promise((r) => setTimeout(r, 0));
      expect(wrapper.find(".form-error").text()).toContain("Username cannot be empty");

      // too short
      await input.setValue("aa");
      await wrapper.find(".save-button").trigger("click");
      await new Promise((r) => setTimeout(r, 0));
      expect(wrapper.find(".form-error").text()).toContain("between 3 and 20 characters");

      // invalid chars
      await input.setValue("bad name$");
      await wrapper.find(".save-button").trigger("click");
      await new Promise((r) => setTimeout(r, 0));
      expect(wrapper.find(".form-error").text()).toContain(
        "only contain letters, numbers, and underscores",
      );
    });

    it("cancel edit resets input and leaves username unchanged", async () => {
      const auth = useAuthStore();
      const u = {
        id: "u7",
        username: "keepme",
        isAdmin: false,
        isBanned: false,
        createdAt: new Date(),
        updatedAt: new Date(),
      } as unknown as User;
      auth.user = u;

      const wrapper = mount(ProfileHeader, {
        props: { username: "keepme" },
        global: { plugins: [router] },
      });

      await wrapper.find(".edit-username-button").trigger("click");
      const input = wrapper.find(".username-input");
      await input.setValue("changed");
      await wrapper.find(".cancel-button").trigger("click");

      expect(wrapper.find(".profile-username").text()).toContain("keepme");
    });

    it("create post button navigates to create-post", async () => {
      const auth = useAuthStore();
      const u = {
        id: "u8",
        username: "me",
        isAdmin: false,
        isBanned: false,
        createdAt: new Date(),
        updatedAt: new Date(),
      } as unknown as User;
      auth.user = u;

      const wrapper = mount(ProfileHeader, {
        props: { username: "me" },
        global: { plugins: [router] },
      });

      const pushSpy = vi.spyOn(router, "push");
      const btn = wrapper.find(".create-post-button");
      expect(btn.exists()).toBe(true);
      await btn.trigger("click");
      expect(pushSpy).toHaveBeenCalledWith("/create-post");
    });
  });
});
