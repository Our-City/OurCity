/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect, vi } from "vitest";
import { mount } from "@vue/test-utils";
import PostItem from "@/components/PostItem.vue";

vi.mock("@/api/mediaService", () => ({
  getMediaByPostId: vi.fn()
}));

import { getMediaByPostId } from "@/api/mediaService";

const post = {
  id: 1,
  authorId: 2,
  authorName: "Test Author",
  title: "Test Post Title",
  location: "Test Location",
  voteCount: 12,
  commentCount: 3,
  createdAt: "2023-01-01T00:00:00Z"
};

describe("PostItem", () => {
  it("renders author, title, votes, and comment count", async () => {
    getMediaByPostId.mockResolvedValue([{ url: "https://test.com/image.jpg" }]);
    const wrapper = mount(PostItem, {
      props: { post }
    });
    await Promise.resolve();
    await wrapper.vm.$nextTick();

    expect(wrapper.find(".post-author-date").exists()).toBe(true);
    expect(wrapper.find(".post-title").text()).toBe(post.title);
    expect(wrapper.findAll(".post-number-stats")[0].text()).toBe(String(post.voteCount));
    expect(wrapper.findAll(".post-number-stats")[1].text()).toBe(String(post.commentCount));
  });

  it("renders post image if available", async () => {
    getMediaByPostId.mockResolvedValue([{ url: "https://test.com/image.jpg" }]);
    const wrapper = mount(PostItem, {
      props: { post }
    });
    await Promise.resolve();
    await wrapper.vm.$nextTick();
    const img = wrapper.find(".post-image");
    expect(img.exists()).toBe(true);
    expect(img.attributes("src")).toBe("https://test.com/image.jpg");
  });

  it("does not render post image if not available", async () => {
    getMediaByPostId.mockResolvedValue([]);
    const wrapper = mount(PostItem, {
      props: { post }
    });
    await Promise.resolve();
    await wrapper.vm.$nextTick();
    expect(wrapper.find(".post-image").exists()).toBe(false);
  });
});