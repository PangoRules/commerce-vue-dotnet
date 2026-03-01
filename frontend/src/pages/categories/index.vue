<script setup lang="ts">
import { useCategories } from "@/composables/useCategories";
import { CategoryCard } from "@/components/categories";
import type { CategoryListQuery } from "@/types/api/categoryTypes";
import { ref, computed, onMounted } from "vue";
import { useDisplay } from "vuetify";

const { loadCategoryList, listCategoryResult, isCategoryListLoading } =
  useCategories();

const { smAndDown, lgAndUp } = useDisplay();

const categoryListQuery = ref<CategoryListQuery>({
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

const isLoading = computed(() => isCategoryListLoading.value);

const itemsPerPage = computed(() => {
  if (smAndDown.value) return 4; // 2 cols × 2 rows
  if (lgAndUp.value) return 8; // 4 cols × 2 rows
  return 6; // 3 cols × 2 rows
});

onMounted(async () => {
  await getCategories();
});
</script>

<template>
  <v-container class="py-8 mx-auto">
    <h1 class="text-h3 text-md-h2 font-weight-bold mb-4">Categories</h1>

    <div v-if="isLoading" class="d-flex justify-center pa-8">
      <v-progress-circular indeterminate />
    </div>

    <v-data-iterator v-else :items="categories" :items-per-page="itemsPerPage">
      <template v-slot:header="{ page, pageCount, prevPage, nextPage }">
        <div class="d-flex justify-space-between mt-0 mb-4 align-center">
          <div class="d-flex align-center">
            <div class="d-inline-flex align-end">
              <v-btn
                :disabled="page === 1"
                class="me-2"
                icon="mdi-arrow-left"
                size="small"
                variant="tonal"
                @click="prevPage"
              ></v-btn>

              <span class="mx-2 text-body-2">{{ page }} / {{ pageCount }}</span>

              <v-btn
                :disabled="page === pageCount"
                icon="mdi-arrow-right"
                size="small"
                variant="tonal"
                @click="nextPage"
              ></v-btn>
            </div>
          </div>
        </div>
      </template>

      <template v-slot:default="{ items }">
        <v-row>
          <v-col
            v-for="(item, i) in items"
            :key="i"
            cols="12"
            sm="6"
            md="4"
            lg="3"
          >
            <CategoryCard :category="item.raw" />
          </v-col>
        </v-row>
      </template>
    </v-data-iterator>
  </v-container>
</template>
