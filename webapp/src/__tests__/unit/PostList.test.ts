/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import PostList from "@/components/PostList.vue";

const posts = [
  {
    id: 1,
    title: "First Post",
    content: "Content of first post",
    upvotes: 10,
    createdAt: new Date("2023-01-01T00:00:00Z"),
    category: "general"
  },
  {
    id: 2,
    title: "Second Post",
    content: "Content of second post",
    upvotes: 5,
    createdAt: new Date("2023-01-02T00:00:00Z"),
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