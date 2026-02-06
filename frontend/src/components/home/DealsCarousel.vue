<script setup lang="ts">
import { useProducts } from "@/composables/useProducts";
import { computed, onMounted } from "vue";
import { useDisplay } from "vuetify";
import { useI18n } from "vue-i18n";
import ProductCard from "@/components/products/ProductCard.vue";

const { t } = useI18n();

const { listProductResult, isProductListLoading, loadProductList } =
  useProducts();

const products = computed(() => {
  if (!listProductResult.value?.ok) return [];
  return Object.values(listProductResult.value.data);
});

const { lgAndUp, smAndDown, mdAndUp } = useDisplay();

const itemsPerPage = computed(() => {
  if (smAndDown.value) return 1; // xs, sm
  if (lgAndUp.value) return 4; // lg, xl
  return 2; // md
});

onMounted(async () => {
  await loadProductList({ onSale: true });
});
</script>

<template>
  <div v-if="isProductListLoading" class="d-flex justify-center pa-8">
    <v-progress-circular indeterminate />
  </div>

  <v-container class="mx-auto">
    <v-data-iterator :items="products" :items-per-page="itemsPerPage">
      <template v-slot:header="{ page, pageCount, prevPage, nextPage }">
        <h2
          :class="[
            mdAndUp ? 'text-h4' : 'text-h5',
            'font-weight-bold d-flex justify-space-between mb-4 align-center',
          ]"
        >
          <div class="text-truncate">{{ t("home.deals.title") }}</div>

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
                :disabled="page === pageCount"
                icon="mdi-arrow-right"
                size="small"
                variant="tonal"
                @click="nextPage"
              ></v-btn>
            </div>
          </div>
        </h2>
      </template>

      <template v-slot:default="{ items }">
        <v-row justify="center">
          <v-col
            v-for="(item, i) in items"
            :key="i"
            cols="12"
            sm="6"
            md="6"
            lg="3"
            class="d-flex"
          >
            <ProductCard
              :product="item.raw"
              :show-category="false"
              class="flex-grow-1"
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:footer="{ page, pageCount }">
        <v-footer class="justify-space-between text-body-2 mt-4">
          <div>{{ t("home.deals.pagination", { page, pageCount }) }}</div>
        </v-footer>
      </template>
    </v-data-iterator>
  </v-container>
</template>
