/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import VoteBox from "@/components/VoteBox.vue";

describe("VoteBox", () => {
  it("renders the vote count", () => {
    const wrapper = mount(VoteBox, {
      props: { votes: 721 },
    });
    expect(wrapper.text()).toContain("721");
  });

  it("emits upvote event when upvote button is clicked", async () => {
    const wrapper = mount(VoteBox, {
      props: { votes: 0 },
    });
    const upvoteBtn = wrapper.find(".upvote");
    await upvoteBtn.trigger("click");
    expect(wrapper.emitted()).toHaveProperty("upvote");
  });

  it("emits downvote event when downvote button is clicked", async () => {
    const wrapper = mount(VoteBox, {
      props: { votes: 0 },
    });
    const downvoteBtn = wrapper.find(".downvote");
    await downvoteBtn.trigger("click");
    expect(wrapper.emitted()).toHaveProperty("downvote");
  });
});
