/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import ImageModal from "@/components/ImageModal.vue";

describe("ImageModal", () => {
  it("renders image and title when show is true", () => {
    const wrapper = mount(ImageModal, {
      props: {
        show: true,
        imageUrl: "https://test.com/testimage.jpg",
        title: "Test Image",
      },
    });
    const modal = wrapper.find(".modal-layout");
    expect(modal.exists()).toBe(true);
    const img = wrapper.find(".modal-image");
    expect(img.exists()).toBe(true);
    expect(img.attributes("src")).toBe("https://test.com/testimage.jpg");
    expect(img.attributes("alt")).toBe("Test Image");
  });

  it("does not render when show is false", () => {
    const wrapper = mount(ImageModal, {
      props: {
        show: false,
        imageUrl: "https://test.com/testimage.jpg",
        title: "Test Image",
      },
    });
    expect(wrapper.find(".image-modal").exists()).toBe(false);
  });

  it("emits 'close' event when button is clicked", async () => {
    const wrapper = mount(ImageModal, {
      props: {
        show: true,
        imageUrl: "https://test.com/testimage.jpg",
        title: "Test Image",
      },
    });
    await wrapper.vm.$nextTick();
    const closeButton = wrapper.find(".close-modal-btn");
    expect(closeButton.exists()).toBe(true);
    await closeButton.trigger("click");
    expect(wrapper.emitted().close).toBeTruthy();
  });
});
