import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import CommentItem from "@/components/CommentItem.vue";

const comment = {
  id: 1,
  authorId: 2,
  postId: 3,
  content: "Test comment content",
  votes: 7,
  isDeleted: false,
  createdAt: "2023-01-01T00:00:00Z",
  updatedAt: "2023-01-01T00:00:00Z",
};

describe("CommentItem", () => {
  it("renders author, content, and votes", () => {
    const wrapper = mount(CommentItem, {
      props: { props: comment },
    });

    expect(wrapper.find(".comment-author").text()).toBe(String(comment.authorId));
    expect(wrapper.find(".comment-text").text()).toBe(comment.content);

    // Check VoteBox prop
    const voteBox = wrapper.findComponent({ name: "VoteBox" });
    expect(voteBox.exists()).toBe(true);
    expect(voteBox.props("votes")).toBe(comment.votes);
  });
});
