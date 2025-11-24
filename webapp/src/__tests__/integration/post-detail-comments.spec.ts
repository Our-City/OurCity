/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.
import { describe, it, expect, beforeEach, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import { setActivePinia, createPinia } from "pinia";
import PostDetailView from "@/views/PostDetailView.vue";
import { getPostById } from "@/api/postService";
import { getMediaByPostId } from "@/api/mediaService";
import { getCommentsByPostId, createComment } from "@/api/commentService";
import { useAuthStore } from "@/stores/authenticationStore";
import { User } from "@/models/user";

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

vi.mock("primevue/textarea", () => ({
  default: {
    props: ["modelValue"],
    emits: ["update:modelValue"],
    template: `<textarea :value="modelValue" @input="$emit('update:modelValue', $event.target.value)"></textarea>`,
  },
}));

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
    (getCommentsByPostId as unknown as vi.Mock).mockResolvedValue({
      items: [
        {
          id: "c1",
          content: "First",
          authorName: "Bob",
          createdAt: new Date(),
          voteCount: 0,
          voteStatus: 0,
        },
      ],
    });

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
          Textarea: {
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

    expect(getPostById).toHaveBeenCalledWith("p1");
    expect(getCommentsByPostId).toHaveBeenCalledWith("p1");

    const title = wrapper.find('[data-testid="post-title"]');
    expect(title.exists()).toBe(true);
    expect(title.text()).toContain("Test Post");

    const header = wrapper.find(".comment-header");
    expect(header.text()).toContain("Comments (1)");
  });

  it("redirects to login when submitting a comment while not authenticated", async () => {
    (getPostById as unknown as vi.Mock).mockResolvedValue({
      id: "p1",
      title: "x",
      authorName: "a",
      createdAt: new Date(),
      tags: [],
      description: "",
      voteCount: 0,
      voteStatus: 0,
    });
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
    auth.user = null;

    const pushSpy = vi.spyOn(router, "push");

    const textarea = wrapper.find("textarea");
    await textarea.setValue("New comment");

    await wrapper.find(".comment-submit-button").trigger("click");

    expect(pushSpy).toHaveBeenCalledWith("/login");
    expect(createComment).not.toHaveBeenCalled();
  });

  it("submits a comment when authenticated and prepends it to the list", async () => {
    (getPostById as unknown as vi.Mock).mockResolvedValue({
      id: "p1",
      title: "x",
      authorName: "a",
      createdAt: new Date(),
      tags: [],
      description: "",
      voteCount: 0,
      voteStatus: 0,
    });
    (getMediaByPostId as unknown as vi.Mock).mockResolvedValue([]);
    (getCommentsByPostId as unknown as vi.Mock).mockResolvedValue({
      items: [
        {
          id: "c1",
          content: "Existing",
          authorName: "B",
          createdAt: new Date(),
          voteCount: 0,
          voteStatus: 0,
        },
      ],
    });

    const created = {
      id: "c2",
      content: "Hello new",
      authorName: "Me",
      createdAt: new Date(),
      voteCount: 0,
      voteStatus: 0,
    };
    (createComment as unknown as vi.Mock).mockResolvedValue(created);

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
          CommentList: {
            props: ["comments"],
            template: `<div><div v-for="c in comments" :key="c.id" class="comment-item">{{ c.content }}</div></div>`,
          },
        },
      },
    });

    await new Promise((r) => setTimeout(r, 0));

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

    const textarea = wrapper.find("textarea");
    await textarea.setValue("Hello new");
    await wrapper.find(".comment-submit-button").trigger("click");

    await new Promise((r) => setTimeout(r, 0));

    expect(createComment).toHaveBeenCalledWith(
      "p1",
      expect.objectContaining({ content: "Hello new" }),
    );

    const items = wrapper.findAll(".comment-item");
    expect(items[0].text()).toContain("Hello new");
    expect(items[1].text()).toContain("Existing");
  });
});
