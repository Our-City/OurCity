/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import LoginView from "@/views/LoginView.vue";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import { User } from "@/models/user";
import { login } from "@/api/authenticationService";

vi.mock("@/api/authenticationService", () => ({
  login: vi.fn(),
}));

vi.mock("@/api/authorizationService", () => ({
  canViewAdminDashboard: vi.fn().mockResolvedValue(false),
}));

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
    const fakeUser = {
      id: "1",
      username: "alice",
      displayName: "Alice",
      isAdmin: false,
      isBanned: false,
      createdAt: new Date(),
      updatedAt: new Date(),
    } as unknown as User;
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

    const username = wrapper.find("#username");
    const password = wrapper.find("#password");
    await username.setValue("alice");
    await password.setValue("s3cret123");

    const pushSpy = vi.spyOn(router, "push");

    const form = wrapper.find("form");
    await form.trigger("submit");

    await flushPromises();

    expect(login).toHaveBeenCalledWith("alice", "s3cret123");
    expect(auth.user).toBeTruthy();
    expect(auth.user?.username || auth.user?.id).toBe(fakeUser.username || fakeUser.id);
    expect(pushSpy).toHaveBeenCalledWith("/");
  });

  it("failed login shows error and does not set store user", async () => {
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

    await wrapper.find("#username").setValue("bob");
    await wrapper.find("#password").setValue("wrongpass");
    const form = wrapper.find("form");
    await form.trigger("submit");

    await new Promise((r) => setTimeout(r, 0));

    expect(login).toHaveBeenCalledWith("bob", "wrongpass");
    expect(auth.user).toBeNull();
    const error = wrapper.find(".form-error");
    expect(error.exists()).toBe(true);
    expect(error.text()).toContain("Invalid username or password");
  });
});
