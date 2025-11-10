import { describe, it, expect, beforeEach, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import VoteBox from "@/components/VoteBox.vue";
import { VoteType } from "@/types/enums";

describe("VoteBox - integration", () => {
  let router: ReturnType<typeof createRouter>;

  beforeEach(() => {
    setActivePinia(createPinia());

    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", name: "home", component: { template: "<div/>" } },
        { path: "/login", name: "login", component: { template: "<div/>" } },
      ],
    });
  });

  it("redirects to login when not authenticated", async () => {
    const auth = useAuthStore();
    auth.user = null;

  const pushSpy = vi.spyOn(router, "push");

    const wrapper = mount(VoteBox, {
      global: { plugins: [router] },
      props: { votes: 3 },
    });

    await wrapper.find(".vote-btn.upvote").trigger("click");
    expect(pushSpy).toHaveBeenCalledWith("/login");

    await wrapper.find(".vote-btn.downvote").trigger("click");
    expect(pushSpy).toHaveBeenCalledWith("/login");
    // ensure no vote event emitted
    expect(wrapper.emitted("vote")).toBeUndefined();
  });

  it("emits correct vote types when authenticated", async () => {
    const auth = useAuthStore();
    auth.user = { id: "u1", username: "tester" } as any;

    // No existing vote -> upvote should emit UPVOTE
    const wrapperNoVote = mount(VoteBox, {
      global: { plugins: [router] },
      props: { votes: 10 },
    });

    await wrapperNoVote.find(".vote-btn.upvote").trigger("click");
    expect(wrapperNoVote.emitted("vote")?.[0]).toEqual([VoteType.UPVOTE]);

    // Existing UPVOTE -> clicking upvote again should emit NOVOTE
    const wrapperUp = mount(VoteBox, {
      global: { plugins: [router] },
      props: { votes: 10, userVote: VoteType.UPVOTE },
    });

    await wrapperUp.find(".vote-btn.upvote").trigger("click");
    expect(wrapperUp.emitted("vote")?.[0]).toEqual([VoteType.NOVOTE]);

    // Existing DOWNVOTE -> clicking upvote should emit UPVOTE
    const wrapperDown = mount(VoteBox, {
      global: { plugins: [router] },
      props: { votes: 10, userVote: VoteType.DOWNVOTE },
    });

    await wrapperDown.find(".vote-btn.upvote").trigger("click");
    expect(wrapperDown.emitted("vote")?.[0]).toEqual([VoteType.UPVOTE]);

    // Existing NOVOTE (explicit) -> clicking downvote should emit DOWNVOTE
    const wrapperNoVote2 = mount(VoteBox, {
      global: { plugins: [router] },
      props: { votes: 4, userVote: VoteType.NOVOTE },
    });
    await wrapperNoVote2.find(".vote-btn.downvote").trigger("click");
    expect(wrapperNoVote2.emitted("vote")?.[0]).toEqual([VoteType.DOWNVOTE]);
  });
});


