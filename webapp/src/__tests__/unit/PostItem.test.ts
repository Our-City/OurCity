import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import PostItem from "@/components/PostItem.vue";

const post = {
  id: 1,
  authorId: 2,
  title: "Test Post Title",
  location: "Test Location",
  votes: 12,
  commentIds: [101, 102, 103],
  images: [{ url: "https://test.com/image.jpg" }],
  createdAt: "2023-01-01T00:00:00Z"
};

describe("PostItem", () => {
  it("renders author, title, votes, and comment count", () => {
    const wrapper = mount(PostItem, {
      props: { post }
    });

    expect(wrapper.find(".post-author-date").exists()).toBe(true);
    expect(wrapper.find(".post-title").text()).toBe(post.title);
    expect(wrapper.findAll(".post-number-stats")[0].text()).toBe(String(post.votes));
    expect(wrapper.findAll(".post-number-stats")[1].text()).toBe(String(post.commentIds.length));
  });

  it("renders post image if available", () => {
    const wrapper = mount(PostItem, {
      props: { post }
    });
    const img = wrapper.find(".post-image");
    expect(img.exists()).toBe(true);
    expect(img.attributes("src")).toBe(post.images[0].url);
  });

  it("does not render post image if not available", () => {
    const postNoImage = { ...post, images: [] };
    const wrapper = mount(PostItem, {
      props: { post: postNoImage }
    });
    expect(wrapper.find(".post-image").exists()).toBe(false);
  });
});