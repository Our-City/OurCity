/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { mount } from "@vue/test-utils";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { createPinia } from "pinia";
import CommentList from "@/components/CommentList.vue";
import CommentItem from "@/components/CommentItem.vue";

describe("CommentList.vue", () => {
  const mockComments = [
    {
      id: "1",
      authorName: "Alice",
      content: "First comment",
      createdAt: new Date(),
      voteCount: 2,
      voteStatus: 1,
    },
    {
      id: "2",
      authorName: "Bob",
      content: "Second comment",
      createdAt: new Date(),
      voteCount: 0,
      voteStatus: 1,
    },
  ];

  let wrapper: ReturnType<typeof mount>;

  beforeEach(() => {
    wrapper = mount(CommentList, {
      props: { comments: mockComments },
      global: {
        plugins: [createPinia()],
        stubs: {
          CommentItem: {
            template: `<div class="mock-comment-item"></div>`,
            props: ["comment"],
          },
        },
      },
    });
  });

  it("renders a list of CommentItem components", () => {
    const items = wrapper.findAllComponents(CommentItem);
    expect(items.length).toBe(mockComments.length);
  });

  it("renders each comment with correct props", () => {
    const renderedComments = wrapper.findAll(".mock-comment-item");
    expect(renderedComments).toHaveLength(2);
    expect(wrapper.props().comments[0].authorName).toBe("Alice");
  });
});
