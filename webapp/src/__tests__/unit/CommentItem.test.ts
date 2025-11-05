/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { mount } from "@vue/test-utils";
import { describe, it, expect, vi, beforeEach } from "vitest";
import CommentItem from "@/components/CommentItem.vue";
import VoteBox from "@/components/VoteBox.vue";
import { voteOnComment } from "@/api/commentService";
import { VoteType } from "@/types/enums";
import { useAuthStore } from "@/stores/authenticationStore";

vi.mock("@/api/commentService", () => ({
  voteOnComment: vi.fn(),
}));

vi.mock("@/stores/authenticationStore", () => ({
  useAuthStore: vi.fn(),
}));

describe("CommentItem.vue", () => {
  const mockComment = {
    id: 1,
    authorName: "Nathan",
    content: "This is a test comment.",
    createdAt: new Date("2024-01-01"),
    voteCount: 5,
    voteStatus: 1,
  };

  interface MockAuthStore {
    user: { id: number; name: string } | null;
  }
  let mockAuthStore: MockAuthStore;

  beforeEach(() => {
    mockAuthStore = { user: { id: 1, name: "Tester" } };
    (useAuthStore as unknown as { mockReturnValue: (store: MockAuthStore) => void }).mockReturnValue(mockAuthStore);
    vi.clearAllMocks();
  });

  it("renders comment details correctly", () => {
    const wrapper = mount(CommentItem, {
      props: { comment: mockComment },
    });

    expect(wrapper.text()).toContain("@Nathan");
    expect(wrapper.text()).toContain("This is a test comment.");
    expect(wrapper.text()).toContain(mockComment.createdAt.toLocaleDateString());
  });

  it("renders VoteBox with correct props", () => {
    const wrapper = mount(CommentItem, {
      props: { comment: mockComment },
    });

    const voteBox = wrapper.findComponent(VoteBox);
    expect(voteBox.exists()).toBe(true);
    expect(voteBox.props("votes")).toBe(5);
    expect(voteBox.props("userVote")).toBe(1);
  });

  it("calls voteOnComment and emits updated when user is authenticated", async () => {
    const updatedComment = { ...mockComment, voteCount: 6 };
    (voteOnComment as any).mockResolvedValue(updatedComment);

    const wrapper = mount(CommentItem, {
      props: { comment: mockComment },
    });

    const voteBox = wrapper.findComponent(VoteBox);
    await voteBox.vm.$emit("vote", VoteType.UPVOTE);

    expect(voteOnComment).toHaveBeenCalledWith(1, { voteType: VoteType.UPVOTE });

    await Promise.resolve();
    expect(wrapper.emitted("updated")).toBeTruthy();
    expect(wrapper.emitted("updated")![0][0]).toEqual(updatedComment);
  });

  it("shows alert when user is not authenticated", async () => {
    mockAuthStore.user = null;
    const alertMock = vi.spyOn(window, "alert").mockImplementation(() => {});

    const wrapper = mount(CommentItem, {
      props: { comment: mockComment },
    });

    const voteBox = wrapper.findComponent(VoteBox);
    await voteBox.vm.$emit("vote", VoteType.DOWNVOTE);

    expect(alertMock).toHaveBeenCalledWith("You must be logged in to vote on comments.");
    expect(voteOnComment).not.toHaveBeenCalled();

    alertMock.mockRestore();
  });

  it("handles errors from voteOnComment gracefully", async () => {
    (voteOnComment as any).mockRejectedValue(new Error("Network error"));
    const consoleSpy = vi.spyOn(console, "error").mockImplementation(() => {});

    const wrapper = mount(CommentItem, {
      props: { comment: mockComment },
    });

    const voteBox = wrapper.findComponent(VoteBox);
    await voteBox.vm.$emit("vote", VoteType.UPVOTE);

    await Promise.resolve();
    expect(consoleSpy).toHaveBeenCalledWith("Failed to vote on comment:", expect.any(Error));

    consoleSpy.mockRestore();
  });
});
