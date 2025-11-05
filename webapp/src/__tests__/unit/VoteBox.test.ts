/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.
import { describe, it, expect, beforeEach, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { setActivePinia, createPinia } from "pinia";
import VoteBox from "@/components/VoteBox.vue";

// mock authentication store to simulate logged-in user
vi.mock("@/stores/authenticationStore", () => ({
  useAuthStore: () => ({
    isAuthenticated: true, // always logged in for these tests
  }),
}));

// mock vue-router to avoid errors during tests
vi.mock("vue-router", () => ({
  useRouter: () => ({
    push: vi.fn(), // avoid navigation errors
  }),
}));

beforeEach(() => {
  setActivePinia(createPinia());
});

describe("VoteBox", () => {
  it("renders the vote count", () => {
    const wrapper = mount(VoteBox, {
      props: { votes: 721 },
    });
    expect(wrapper.text()).toContain("721");
  });

  it("emits upvote event when upvote button is clicked", async () => {
    const wrapper = mount(VoteBox, {
      props: { votes: 0, userVote: 0 },
    });

    const upvoteBtn = wrapper.find(".upvote");
    expect(upvoteBtn.exists()).toBe(true);

    await upvoteBtn.trigger("click");

    // verify event
    expect(wrapper.emitted()).toHaveProperty("vote");
    expect(wrapper.emitted().vote[0]).toEqual([1]); // optional: verify value
  });

  it("emits downvote event when downvote button is clicked", async () => {
    const wrapper = mount(VoteBox, {
      props: { votes: 0, userVote: 0 },
    });

    const downvoteBtn = wrapper.find(".downvote");
    expect(downvoteBtn.exists()).toBe(true);

    await downvoteBtn.trigger("click");

    // verify event
    expect(wrapper.emitted()).toHaveProperty("vote");
    expect(wrapper.emitted().vote[0]).toEqual([-1]); // optional: verify value
  });
});
