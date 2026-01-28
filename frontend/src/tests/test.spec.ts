import { describe, it, expect } from "vitest";
import { screen } from "@testing-library/vue";

describe("vitest wiring", () => {
  it("jest-dom works", () => {
    document.body.innerHTML = `<div>hi</div>`;
    expect(screen.getByText("hi")).toBeInTheDocument();
  });
});
