/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import PageHeader from "@/components/PageHeader.vue";
import { createRouter, createWebHistory } from "vue-router";
import { createPinia } from "pinia";

const router = createRouter({
  history: createWebHistory(),
  routes: [{ path: "/", component: { template: "<div />" } }],
});

const pinia = createPinia();

const mountOptions = {
  global: {
    plugins: [router, pinia],
    stubs: {
      InputText: { template: '<input class="search-input" />' },
      Dropdown: true,
      // Do not stub Toolbar so slot content is rendered
    },
  },
};

describe("PageHeader", () => {
  it("renders the app title", () => {
    const wrapper = mount(PageHeader, mountOptions);
    expect(wrapper.find(".app-title").exists()).toBe(true);
  });

  it("renders the search input", () => {
    const wrapper = mount(PageHeader, mountOptions);
    expect(wrapper.find(".search-input").exists()).toBe(true);
    expect(wrapper.find(".search-input").attributes("placeholder")).toBe("Search...");
  });

  it("renders login and signup buttons when not logged in", () => {
    const wrapper = mount(PageHeader, mountOptions);
    expect(wrapper.find(".login-button").exists()).toBe(true);
    expect(wrapper.find(".signup-button").exists()).toBe(true);
  });

  it("updates searchQuery when typing", async () => {
    const wrapper = mount(PageHeader, mountOptions);
    const input = wrapper.find(".search-input");
    await input.setValue("parks");
    expect(input.element.value).toBe("parks");
  });
});
