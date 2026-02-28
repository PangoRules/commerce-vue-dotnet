<script setup lang="ts">
import { useCategories } from "@/composables/useCategories";
import { computed, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { useDisplay } from "vuetify";
import { CategoryCard } from "@/components/categories";

const { listCategoryResult, isCategoryListLoading, loadCategoryList } =
  useCategories();

const featuredCategories = computed(() => {
  if (!listCategoryResult.value?.ok) return [];
  return listCategoryResult.value.data;
});

onMounted(async () => {
  await loadCategoryList({ page: 1, pageSize: 10, featuredOnly: true });
});

const { t } = useI18n();
const { mdAndUp } = useDisplay();
</script>

<template>
  <div v-if="isCategoryListLoading" class="d-flex justify-center pa-8">
    <v-progress-circular indeterminate />
  </div>

  <!-- Mobile: Carousel with one item per page -->
  <v-container v-else-if="!mdAndUp" class="mx-auto" max-width="100%">
    <v-row>
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center">
          <h2 class="text-truncate text-h4 font-weight-bold">
            {{ t("categories.featuredCategories") }}
          </h2>
          <v-chip to="/categories">{{ t("common.actions.viewAll") }}</v-chip>
        </div>
      </v-col>
    </v-row>

    <v-carousel height="400" show-arrows="hover" hide-delimiter-background>
      <v-carousel-item
        v-for="category in featuredCategories"
        :key="category.id"
      >
        <CategoryCard :category="category" :image-height="280" />
      </v-carousel-item>
    </v-carousel>
  </v-container>

  <!-- Desktop: Slide group with multiple visible -->
  <v-container v-else>
    <div class="d-flex justify-space-between align-center">
      <h2 class="text-truncate text-h4 font-weight-bold">
        {{ t("categories.featuredCategories") }}
      </h2>
      <v-chip to="/categories">{{ t("common.actions.viewAll") }}</v-chip>
    </div>
    <v-row>
      <v-col cols="12">
        <v-slide-group show-arrows>
          <v-slide-group-item
            v-for="category in featuredCategories"
            :key="category.id"
          >
            <div class="ma-2" style="width: 50%; min-height: 420px">
              <CategoryCard :category="category" :image-height="320" />
            </div>
          </v-slide-group-item>
        </v-slide-group>
      </v-col>
    </v-row>
  </v-container>
</template>

