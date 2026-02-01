<script setup lang="ts">
import type { ProductResponse } from "@/types/api/productTypes";
import { useDisplay } from "vuetify";
import ProductCard from "@/components/products/ProductCard.vue";

const props = defineProps<{
  products: ProductResponse[];
}>();

const { mdAndUp } = useDisplay();
</script>

<template>
  <v-sheet class="rounded-lg w-100" elevation="8">
    <v-slide-group class="pa-4 mx-auto" :show-arrows="mdAndUp">
      <v-slide-group-item
        v-for="product in props.products"
        :key="product.id"
        v-slot="{}"
      >
        <div class="mx-2">
          <ProductCard
            :product="product"
            :show-category="false"
            style="width: 280px"
          />
        </div>
      </v-slide-group-item>
    </v-slide-group>
  </v-sheet>
</template>
