<script setup lang="ts">
import { useCategories } from "@/composables/useCategories";
import { computed, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { useDisplay } from "vuetify";

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
        <h2
          class="text-h5 font-weight-bold d-flex justify-space-between mb-4 align-center"
        >
          <div class="text-truncate">
            {{ t("categories.featuredCategories") }}
          </div>
        </h2>
      </v-col>
    </v-row>

    <v-carousel height="400" show-arrows="hover" hide-delimiter-background>
      <v-carousel-item
        v-for="category in featuredCategories"
        :key="category.id"
      >
        <v-card
          class="d-flex flex-column h-100"
          border
          flat
          :to="`/products?category=${category.id}`"
        >
          <v-img src="https://placehold.co/400x400" height="280" cover />
          <v-card-item>
            <v-card-title class="text-h6">{{ category.name }}</v-card-title>
            <v-card-subtitle class="text-wrap">
              {{ category.description }}
            </v-card-subtitle>
          </v-card-item>
        </v-card>
      </v-carousel-item>
    </v-carousel>
  </v-container>

  <!-- Desktop: Slide group with multiple visible -->
  <v-container v-else>
    <v-row>
      <v-col cols="12">
        <h2
          class="text-h4 font-weight-bold d-flex justify-space-between mb-4 align-center"
        >
          <div class="text-truncate">
            {{ t("categories.featuredCategories") }}
          </div>
        </h2>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-slide-group show-arrows>
          <v-slide-group-item
            v-for="category in featuredCategories"
            :key="category.id"
          >
            <v-card
              class="ma-2 d-flex flex-column category-card"
              width="50%"
              border
              flat
              :to="`/products?category=${category.id}`"
            >
              <v-img src="https://placehold.co/400x400" height="320" cover />
              <v-card-item class="flex-grow-1">
                <v-card-title class="text-h5">{{ category.name }}</v-card-title>
                <v-card-subtitle class="text-wrap text-body-1">
                  {{ category.description }}
                </v-card-subtitle>
              </v-card-item>
            </v-card>
          </v-slide-group-item>
        </v-slide-group>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
.category-card {
  min-height: 420px;
}
</style>
