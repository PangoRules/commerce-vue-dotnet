<script setup lang="ts">
import { useCategories } from "@/composables/useCategories";
import { computed, onMounted, onUnmounted, watch, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useDisplay } from "vuetify";
import type { CategoryResponse } from "@/types/api/categoryTypes";

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

// Per-category slideshow: track current image index for each category
const imageIndices = ref<Record<number, number>>({});
const intervalIds: number[] = [];

// Base interval with per-category offset so they don't all switch together
const BASE_INTERVAL = 4000;
const OFFSET_STEP = 1200;

function getCurrentImageIndex(category: CategoryResponse): number {
  return imageIndices.value[category.id] ?? 0;
}

function hasImages(category: CategoryResponse): boolean {
  return category.images.length > 0;
}

function startSlideshows() {
  stopSlideshows();
  const categories = featuredCategories.value;
  categories.forEach((cat, i) => {
    if (cat.images.length > 1) {
      imageIndices.value[cat.id] = 0;
      const interval = BASE_INTERVAL + i * OFFSET_STEP;
      const id = window.setInterval(() => {
        const current = imageIndices.value[cat.id] ?? 0;
        imageIndices.value[cat.id] = (current + 1) % cat.images.length;
      }, interval);
      intervalIds.push(id);
    }
  });
}

function stopSlideshows() {
  intervalIds.forEach((id) => window.clearInterval(id));
  intervalIds.length = 0;
}

// Start slideshows once data arrives
watch(featuredCategories, (cats) => {
  if (cats.length > 0) {
    startSlideshows();
  }
});

onUnmounted(() => {
  stopSlideshows();
});
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
        <v-card
          class="d-flex flex-column h-100"
          border
          flat
          :to="`/categories/${category.id}`"
        >
          <div
            v-if="hasImages(category)"
            class="category-slideshow"
            style="height: 280px"
          >
            <v-img
              v-for="(image, idx) in category.images"
              :key="image.id"
              :src="image.url"
              height="280"
              cover
              class="category-slideshow-image"
              :class="{ active: idx === getCurrentImageIndex(category) }"
            />
          </div>
          <v-img
            v-else
            src="https://placehold.co/400x400?text=No+Image"
            height="280"
            cover
          />
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
            <v-card
              class="ma-2 d-flex flex-column category-card"
              width="50%"
              border
              flat
              :to="`/categories/${category.id}`"
            >
              <div
                v-if="hasImages(category)"
                class="category-slideshow"
                style="height: 320px"
              >
                <v-img
                  v-for="(image, idx) in category.images"
                  :key="image.id"
                  :src="image.url"
                  height="320"
                  cover
                  class="category-slideshow-image"
                  :class="{ active: idx === getCurrentImageIndex(category) }"
                />
              </div>
              <v-img
                v-else
                src="https://placehold.co/400x400?text=No+Image"
                height="320"
                cover
              />
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

.category-slideshow {
  position: relative;
  overflow: hidden;
}

.category-slideshow-image {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  transition: opacity 1s ease-in-out;
}

.category-slideshow-image.active {
  opacity: 1;
}
</style>
