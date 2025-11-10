import { describe, it, expect, beforeEach, vi } from "vitest";

// Mock API modules before importing the view
vi.mock("@/api/postService", () => ({
  getPostById: vi.fn(),
  voteOnPost: vi.fn(),
}));
vi.mock("@/api/mediaService", () => ({
  getMediaByPostId: vi.fn(),
}));
vi.mock("@/api/commentService", () => ({
  getCommentsByPostId: vi.fn(),
  createComment: vi.fn(),
  voteOnComment: vi.fn(),
}));

// Stub primevue Textarea module to avoid accessing PrimeVue global config during imports
vi.mock("primevue/textarea", () => ({
  default: {
    props: ["modelValue"],
    emits: ["update:modelValue"],
    template: `<textarea :value="modelValue" @input="$emit('update:modelValue', $event.target.value)"></textarea>`,
  },
}));

import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import { setActivePinia, createPinia } from "pinia";
import PostDetailView from "@/views/PostDetailView.vue";
import { getPostById } from "@/api/postService";
import { getMediaByPostId } from "@/api/mediaService";
import { getCommentsByPostId, createComment } from "@/api/commentService";
import { ref } from "vue";
import { useAuthStore } from "@/stores/authenticationStore";

describe("PostDetailView - integration (comments)", () => {
  let router: ReturnType<typeof createRouter>;

  beforeEach(() => {
    setActivePinia(createPinia());

    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", name: "home", component: { template: "<div/>" } },
        { path: "/login", name: "login", component: { template: "<div/>" } },
        { path: "/posts/:id", name: "post-detail", component: { template: "<div/>" } },
      ],
    });
  });

  it("loads post and comments and renders title and comment count", async () => {
    // Arrange mocks
    const fakePost = {
      id: "p1",
      title: "Test Post",
      authorName: "Alice",
      createdAt: new Date(),
      tags: [],
      description: "Hello",
      voteCount: 0,
      voteStatus: 0,
    };

    (getPostById as unknown as vi.Mock).mockResolvedValue(fakePost);
    (getMediaByPostId as unknown as vi.Mock).mockResolvedValue([]);
    (getCommentsByPostId as unknown as vi.Mock).mockResolvedValue({ items: [
      { id: "c1", content: "First", authorName: "Bob", createdAt: new Date(), voteCount: 0, voteStatus: 0 },
    ] });

    // set route and wait for router to be ready so route.params.id is available
    await router.push("/posts/p1");
    await router.isReady();

    const wrapper = mount(PostDetailView, {
      global: {
        plugins: [router],
        stubs: {
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
          ImageGalleria: { template: "<div/>" },
          VoteBox: { template: "<div/>" },
          // TextArea: provide a real textarea to allow v-model changes
          TextArea: {
            props: ["modelValue"],
            emits: ["update:modelValue"],
            template: `<textarea :value="modelValue" @input="$emit('update:modelValue', $event.target.value)"></textarea>`,
          },
          // also register a lowercase variant in case the template uses a different tag name
          Textarea: {
            props: ["modelValue"],
            emits: ["update:modelValue"],
            template: `<textarea :value="modelValue" @input="$emit('update:modelValue', $event.target.value)"></textarea>`,
          },
          // Simple CommentList that renders comments prop
          CommentList: {
            props: ["comments"],
            template: `<div><div v-for="c in comments" :key="c.id" class="comment-item">{{ c.content }}</div></div>`,
          },
        },
      },
    });

    // Wait next tick for onMounted to run
    await new Promise((r) => setTimeout(r, 0));

    expect(getPostById).toHaveBeenCalledWith("p1");
    expect(getCommentsByPostId).toHaveBeenCalledWith("p1");

    // Title rendered
    const title = wrapper.find("[data-testid=\"post-title\"]");
    expect(title.exists()).toBe(true);
    expect(title.text()).toContain("Test Post");

    // Comment header reflects one comment
    const header = wrapper.find(".comment-header");
    expect(header.text()).toContain("Comments (1)");
  });

  it("redirects to login when submitting a comment while not authenticated", async () => {
    (getPostById as unknown as vi.Mock).mockResolvedValue({ id: "p1", title: "x", authorName: "a", createdAt: new Date(), tags: [], description: "", voteCount: 0, voteStatus: 0 });
    (getMediaByPostId as unknown as vi.Mock).mockResolvedValue([]);
    (getCommentsByPostId as unknown as vi.Mock).mockResolvedValue({ items: [] });

  await router.push("/posts/p1");
  await router.isReady();

    const wrapper = mount(PostDetailView, {
      global: {
        plugins: [router],
        stubs: {
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
          ImageGalleria: { template: "<div/>" },
          VoteBox: { template: "<div/>" },
          TextArea: {
            props: ["modelValue"],
            emits: ["update:modelValue"],
            template: `<textarea :value="modelValue" @input="$emit('update:modelValue', $event.target.value)"></textarea>`,
          },
          CommentList: { props: ["comments"], template: `<div></div>` },
        },
      },
    });

    await new Promise((r) => setTimeout(r, 0));

    const auth = useAuthStore();
    auth.user = null; // ensure not logged in

    const pushSpy = vi.spyOn(router, "push");

    // set textarea value
    const textarea = wrapper.find("textarea");
    await textarea.setValue("New comment");

    // click submit button
    await wrapper.find(".comment-submit-button").trigger("click");

    expect(pushSpy).toHaveBeenCalledWith("/login");
    expect(createComment).not.toHaveBeenCalled();
  });

  it("submits a comment when authenticated and prepends it to the list", async () => {
    (getPostById as unknown as vi.Mock).mockResolvedValue({ id: "p1", title: "x", authorName: "a", createdAt: new Date(), tags: [], description: "", voteCount: 0, voteStatus: 0 });
    (getMediaByPostId as unknown as vi.Mock).mockResolvedValue([]);
    (getCommentsByPostId as unknown as vi.Mock).mockResolvedValue({ items: [ { id: "c1", content: "Existing", authorName: "B", createdAt: new Date(), voteCount:0, voteStatus:0 } ] });

    const created = { id: "c2", content: "Hello new", authorName: "Me", createdAt: new Date(), voteCount:0, voteStatus:0 };
    (createComment as unknown as vi.Mock).mockResolvedValue(created);

  await router.push("/posts/p1");
  await router.isReady();

    // stub CommentList to reflect comments prop
    const wrapper = mount(PostDetailView, {
      global: {
        plugins: [router],
        stubs: {
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
          ImageGalleria: { template: "<div/>" },
          VoteBox: { template: "<div/>" },
          TextArea: {
            props: ["modelValue"],
            emits: ["update:modelValue"],
            template: `<textarea :value="modelValue" @input="$emit('update:modelValue', $event.target.value)"></textarea>`,
          },
          CommentList: {
            props: ["comments"],
            template: `<div><div v-for="c in comments" :key="c.id" class="comment-item">{{ c.content }}</div></div>`,
          },
        },
      },
    });

    await new Promise((r) => setTimeout(r, 0));

    const auth = useAuthStore();
    auth.user = { id: "u1", username: "me" } as any;

    // set textarea and submit
    const textarea = wrapper.find("textarea");
    await textarea.setValue("Hello new");
    await wrapper.find(".comment-submit-button").trigger("click");

    // allow async
    await new Promise((r) => setTimeout(r, 0));

    expect(createComment).toHaveBeenCalledWith("p1", expect.objectContaining({ content: "Hello new" }));

    // Comment list should now include the new comment at the top
    const items = wrapper.findAll(".comment-item");
    expect(items[0].text()).toContain("Hello new");
    expect(items[1].text()).toContain("Existing");
  });
});
