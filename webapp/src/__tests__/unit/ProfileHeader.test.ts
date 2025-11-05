/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { vi } from "vitest";
vi.mock('@/api/userService', () => ({
	updateCurrentUser: vi.fn().mockResolvedValue({ username: 'NewName' })
}));

import { describe, it, expect, vi } from "vitest";
import { mount } from "@vue/test-utils";
import ProfileHeader from "@/components/profile/ProfileHeader.vue";
import { createRouter, createWebHistory } from "vue-router";
import { createPinia } from "pinia";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: { template: '<div />' } },
    { path: '/create-post', component: { template: '<div />' } } 
  ]
});

const pinia = createPinia();
const mountOptions = {
	global: {
		plugins: [router, pinia],
	},
};

describe("ProfileHeader", () => {
	it("renders username correctly", () => {
		const wrapper = mount(ProfileHeader, {
			props: { username: "TestUser" },
			...mountOptions,
		});
		expect(wrapper.find(".profile-username").text()).toBe("TestUser");
		expect(wrapper.find(".edit-username-button").exists()).toBe(true);
	});

	it("shows input and buttons when editing username", async () => {
		const wrapper = mount(ProfileHeader, {
			props: { username: "TestUser" },
			...mountOptions,
		});
		await wrapper.find(".edit-username-button").trigger("click");
		expect(wrapper.find(".username-input").exists()).toBe(true);
		expect(wrapper.find(".save-button").exists()).toBe(true);
		expect(wrapper.find(".cancel-button").exists()).toBe(true);
	});

	it("restores original username when canceling edit", async () => {
		const wrapper = mount(ProfileHeader, {
			props: { username: "TestUser" },
			...mountOptions,
		});
		await wrapper.find(".edit-username-button").trigger("click");
		const input = wrapper.find(".username-input");
		await input.setValue("ChangedName");
		await wrapper.find(".cancel-button").trigger("click");
		expect(wrapper.find(".profile-username").text()).toBe("TestUser");
	});

	it("navigates to /create-post when Create Post button is clicked", async () => {
		const pushSpy = vi.spyOn(router, "push");
		const wrapper = mount(ProfileHeader, {
			props: { username: "TestUser" },
			...mountOptions,
		});
		await wrapper.find(".create-post-button").trigger("click");
		expect(pushSpy).toHaveBeenCalledWith("/create-post");
	});
});
