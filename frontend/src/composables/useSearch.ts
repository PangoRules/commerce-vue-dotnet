import { ref, computed } from "vue";
import { useRouter } from "vue-router";

export function useSearch() {
  const router = useRouter();

  const searchQuery = ref("");
  const selectedCategoryId = ref<number | null>(null);

  const hasSearchQuery = computed(() => searchQuery.value.trim().length > 0);

  function setSearchQuery(query: string) {
    searchQuery.value = query;
  }

  function setSelectedCategory(categoryId: number | null) {
    selectedCategoryId.value = categoryId;
  }

  function clearSearch() {
    searchQuery.value = "";
    selectedCategoryId.value = null;
  }

  function submitSearch() {
    const query: Record<string, string> = {};

    if (searchQuery.value.trim()) {
      query.q = searchQuery.value.trim();
    }

    if (selectedCategoryId.value !== null) {
      query.category = String(selectedCategoryId.value);
    }

    router.push({ path: "/products", query });
  }

  return {
    searchQuery,
    selectedCategoryId,
    hasSearchQuery,
    setSearchQuery,
    setSelectedCategory,
    clearSearch,
    submitSearch,
  };
}
