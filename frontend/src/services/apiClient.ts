import { createHttpClient } from "@/lib/http";
import { notifyApiError } from "@/lib/notify";

export const api = createHttpClient({
  defaultHeaders: { Accept: "application/json" },
  onError: (err) => notifyApiError(err),
});

//If you want to suppress a toast for a specific call:
//await api.get("/api/health", { silent: true });
