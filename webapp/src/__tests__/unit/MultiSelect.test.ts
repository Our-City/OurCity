/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import MultiSelect from "@/components/utils/MultiSelect.vue";

const options = ["Apple", "Banana", "Cherry", "Date", "Elderberry"];

describe("MultiSelect", () => {
  it("renders placeholder and options", async () => {
    const wrapper = mount(MultiSelect, {
      props: { modelValue: [], options, placeholder: "Pick fruits" },
    });
    expect(wrapper.find(".multiselect__placeholder").text()).toBe("Pick fruits");
    await wrapper.find(".multiselect__trigger").trigger("click");
    options.forEach((opt) => {
      expect(wrapper.findAll(".multiselect__option-text").map((o) => o.text())).toContain(opt);
    });
  });

  it("filters options by search", async () => {
    const wrapper = mount(MultiSelect, {
      props: { modelValue: [], options },
    });
    await wrapper.find(".multiselect__trigger").trigger("click");
    const input = wrapper.find(".multiselect__search-input");
    await input.setValue("Ban");
    expect(wrapper.findAll(".multiselect__option-text").map((o) => o.text())).toContain("Banana");
    expect(wrapper.findAll(".multiselect__option-text").map((o) => o.text())).not.toContain(
      "Apple",
    );
  });

  it("selects and removes options, updates modelValue", async () => {
    const wrapper = mount(MultiSelect, {
      props: { modelValue: [], options },
    });
    await wrapper.find(".multiselect__trigger").trigger("click");
    await wrapper.findAll(".multiselect__option")[0].trigger("click");
    expect(wrapper.emitted()["update:modelValue"]).toBeTruthy();
    expect(wrapper.emitted()["change"]).toBeTruthy();
    await wrapper.setProps({ modelValue: ["Apple"] });
    await wrapper.find(".multiselect__tag-remove").trigger("click");
    expect(wrapper.emitted()["update:modelValue"]).toBeTruthy();
  });

  it("shows selected tags and '+N more' if >3 selected", async () => {
    const selected = ["Apple", "Banana", "Cherry", "Date"];
    const wrapper = mount(MultiSelect, {
      props: { modelValue: selected, options },
    });
    expect(wrapper.findAll(".multiselect__tag").length).toBe(3);
    expect(wrapper.find(".multiselect__more").text()).toContain("+1 more");
  });

  it("disables selection when maxSelected is reached", async () => {
    const wrapper = mount(MultiSelect, {
      props: { modelValue: ["Apple", "Banana"], options, maxSelected: 2 },
    });
    await wrapper.find(".multiselect__trigger").trigger("click");
    // Only already selected options are enabled
    const disabledOptions = wrapper.findAll(".multiselect__option--disabled");
    expect(disabledOptions.length).toBe(options.length - 2);
  });

  it("clears all selections", async () => {
    const wrapper = mount(MultiSelect, {
      props: { modelValue: ["Apple", "Banana"], options },
    });
    await wrapper.find(".multiselect__clear").trigger("click");
    expect(wrapper.emitted()["update:modelValue"]).toBeTruthy();
  });

  it("handles disabled state", async () => {
    const wrapper = mount(MultiSelect, {
      props: { modelValue: [], options, disabled: true },
    });
    expect(wrapper.find(".multiselect--disabled").exists()).toBe(true);
    await wrapper.find(".multiselect__trigger").trigger("click");
    expect(wrapper.find(".multiselect__dropdown").isVisible()).toBe(false);
  });
});
