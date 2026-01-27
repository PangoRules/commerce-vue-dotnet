import { describe, it, expect, beforeEach } from "vitest";
import { createPinia, setActivePinia } from "pinia";
import { useToastStore } from "./toast";

describe("toast store", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it("has correct default state", () => {
    const toast = useToastStore();

    expect(toast.show).toBe(false);
    expect(toast.text).toBe("");
    expect(toast.level).toBe("info");
    expect(toast.timeout).toBe(4000);
  });

  it("open() sets state correctly", () => {
    const toast = useToastStore();

    toast.open("Hello", "warning", 2000);

    expect(toast.show).toBe(true);
    expect(toast.text).toBe("Hello");
    expect(toast.level).toBe("warning");
    expect(toast.timeout).toBe(2000);
  });

  it("success() uses success level and default timeout", () => {
    const toast = useToastStore();

    toast.success("Saved!");

    expect(toast.show).toBe(true);
    expect(toast.text).toBe("Saved!");
    expect(toast.level).toBe("success");
    expect(toast.timeout).toBe(4000);
  });

  it("error() uses error level and longer timeout", () => {
    const toast = useToastStore();

    toast.error("Boom");

    expect(toast.show).toBe(true);
    expect(toast.text).toBe("Boom");
    expect(toast.level).toBe("error");
    expect(toast.timeout).toBe(6000);
  });
});
