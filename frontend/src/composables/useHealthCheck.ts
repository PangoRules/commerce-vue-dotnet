import { ref } from "vue";
import type { ApiResult } from "@/lib/http";
import { api } from "@/services/apiClient";

export type HealthResponse = { status: string };

export function useHealthCheck() {
  const result = ref<ApiResult<HealthResponse> | null>(null);
  const isLoading = ref(false);

  async function check() {
    isLoading.value = true;
    result.value = await api.get<HealthResponse>("/api/health");
    isLoading.value = false;
  }

  return { result, isLoading, check };
}
