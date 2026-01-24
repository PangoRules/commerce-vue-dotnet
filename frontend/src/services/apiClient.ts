import { createHttpClient } from "@/lib/http";

export const api = createHttpClient({
  // baseUrl defaults to VITE_API_BASE_URL already
  defaultHeaders: {
    Accept: "application/json",
  },
});
