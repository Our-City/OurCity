import { describe, it, expect, vi } from "vitest";
import { mount } from "@vue/test-utils";
import SideBar from "@/components/SideBar.vue";
import { createRouter, createWebHistory } from "vue-router";

describe("SideBar", () => {
  const router = createRouter({
    history: createWebHistory(),
    routes: [{ path: "/", component: { template: "<div />" } }],
  });

  it("renders all navigation buttons", () => {
    const wrapper = mount(SideBar, {
      global: {
        plugins: [router],
      },
    });
    expect(wrapper.find(".home-button").exists()).toBe(true);
    expect(wrapper.find(".popular-button").exists()).toBe(true);
    expect(wrapper.find(".nearby-button").exists()).toBe(true);
    expect(wrapper.find(".recreational-button").exists()).toBe(true);
    expect(wrapper.find(".infrastructure-button").exists()).toBe(true);
  });

  it("renders the GitHub link", () => {
    const wrapper = mount(SideBar, {
      global: {
        plugins: [router],
      },
    });
    const githubLink = wrapper.find(".github-link");
    expect(githubLink.exists()).toBe(true);
    expect(githubLink.attributes("href")).toBe("https://github.com/Our-City/OurCity");
  });

  it("emits navigation when Home button is clicked", async () => {
    const pushSpy = vi.spyOn(router, "push");
    const wrapper = mount(SideBar, {
      global: {
        plugins: [router],
      },
    });
    await wrapper.find(".home-button").trigger("click");
    expect(pushSpy).toHaveBeenCalledWith("/");
    pushSpy.mockRestore();
  });
});
