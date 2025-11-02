import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import CommentList from "@/components/CommentList.vue";

const comments = [
  {
    id: 1,
    authorId: 2,
    postId: 3,
    content: "First comment",
    votes: 5,
    isDeleted: false,
    createdAt: "2023-01-01T00:00:00Z",
    updatedAt: "2023-01-01T00:00:00Z"
  },
  {
    id: 2,
    authorId: 3,
    postId: 3,
    content: "Second comment",
    votes: 2,
    isDeleted: false,
    createdAt: "2023-01-02T00:00:00Z",
    updatedAt: "2023-01-02T00:00:00Z"
  }
];

describe("CommentList", () => {
  it("renders the correct number of CommentItem components", () => {
    const wrapper = mount(CommentList, {
      props: { props: comments }
    });
    const items = wrapper.findAllComponents({ name: "CommentItem" });
    expect(items.length).toBe(comments.length);
  });

  it("passes the correct props to CommentItem", () => {
    const wrapper = mount(CommentList, {
      props: { props: comments }
    });
    const items = wrapper.findAllComponents({ name: "CommentItem" });
    items.forEach((item, idx) => {
      expect(item.props("props")).toEqual(comments[idx]);
    });
  });
});