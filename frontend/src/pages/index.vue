<script setup lang="ts">
import DealsCarousel from "@/components/home/DealsCarousel.vue";
import FeaturedCategories from "@/components/home/FeaturedCategories.vue";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const welcomeTitle = computed(() =>
  t("home.hero.title", { appName: t("app.name") }),
);
</script>

<template>
  <v-container class="py-8 mx-auto" fluid>
    <!-- Hero Section -->
    <section class="hero-section text-center mb-12">
      <h1 class="text-h3 text-md-h2 font-weight-bold mb-4">
        {{ welcomeTitle }}
      </h1>
      <p class="text-h6 text-medium-emphasis mx-auto" style="max-width: 600px">
        {{ t("home.hero.subtitle") }}
      </p>
    </section>

    <!-- Featured Categories -->
    <FeaturedCategories />

    <!-- Deals Carousel -->
    <DealsCarousel />
  </v-container>
</template>
