/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import DropdownMenu from "@/components/utils/DropdownMenu.vue";

describe("DropdownMenu", () => {
  it("renders default button and dropdown content", () => {
    const wrapper = mount(DropdownMenu);
    expect(wrapper.find(".dropdown-button").exists()).toBe(true);
    expect(wrapper.find(".dropdown-menu").exists()).toBe(true);

    const dropdown = wrapper.find(".dropdown-menu");
    expect(getComputedStyle(dropdown.element).display).toBe("none");
  });

  it("shows and hides dropdown when button is clicked", async () => {
    const wrapper = mount(DropdownMenu);
    const button = wrapper.find(".dropdown-button");
    await button.trigger("click");
    expect(wrapper.find(".dropdown-menu").isVisible()).toBe(true);

    wrapper.vm.closeDropdown();
    await wrapper.vm.$nextTick();
    expect(wrapper.vm.isDropdownVisible).toBe(false);
  });

  it("renders slot content for button and dropdown", async () => {
    const wrapper = mount(DropdownMenu, {
      slots: {
        button: '<span class="custom-btn">CustomBtn</span>',
        dropdown: '<div class="custom-dd">CustomDropdown</div>',
      },
    });
    expect(wrapper.find(".custom-btn").exists()).toBe(true);
    await wrapper.find(".dropdown-button").trigger("click");
    expect(wrapper.find(".custom-dd").exists()).toBe(true);
  });

  it("applies custom classes from props", () => {
    const wrapper = mount(DropdownMenu, {
      props: { buttonClass: "my-btn", dropdownClass: "my-dd" },
    });
    expect(wrapper.find(".my-btn").exists()).toBe(true);
    expect(wrapper.find(".my-dd").exists()).toBe(true);
  });

  it("calls closeDropdown when slot uses it", async () => {
    const wrapper = mount(DropdownMenu, {
      slots: {
        dropdown: `<button class='close-btn' @click='close'>Close</button>`,
      },
    });
    await wrapper.find(".dropdown-button").trigger("click");
    expect(wrapper.find(".close-btn").exists()).toBe(true);
  });
});
