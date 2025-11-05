/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

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
