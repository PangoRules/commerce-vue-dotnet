<script setup lang="ts">
import { ref, computed, watch, onMounted } from "vue";
import { useCategories } from "@/composables/useCategories";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  modelValue?: string;
  categoryId?: number | null;
  hideCategory?: boolean;
  expanded?: boolean;
}>();

const emit = defineEmits<{
  "update:modelValue": [value: string];
  "update:categoryId": [value: number | null];
  submit: [];
}>();

const { t } = useI18n();
const { loadCategoryList, listCategoryResult, isCategoryListLoading } =
  useCategories();

const localQuery = ref(props.modelValue ?? "");
const localCategoryId = ref<number | null>(props.categoryId ?? null);

const categoryItems = computed(() => {
  const allOption = { id: null, name: t("navbar.search.allCategories") };
  if (!listCategoryResult.value?.ok) {
    return [allOption];
  }
  return [allOption, ...listCategoryResult.value.data];
});

watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal !== undefined) {
      localQuery.value = newVal;
    }
  },
);

watch(
  () => props.categoryId,
  (newVal) => {
    localCategoryId.value = newVal ?? null;
  },
);

watch(localQuery, (newVal) => {
  emit("update:modelValue", newVal);
});

watch(localCategoryId, (newVal) => {
  emit("update:categoryId", newVal);
});

function handleSubmit() {
  emit("submit");
}

onMounted(() => {
  if (!isCategoryListLoading.value && !listCategoryResult.value) {
    loadCategoryList();
  }
});
</script>

<template>
  <div
    class="navbar-search d-flex align-center flex-grow-1 ga-0"
    :class="{ expanded }"
  >
    <v-select
      v-if="!hideCategory"
      v-model="localCategoryId"
      :items="categoryItems"
      item-title="name"
      item-value="id"
      density="compact"
      variant="outlined"
      hide-details
      class="navbar-search__category"
      :class="{ 'navbar-search__category--hidden': hideCategory }"
    />
    <v-text-field
      v-model="localQuery"
      :placeholder="$t('navbar.search.placeholder')"
      density="compact"
      variant="outlined"
      hide-details
      single-line
      class="navbar-search__input"
      @keyup.enter="handleSubmit"
    >
      <template #append-inner>
        <v-btn
          icon
          size="small"
          variant="text"
          color="primary"
          :aria-label="$t('navbar.search.button')"
          @click="handleSubmit"
        >
          <v-icon icon="mdi-magnify" />
        </v-btn>
      </template>
    </v-text-field>
  </div>
</template>

<style scoped>
.navbar-search {
  display: flex;
  align-items: center;
  flex: 10;
  gap: 0;
}

.navbar-search__category {
  flex: 0 0 auto;
  max-width: 20rem;
  border-radius: 4px 0 0 4px;
}

.navbar-search__category :deep(.v-field) {
  border-radius: 4px 0 0 4px;
}

.navbar-search__category--hidden {
  display: none;
}

.navbar-search__input {
  flex: 1;
}

.navbar-search__input :deep(.v-field) {
  border-radius: 0 4px 4px 0;
}

.navbar-search.expanded {
  max-width: none;
}

.navbar-search.expanded .navbar-search__input :deep(.v-field) {
  border-radius: 4px;
}
</style>
