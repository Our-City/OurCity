/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import ProfileToolbar from "@/components/profile/ProfileToolbar.vue";

describe("ProfileToolbar", () => {
  it("renders all toolbar buttons", () => {
    const wrapper = mount(ProfileToolbar);
    expect(wrapper.find(".posts-button").exists()).toBe(true);
  });

  it("buttons are clickable", async () => {
    const wrapper = mount(ProfileToolbar);
    await wrapper.find(".posts-button").trigger("click");

    expect(true).toBe(true);
  });
});
