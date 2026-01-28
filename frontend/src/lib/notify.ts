import type { Pinia } from "pinia";
import { useToastStore } from "@/stores/toast";
import type { ApiError } from "@/lib/http";

let _pinia: Pinia | null = null;

/**
 * Called once in main.ts so non-Vue code
 * (http client, services, etc) can fire toasts
 */
export function initNotify(pinia: Pinia) {
  _pinia = pinia;
}

function toast() {
  if (!_pinia) return null;
  return useToastStore(_pinia);
}

export function notifyError(message: string) {
  toast()?.error(message);
}

export function notifyApiError(err: ApiError) {
  const message =
    err.kind === "timeout"
      ? "Request timed out. Please try again."
      : err.kind === "network"
        ? "Network error. Check your connection."
        : err.message || "Something went wrong.";

  toast()?.error(message);
}
