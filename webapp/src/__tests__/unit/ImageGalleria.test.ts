import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import ImageGalleria from "@/components/ImageGalleria.vue";

const images = [
  { src: "https://test.com/img1.jpg", alt: "Image 1", title: "Title 1" },
  { src: "https://test.com/img2.jpg", alt: "Image 2", title: "Title 2" },
  { src: "https://test.com/img3.jpg", alt: "Image 3", title: "Title 3" },
];

describe("ImageGalleria", () => {
  it("renders the first image by default", () => {
    const wrapper = mount(ImageGalleria, { props: { images } });
    const img = wrapper.find(".post-image");
    expect(img.exists()).toBe(true);
    expect(img.attributes("src")).toBe(images[0].src);
    expect(img.attributes("alt")).toBe(images[0].alt);
  });

  it("navigates to next and previous images", async () => {
    const wrapper = mount(ImageGalleria, { props: { images } });
    await wrapper.find(".image-next").trigger("click");
    expect(wrapper.find(".post-image").attributes("src")).toBe(images[1].src);

    await wrapper.find(".image-prev").trigger("click");
    expect(wrapper.find(".post-image").attributes("src")).toBe(images[0].src);
  });

  it("shows modal when image is clicked", async () => {
    const wrapper = mount(ImageGalleria, { props: { images } });
    await wrapper.find(".post-image").trigger("click");
    expect(wrapper.findComponent({ name: "ImageModal" }).props("show")).toBe(true);
  });

  it("highlights the correct indicator", async () => {
    const wrapper = mount(ImageGalleria, { props: { images } });
    let indicators = wrapper.findAll(".indicator");
    expect(indicators[0].classes()).toContain("active");
    await wrapper.find(".image-next").trigger("click");
    indicators = wrapper.findAll(".indicator");
    expect(indicators[1].classes()).toContain("active");
  });
});
