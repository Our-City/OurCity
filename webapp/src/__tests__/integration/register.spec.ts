import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
// Mock modules used by RegisterView before importing the view
vi.mock("@/api/userService", () => ({
  createUser: vi.fn(),
}));
vi.mock("@/api/authenticationService", () => ({
  login: vi.fn(),
}));
vi.mock("@/utils/error", () => ({
  resolveErrorMessage: vi.fn(() => "Registration failed."),
}));
vi.mock("primevue/usetoast", () => ({
  useToast: () => ({ add: vi.fn() }),
}));

import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import RegisterView from "@/views/RegisterView.vue";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import { createUser } from "@/api/userService";
import { login } from "@/api/authenticationService";
import { resolveErrorMessage } from "@/utils/error";

// Reuse a simple Form stub and InputText stub like the login tests
const FormStub = {
  props: ["loading", "variant", "width", "title", "subtitle"],
  template: `
    <form @submit.prevent="$emit('submit', $event)">
      <slot />
      <slot name="actions" :loading="loading" />
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

describe("RegisterView - integration", () => {
  let router: ReturnType<typeof createRouter>;

  beforeEach(() => {
    setActivePinia(createPinia());

    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", name: "home", component: { template: "<div/>" } },
      ],
    });
  });

  afterEach(() => {
    vi.resetAllMocks();
  });

  it("successful registration calls createUser, auto-logins, shows toast and navigates to /", async () => {
    const fakeUser = { id: "10", username: "newbie" } as any;
    (createUser as unknown as vi.Mock).mockResolvedValue({});
    (login as unknown as vi.Mock).mockResolvedValue(fakeUser);

    const wrapper = mount(RegisterView, {
      global: {
        plugins: [router],
        stubs: {
          Form: FormStub,
          InputText: InputTextStub,
        },
      },
    });

    const auth = useAuthStore();

    // Fill form fields
    await wrapper.find("#username").setValue("newbie");
    await wrapper.find("#password").setValue("s3cret123");
    await wrapper.find("#confirmPassword").setValue("s3cret123");

    const pushSpy = vi.spyOn(router, "push");

    // Submit
    const form = wrapper.find("form");
    await form.trigger("submit");

    // allow async tasks to resolve
    await new Promise((r) => setTimeout(r, 0));

    expect(createUser).toHaveBeenCalledWith("newbie", "s3cret123");
    expect(login).toHaveBeenCalledWith("newbie", "s3cret123");
    expect(auth.user).toBeTruthy();
    expect(pushSpy).toHaveBeenCalledWith("/");
  });

  it("failed registration shows an error message", async () => {
    (createUser as unknown as vi.Mock).mockRejectedValue(new Error("server error"));
    (resolveErrorMessage as unknown as vi.Mock).mockReturnValue("Server said no");

    const wrapper = mount(RegisterView, {
      global: {
        plugins: [router],
        stubs: {
          Form: FormStub,
          InputText: InputTextStub,
        },
      },
    });

    // Fill form
    await wrapper.find("#username").setValue("failuser");
    await wrapper.find("#password").setValue("abc123");
    await wrapper.find("#confirmPassword").setValue("abc123");

    // Submit
    const form = wrapper.find("form");
    await form.trigger("submit");

    await new Promise((r) => setTimeout(r, 0));

    // store shouldn't have a user
    const auth = useAuthStore();
    expect(auth.user).toBeNull();

    const error = wrapper.find(".form-error");
    expect(error.exists()).toBe(true);
    expect(error.text()).toContain("Server said no");
  });
});
