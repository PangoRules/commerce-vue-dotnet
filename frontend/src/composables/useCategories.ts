import { ref } from "vue";
import type { ApiResult } from "@/lib/http";
import type {
  CategoryAdminDetailsResponse,
  CategoryListQuery,
  CategoryRequest,
  CategoryResponse,
} from "@/types/api/categoryTypes";
import { categoryApi } from "@/services/categoryApi";

export function useCategories() {
  // LIST
  const listCategoryResult = ref<ApiResult<CategoryResponse[]> | null>(null);
  const isCategoryListLoading = ref(false);

  async function loadCategoryList(query?: CategoryListQuery) {
    isCategoryListLoading.value = true;
    listCategoryResult.value = await categoryApi.getCategories(query);
    isCategoryListLoading.value = false;
  }

  // BY ID (Admin details)
  const byCategoryIdResult =
    ref<ApiResult<CategoryAdminDetailsResponse> | null>(null);
  const isByCategoryIdLoading = ref(false);

  async function loadByCategoryId(id: number) {
    isByCategoryIdLoading.value = true;
    byCategoryIdResult.value = await categoryApi.getCategoryById(id);
    isByCategoryIdLoading.value = false;
  }

  // CREATE
  const createdCategoryResult = ref<ApiResult<null | void> | null>(null);
  const isCategoryCreatedLoading = ref(false);

  async function createCategory(request: CategoryRequest) {
    isCategoryCreatedLoading.value = true;
    createdCategoryResult.value = await categoryApi.postCategory(request);
    isCategoryCreatedLoading.value = false;
  }

  // UPDATE
  const updatedCategoryResult = ref<ApiResult<null | void> | null>(null);
  const isUpdatedCategoryLoading = ref(false);

  async function updateCategory(id: number, request: CategoryRequest) {
    isUpdatedCategoryLoading.value = true;
    updatedCategoryResult.value = await categoryApi.putCategory(id, request);
    isUpdatedCategoryLoading.value = false;
  }

  // TOGGLE ACTIVE
  const patchToggleCategoryStatusResult = ref<ApiResult<null | void> | null>(
    null,
  );
  const isPatchToggleCategoryStatusLoading = ref(false);

  async function toggleCategoryStatus(id: number) {
    isPatchToggleCategoryStatusLoading.value = true;
    patchToggleCategoryStatusResult.value =
      await categoryApi.toggleCategory(id);
    isPatchToggleCategoryStatusLoading.value = false;
  }

  // ROOTS
  const rootsResult = ref<ApiResult<CategoryResponse[]> | null>(null);
  const isRootsLoading = ref(false);

  async function loadRoots(includeInactive = false) {
    isRootsLoading.value = true;
    rootsResult.value = await categoryApi.getRoots(includeInactive);
    isRootsLoading.value = false;
  }

  // CHILDREN
  const childrenResult = ref<ApiResult<CategoryResponse[]> | null>(null);
  const isChildrenLoading = ref(false);

  async function loadChildren(
    parentCategoryId: number,
    includeInactive = false,
  ) {
    isChildrenLoading.value = true;
    childrenResult.value = await categoryApi.getChildren(
      parentCategoryId,
      includeInactive,
    );
    isChildrenLoading.value = false;
  }

  // ATTACH CHILD
  const attachChildResult = ref<ApiResult<null | void> | null>(null);
  const isAttachChildLoading = ref(false);

  async function attachChild(
    parentCategoryId: number,
    childCategoryId: number,
  ) {
    isAttachChildLoading.value = true;
    attachChildResult.value = await categoryApi.attachChild(
      parentCategoryId,
      childCategoryId,
    );
    isAttachChildLoading.value = false;
  }

  // DETACH CHILD
  const detachChildResult = ref<ApiResult<null | void> | null>(null);
  const isDetachChildLoading = ref(false);

  async function detachChild(
    parentCategoryId: number,
    childCategoryId: number,
  ) {
    isDetachChildLoading.value = true;
    detachChildResult.value = await categoryApi.detachChild(
      parentCategoryId,
      childCategoryId,
    );
    isDetachChildLoading.value = false;
  }

  return {
    // list
    listCategoryResult,
    isCategoryListLoading,
    loadCategoryList,

    // by id
    byCategoryIdResult,
    isByCategoryIdLoading,
    loadByCategoryId,

    // create
    createdCategoryResult,
    isCategoryCreatedLoading,
    createCategory,

    // update
    updatedCategoryResult,
    isUpdatedCategoryLoading,
    updateCategory,

    // toggle
    patchToggleCategoryStatusResult,
    isPatchToggleCategoryStatusLoading,
    toggleCategoryStatus,

    // roots
    rootsResult,
    isRootsLoading,
    loadRoots,

    // children
    childrenResult,
    isChildrenLoading,
    loadChildren,

    // attach / detach
    attachChildResult,
    isAttachChildLoading,
    attachChild,

    detachChildResult,
    isDetachChildLoading,
    detachChild,
  };
}
