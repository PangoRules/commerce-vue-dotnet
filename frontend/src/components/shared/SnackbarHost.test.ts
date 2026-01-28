import { describe, it, expect } from "vitest";
import { screen } from "@testing-library/vue";
import { renderWithPlugins } from "@/tests/render";
import SnackbarHost from "./SnackbarHost.vue";
import { useToastStore } from "@/stores/toast";

describe("SnackbarHost", () => {
  it("renders toast text when show is true", async () => {
    await renderWithPlugins(SnackbarHost);

    const toast = useToastStore();
    toast.text = "Saved!";
    toast.level = "success";
    toast.timeout = 3000;
    toast.show = true;

    expect(await screen.findByText("Saved!")).toBeInTheDocument();
  });

  it("closes snackbar when Close button is clicked", async () => {
    await renderWithPlugins(SnackbarHost);

    const toast = useToastStore();
    toast.text = "Closing";
    toast.show = true;

    // wait until it actually appears
    await screen.findByText("Closing");

    const closeBtn = await screen.findByRole("button", { name: /close/i });
    await closeBtn.click();

    expect(toast.show).toBe(false);
  });
});
