import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
// Mock the authentication service before importing modules that use it
vi.mock("@/api/authenticationService", () => ({
  login: vi.fn(),
}));

import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import LoginView from "@/views/LoginView.vue";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import { User } from "@/models/user";
import { login } from "@/api/authenticationService";

// Simple Form stub that exposes named slots (actions/footer) and emits submit
const FormStub = {
  props: ["loading", "variant", "width", "title", "subtitle"],
  template: `
    <form @submit.prevent="$emit('submit', $event)">
      <slot />
      <slot name="actions" :loading="loading" />
      <slot name="footer" />
    </form>
  `,
};

// Simple InputText stub to support v-model and blur
const InputTextStub = {
  props: ["modelValue", "type", "id", "placeholder"],
  emits: ["update:modelValue", "blur"],
  template: `
    <input
      :id="id"
      :type="type || 'text'"
      :placeholder="placeholder"
      :value="modelValue"
      @input="$emit('update:modelValue', $event.target.value)"
      @blur="$emit('blur', $event)"
    />
  `,
};

describe("LoginView - integration", () => {
  let router: ReturnType<typeof createRouter>;

  beforeEach(async () => {
    // Ensure a fresh Pinia per test
    setActivePinia(createPinia());

    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", name: "home", component: { template: "<div/>" } },
        { path: "/register", name: "register", component: { template: "<div/>" } },
      ],
    });
  });

  afterEach(() => {
    vi.resetAllMocks();
  });

  it("successful login calls auth service, updates store and navigates to /", async () => {
    // Arrange - mock successful login response
  const fakeUser = { id: "1", username: "alice", displayName: "Alice", isAdmin: false, isBanned: false, createdAt: new Date(), updatedAt: new Date() } as unknown as User;
  (login as unknown as vi.Mock).mockResolvedValue(fakeUser);

    const wrapper = mount(LoginView, {
      global: {
        plugins: [router],
        stubs: {
          Form: FormStub,
          InputText: InputTextStub,
        },
      },
    });

    const auth = useAuthStore();

    // Act - fill form and submit
    const username = wrapper.find("#username");
    const password = wrapper.find("#password");
    await username.setValue("alice");
    await password.setValue("s3cret123");

    // Spy on router.push
    const pushSpy = vi.spyOn(router, "push");

    // Trigger the stubbed form's submit event directly
    const form = wrapper.find("form");
    await form.trigger("submit");

    // Wait a tick for promises
    await new Promise((r) => setTimeout(r, 0));

    // Assert - login called, store updated, navigation happened
    expect(login).toHaveBeenCalledWith("alice", "s3cret123");
    expect(auth.user).toBeTruthy();
    expect(auth.user?.username || auth.user?.id).toBe(fakeUser.username || fakeUser.id);
    expect(pushSpy).toHaveBeenCalledWith("/");
  });

  it("failed login shows error and does not set store user", async () => {
    // Arrange - mock failed login
    (login as unknown as vi.Mock).mockRejectedValue(new Error("invalid credentials"));

    const wrapper = mount(LoginView, {
      global: {
        plugins: [router],
        stubs: {
          Form: FormStub,
          InputText: InputTextStub,
        },
      },
    });

    const auth = useAuthStore();

    // Act
    await wrapper.find("#username").setValue("bob");
    await wrapper.find("#password").setValue("wrongpass");
    // Trigger the stubbed form's submit event directly
    const form = wrapper.find("form");
    await form.trigger("submit");

    await new Promise((r) => setTimeout(r, 0));

    // Assert
    expect(login).toHaveBeenCalledWith("bob", "wrongpass");
    // store should not have user
    expect(auth.user).toBeNull();
    // error message should be displayed
    const error = wrapper.find(".form-error");
    expect(error.exists()).toBe(true);
    expect(error.text()).toContain("Invalid username or password");
  });
});
