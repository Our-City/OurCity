/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import FormCmp from "@/components/utils/FormCmp.vue";

describe("FormCmp", () => {
	it("renders title and subtitle", () => {
		const wrapper = mount(FormCmp, {
			props: { title: "Test Title", subtitle: "Test Subtitle" },
		});
		expect(wrapper.find(".form-title").text()).toBe("Test Title");
		expect(wrapper.find(".form-subtitle").text()).toBe("Test Subtitle");
	});

	it("renders slot content", () => {
		const wrapper = mount(FormCmp, {
			slots: {
				default: '<div class="custom-field">Field</div>',
				actions: '<button class="custom-action">Action</button>',
				header: '<div class="custom-header">Header</div>',
				footer: '<div class="custom-footer">Footer</div>',
				before: '<div class="custom-before">Before</div>',
				between: '<div class="custom-between">Between</div>',
				after: '<div class="custom-after">After</div>',
			},
		});
		expect(wrapper.find(".custom-field").exists()).toBe(true);
		expect(wrapper.find(".custom-action").exists()).toBe(true);
		expect(wrapper.find(".custom-header").exists()).toBe(true);
		expect(wrapper.find(".custom-footer").exists()).toBe(true);
		expect(wrapper.find(".custom-before").exists()).toBe(true);
		expect(wrapper.find(".custom-between").exists()).toBe(true);
		expect(wrapper.find(".custom-after").exists()).toBe(true);
	});

	it("emits submit event when form is submitted", async () => {
		const wrapper = mount(FormCmp);
		await wrapper.find("form").trigger("submit");
		expect(wrapper.emitted().submit).toBeTruthy();
	});

	it("does not emit submit when loading or disabled", async () => {
		const wrapperLoading = mount(FormCmp, { props: { loading: true } });
		await wrapperLoading.find("form").trigger("submit");
		expect(wrapperLoading.emitted().submit).toBeFalsy();

		const wrapperDisabled = mount(FormCmp, { props: { disabled: true } });
		await wrapperDisabled.find("form").trigger("submit");
		expect(wrapperDisabled.emitted().submit).toBeFalsy();
	});

	it("emits reset event when form is reset", async () => {
		const wrapper = mount(FormCmp);
		await wrapper.find("form").trigger("reset");
		expect(wrapper.emitted().reset).toBeTruthy();
	});

	it("does not emit reset when loading or disabled", async () => {
		const wrapperLoading = mount(FormCmp, { props: { loading: true } });
		await wrapperLoading.find("form").trigger("reset");
		expect(wrapperLoading.emitted().reset).toBeFalsy();

		const wrapperDisabled = mount(FormCmp, { props: { disabled: true } });
		await wrapperDisabled.find("form").trigger("reset");
		expect(wrapperDisabled.emitted().reset).toBeFalsy();
	});

	it("shows loading overlay when loading is true", () => {
		const wrapper = mount(FormCmp, { props: { loading: true } });
		expect(wrapper.find(".form-loading-overlay").exists()).toBe(true);
	});

	it("applies correct classes for variant and width", () => {
		const wrapper = mount(FormCmp, { props: { variant: "card", width: "wide" } });
		expect(wrapper.find(".form-container--card").exists()).toBe(true);
		expect(wrapper.find(".form-container--wide").exists()).toBe(true);
	});
});
