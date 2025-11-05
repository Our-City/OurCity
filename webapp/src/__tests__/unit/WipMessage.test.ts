/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import WipMessage from "@/components/WipMessage.vue";

describe("WipMessage", () => {
  it("renders the message prop", () => {
    const wrapper = mount(WipMessage, {
      props: { message: "Work in progress!" },
    });
    expect(wrapper.find("#wip-msg").text()).toBe("Work in progress!");
  });

  it("renders the description prop", () => {
    const wrapper = mount(WipMessage, {
      props: { description: "This feature is not yet available." },
    });
    expect(wrapper.find("#wip-description").text()).toBe("This feature is not yet available.");
  });

  it("renders both message and description", () => {
    const wrapper = mount(WipMessage, {
      props: { message: "WIP", description: "Coming soon." },
    });
    expect(wrapper.find("#wip-msg").text()).toBe("WIP");
    expect(wrapper.find("#wip-description").text()).toBe("Coming soon.");
  });
});
