import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import PostList from "@/components/PostList.vue";

const posts = [
  {
    id: 1,
    title: "First Post",
    content: "Content of first post",
    upvotes: 10,
    createdAt: "2023-01-01T00:00:00Z",
    category: "general"
  },
  {
    id: 2,
    title: "Second Post",
    content: "Content of second post",
    upvotes: 5,
    createdAt: "2023-01-02T00:00:00Z",
    category: "general"
  }
];

describe("PostList", () => {
  it("renders the correct number of posts", () => {
    const wrapper = mount(PostList, {
      props: { posts },
      global: {
        stubs: {
          'router-link': true
        }
      }
    });
    const links = wrapper.findAll(".post-link");
    expect(links.length).toBe(posts.length);
  });

  it("shows empty message when no posts", () => {
    const wrapper = mount(PostList, {
      props: { posts: [] },
      global: {
        stubs: {
          'router-link': true
        }
      }
    });
    expect(wrapper.find(".empty-message").exists()).toBe(true);
    expect(wrapper.find(".empty-message p").text()).toContain("No posts found");
  });
});