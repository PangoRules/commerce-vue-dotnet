<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";
import type { CategoryResponse } from "@/types/api/categoryTypes";

const props = withDefaults(
  defineProps<{
    category: CategoryResponse;
    imageHeight?: number;
  }>(),
  {
    imageHeight: 280,
  },
);

const currentIndex = ref(0);
let intervalId: number | undefined;

const hasImages = computed(() => props.category.images.length > 0);

onMounted(() => {
  if (props.category.images.length > 1) {
    intervalId = window.setInterval(() => {
      currentIndex.value =
        (currentIndex.value + 1) % props.category.images.length;
    }, 4000);
  }
});

onUnmounted(() => {
  if (intervalId !== undefined) {
    window.clearInterval(intervalId);
  }
});
</script>

<template>
  <v-card
    class="d-flex flex-column h-100"
    border
    flat
    :to="`/categories/${category.id}`"
  >
    <div
      v-if="hasImages"
      class="category-slideshow"
      :style="{ height: `${imageHeight}px` }"
    >
      <v-img
        v-for="(image, idx) in category.images"
        :key="image.id"
        :src="image.url"
        :height="imageHeight"
        cover
        class="category-slideshow-image"
        :class="{ active: idx === currentIndex }"
      />
    </div>
    <v-img
      v-else
      src="https://placehold.co/400x400?text=No+Image"
      :height="imageHeight"
      cover
    />
    <v-card-item>
      <v-card-title class="text-h6">{{ category.name }}</v-card-title>
      <v-card-subtitle v-if="category.description" class="text-wrap">
        {{ category.description }}
      </v-card-subtitle>
    </v-card-item>
  </v-card>
</template>

<style scoped>
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
