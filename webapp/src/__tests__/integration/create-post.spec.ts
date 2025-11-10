/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";

// Mock API modules before importing the view
vi.mock("@/api/postService", () => ({
  createPost: vi.fn(),
}));
vi.mock("@/api/mediaService", () => ({
  uploadMedia: vi.fn(),
}));
vi.mock("@/api/tagService", () => ({
  getTags: vi.fn(),
}));

// Mock resolveErrorMessage so tests can assert predictable error text
vi.mock("@/utils/error", () => ({
  resolveErrorMessage: (err: unknown, fallback: string) => fallback,
}));

// Mock PrimeVue toast composable to capture toast.add calls
const toastAdd = vi.fn();
vi.mock("primevue/usetoast", () => ({
  useToast: () => ({ add: toastAdd }),
}));

import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import CreatePostView from "@/views/CreatePostView.vue";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";
import { createPost } from "@/api/postService";
import { uploadMedia } from "@/api/mediaService";
import { getTags } from "@/api/tagService";
import type { User } from "@/models/user";

// Will hold original URL.createObjectURL to restore after tests
let originalCreateObjectURL: unknown;

// Simple Form stub that exposes named slots (actions/footer) and emits submit/reset
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

// Simple textarea stub
const TextareaStub = {
  props: ["modelValue", "id"],
  emits: ["update:modelValue", "blur"],
  template: `<textarea :id="id" :value="modelValue" @input="$emit('update:modelValue', $event.target.value)" @blur="$emit('blur', $event)"></textarea>`,
};

// Simple MultiSelect stub that supports v-model
const MultiSelectStub = {
  props: ["modelValue", "options", "id"],
  emits: ["update:modelValue"],
  template: `<select :id="id" multiple @change="$emit('update:modelValue', Array.from($event.target.selectedOptions).map(o => JSON.parse(o.value)))">
    <option v-for="o in options" :key="o.id" :value="JSON.stringify(o)">{{ o.name }}</option>
  </select>`,
};

describe("CreatePostView - integration", () => {
  let router: ReturnType<typeof createRouter>;

  beforeEach(() => {
    setActivePinia(createPinia());

    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", name: "home", component: { template: "<div/>" } },
        { path: "/posts/:id", name: "post-detail", component: { template: "<div/>" } },
      ],
    });

    // Default tag fetch returns empty array unless overridden by a test
    (getTags as unknown as vi.Mock).mockResolvedValue([]);

    // Stub URL.createObjectURL for tests that create File objects
    // Save existing function to restore later
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    originalCreateObjectURL = URL.createObjectURL;
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    URL.createObjectURL = vi.fn(() => "blob:fake");
  });

  afterEach(() => {
    vi.resetAllMocks();

    // restore URL.createObjectURL
    try {
      // eslint-disable-next-line @typescript-eslint/ban-ts-comment
      // @ts-ignore
      URL.createObjectURL = originalCreateObjectURL;
    } catch {
      // ignore
    }
  });

  it("shows validation errors when required fields are missing or too short", async () => {
    const wrapper = mount(CreatePostView, {
      global: {
        plugins: [router],
        stubs: {
          Form: FormStub,
          InputText: InputTextStub,
          Textarea: TextareaStub,
          MultiSelect: MultiSelectStub,
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
        },
      },
    });

    // trigger submit without filling required fields
    const form = wrapper.find("form");
    await form.trigger("submit");

    await new Promise((r) => setTimeout(r, 0));

    // Title and description errors should be shown
    const titleError = wrapper
      .findAll(".form-error")
      .filter((n) => n.text().includes("Title is required"));
    const descError = wrapper
      .findAll(".form-error")
      .filter((n) => n.text().includes("Description is required"));

    expect(titleError.length).toBeGreaterThan(0);
    expect(descError.length).toBeGreaterThan(0);

    // provide a short description and ensure length validation prevents submission
    await wrapper.find("#title").setValue("Short title");
    const descTextarea = wrapper.find("textarea#description");
    if (descTextarea.exists()) {
      await descTextarea.setValue("short");
    } else {
      await wrapper.find("#description").setValue("short");
    }
    await form.trigger("submit");
    await new Promise((r) => setTimeout(r, 0));

    // There should still be form errors and createPost must not be called
    const anyErrors = wrapper.findAll(".form-error");
    expect(anyErrors.length).toBeGreaterThan(0);
    expect(createPost).not.toHaveBeenCalled();
  });

  it("shows submit error when not authenticated", async () => {
    const wrapper = mount(CreatePostView, {
      global: {
        plugins: [router],
        stubs: {
          Form: FormStub,
          InputText: InputTextStub,
          Textarea: TextareaStub,
          MultiSelect: MultiSelectStub,
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
        },
      },
    });

    // fill required fields
    await wrapper.find("#title").setValue("A title");
    await wrapper.find("#description").setValue("A long enough description");

    // ensure user is null
    const auth = useAuthStore();
    auth.user = null as unknown as User | null;

    // submit
    const form = wrapper.find("form");
    await form.trigger("submit");

    await new Promise((r) => setTimeout(r, 0));

    // form-level submit error should be displayed (resolveErrorMessage returns fallback)
    const submitError = wrapper
      .findAll(".form-error")
      .filter((n) => n.text().includes("Failed to create post"));
    expect(submitError.length).toBeGreaterThan(0);
    expect(createPost).not.toHaveBeenCalled();
  });

  it("creates a post, uploads images if present, shows toast and navigates to the new post", async () => {
    const fakeCreated = { id: "p123" };
    (createPost as unknown as vi.Mock).mockResolvedValue(fakeCreated);
    (uploadMedia as unknown as vi.Mock).mockResolvedValue({});
    // return some tags so MultiSelect has options
    (getTags as unknown as vi.Mock).mockResolvedValue([{ id: "t1", name: "Community" }]);

    const wrapper = mount(CreatePostView, {
      global: {
        plugins: [router],
        stubs: {
          Form: FormStub,
          InputText: InputTextStub,
          Textarea: TextareaStub,
          MultiSelect: MultiSelectStub,
          PageHeader: { template: "<div/>" },
          SideBar: { template: "<div/>" },
        },
      },
    });

    // ensure the auth store has a user
    const auth = useAuthStore();
    auth.user = { id: "u1", username: "me" } as unknown as User;

    // set form fields
    await wrapper.find("#title").setValue("A new event");
    await wrapper.find("#description").setValue("This is a description long enough.");

    // simulate selecting a tag via the MultiSelect stub
    const select = wrapper.find("select#tags");
    if (select.exists()) {
      // select the first option
      const option = select.element.querySelector("option");
      if (option) {
        option.selected = true;
        await select.trigger("change");
      }
    }

    // simulate file input (one image)
    const file = new File(["abc"], "photo.png", { type: "image/png" });
    const input = wrapper.find("#images");
    // trigger change with files list
    Object.defineProperty(input.element, "files", { value: [file] });
    await input.trigger("change");

    const pushSpy = vi.spyOn(router, "push");

    // submit
    const form = wrapper.find("form");
    await form.trigger("submit");

    // wait async operations
    await new Promise((r) => setTimeout(r, 0));

    expect(createPost).toHaveBeenCalled();
    // uploadMedia should have been called with the file and created post id
    expect(uploadMedia).toHaveBeenCalledWith(expect.any(File), fakeCreated.id);

    // toast.add was mocked by useToast; ensure it was called
    const { useToast } = await import("primevue/usetoast");
    expect(useToast().add).toHaveBeenCalled();

    expect(pushSpy).toHaveBeenCalledWith(`/posts/${fakeCreated.id}`);
  });
});
