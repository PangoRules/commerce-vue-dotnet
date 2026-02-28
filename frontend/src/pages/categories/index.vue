<script setup lang="ts">
import { useCategories } from "@/composables/useCategories";
import type { CategoryListQuery } from "@/types/api/categoryTypes";
import { ref } from "vue";
import { computed, onMounted } from "vue";

//Categories
const { loadCategoryList, listCategoryResult, isCategoryListLoading } =
  useCategories();

const categoryListQuery = ref<CategoryListQuery>({
  pageSize: 10,
  isActive: true,
});
const getCategories = async () => {
  if (isLoading.value) return;

  await loadCategoryList(categoryListQuery.value);
};
const categories = computed(() => {
  if (!listCategoryResult.value?.ok) return [];
  return listCategoryResult.value.data;
});

//Helpers
const isLoading = computed(() => isCategoryListLoading.value);

onMounted(async () => {
  await getCategories();
});
</script>

<template>
  <v-container class="py-8 mx-auto" fluid>
    <h1 class="text-h3 text-md-h2 font-weight-bold mb-4">Categories</h1>
    <v-data-iterator
      :items="categories"
      :items-per-page="categoryListQuery.pageSize"
    >
      <template v-slot:header="{ page, pageCount, prevPage, nextPage }">
        <h3
          class="text-headline-large font-weight-bold d-flex justify-space-between mt-0 mb-4 align-center"
        >
          <div class="text-truncate">All Categories</div>
          <div class="d-flex align-center">
            <div class="d-inline-flex">
              <v-btn
                :disabled="page === 1"
                class="me-2"
                icon="mdi-arrow-left"
                size="small"
                variant="tonal"
                @click="prevPage"
              ></v-btn>

              <v-btn
                :disable="page === pageCount"
                icon="mdi-arrow-right"
                size="small"
                variant="tonal"
                @click="nextPage"
              ></v-btn>
            </div>
          </div>
        </h3>
      </template>

      <template v-slot:default="{ items }">
        <v-row>
          <v-col v-for="(item, i) in items" :key="i" cols="12" md="6" xl="3">
            <v-sheet border></v-sheet>
          </v-col>
        </v-row>
      </template>
    </v-data-iterator>
  </v-container>
</template>
