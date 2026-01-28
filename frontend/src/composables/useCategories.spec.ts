import { describe, it, expect, vi, beforeEach } from "vitest";
import { nextTick } from "vue";

import { useCategories } from "@/composables/useCategories";
import { categoryApi } from "@/services/categoryApi";
import type { ApiResult } from "@/lib/http";
import type {
  CategoryAdminDetailsResponse,
  CategoryRequest,
  CategoryResponse,
} from "@/types/api/categoryTypes";
import { httpFail, httpOk } from "@/tests/helpers/apiResult";

vi.mock("@/services/categoryApi", () => ({
  categoryApi: {
    getCategories: vi.fn(),
    getCategoryById: vi.fn(),
    postCategory: vi.fn(),
    putCategory: vi.fn(),
    toggleCategory: vi.fn(),
    getRoots: vi.fn(),
    getChildren: vi.fn(),
    attachChild: vi.fn(),
    detachChild: vi.fn(),
  },
}));

describe("useCategories", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("loadCategoryList sets loading and stores result", async () => {
    const fakeResult: ApiResult<CategoryResponse[]> = httpOk([], 200);
    vi.mocked(categoryApi.getCategories).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const p = sut.loadCategoryList({ page: 1, pageSize: 10 });
    expect(sut.isCategoryListLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.getCategories).toHaveBeenCalledWith({
      page: 1,
      pageSize: 10,
    });
    expect(sut.listCategoryResult.value).toStrictEqual(fakeResult);
    expect(sut.isCategoryListLoading.value).toBe(false);
  });

  it("loadByCategoryId sets loading and stores result", async () => {
    const details: CategoryAdminDetailsResponse = {
      id: 1,
      name: "Electronics",
      description: null,
      isActive: true,
      parents: [],
      children: [],
    };

    const fakeResult: ApiResult<CategoryAdminDetailsResponse> = httpOk(
      details,
      200,
    );
    vi.mocked(categoryApi.getCategoryById).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const p = sut.loadByCategoryId(1);
    expect(sut.isByCategoryIdLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.getCategoryById).toHaveBeenCalledWith(1);
    expect(sut.byCategoryIdResult.value).toStrictEqual(fakeResult);
    expect(sut.isByCategoryIdLoading.value).toBe(false);
  });

  it("createCategory sets loading and stores result", async () => {
    const fakeResult: ApiResult<void> = httpOk(undefined, 201);
    vi.mocked(categoryApi.postCategory).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const payload: CategoryRequest = {
      name: "New Category",
      description: "Desc",
      parentCategoryIds: [1],
    };

    const p = sut.createCategory(payload);
    expect(sut.isCategoryCreatedLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.postCategory).toHaveBeenCalledWith(payload);
    expect(sut.createdCategoryResult.value).toStrictEqual(fakeResult);
    expect(sut.isCategoryCreatedLoading.value).toBe(false);
  });

  it("updateCategory sets loading and stores result", async () => {
    const fakeResult: ApiResult<void> = httpOk(undefined, 204);
    vi.mocked(categoryApi.putCategory).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const payload: CategoryRequest = {
      name: "Updated",
      description: "Updated desc",
      parentCategoryIds: [],
    };

    const p = sut.updateCategory(10, payload);
    expect(sut.isUpdatedCategoryLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.putCategory).toHaveBeenCalledWith(10, payload);
    expect(sut.updatedCategoryResult.value).toStrictEqual(fakeResult);
    expect(sut.isUpdatedCategoryLoading.value).toBe(false);
  });

  it("toggleCategoryStatus sets loading and stores result", async () => {
    const fakeResult: ApiResult<void> = httpOk(undefined, 204);
    vi.mocked(categoryApi.toggleCategory).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const p = sut.toggleCategoryStatus(10);
    expect(sut.isPatchToggleCategoryStatusLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.toggleCategory).toHaveBeenCalledWith(10);
    expect(sut.patchToggleCategoryStatusResult.value).toStrictEqual(fakeResult);
    expect(sut.isPatchToggleCategoryStatusLoading.value).toBe(false);
  });

  it("loadRoots sets loading and stores result", async () => {
    const fakeResult: ApiResult<CategoryResponse[]> = httpOk([], 200);
    vi.mocked(categoryApi.getRoots).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const p = sut.loadRoots(true);
    expect(sut.isRootsLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.getRoots).toHaveBeenCalledWith(true);
    expect(sut.rootsResult.value).toStrictEqual(fakeResult);
    expect(sut.isRootsLoading.value).toBe(false);
  });

  it("loadChildren sets loading and stores result", async () => {
    const fakeResult: ApiResult<CategoryResponse[]> = httpOk([], 200);
    vi.mocked(categoryApi.getChildren).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const p = sut.loadChildren(5, true);
    expect(sut.isChildrenLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.getChildren).toHaveBeenCalledWith(5, true);
    expect(sut.childrenResult.value).toStrictEqual(fakeResult);
    expect(sut.isChildrenLoading.value).toBe(false);
  });

  it("attachChild sets loading and stores result", async () => {
    const fakeResult: ApiResult<void> = httpOk(undefined, 204);
    vi.mocked(categoryApi.attachChild).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const p = sut.attachChild(1, 99);
    expect(sut.isAttachChildLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.attachChild).toHaveBeenCalledWith(1, 99);
    expect(sut.attachChildResult.value).toStrictEqual(fakeResult);
    expect(sut.isAttachChildLoading.value).toBe(false);
  });

  it("detachChild sets loading and stores result", async () => {
    const fakeResult: ApiResult<void> = httpOk(undefined, 204);
    vi.mocked(categoryApi.detachChild).mockResolvedValue(fakeResult);

    const sut = useCategories();

    const p = sut.detachChild(1, 99);
    expect(sut.isDetachChildLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.detachChild).toHaveBeenCalledWith(1, 99);
    expect(sut.detachChildResult.value).toStrictEqual(fakeResult);
    expect(sut.isDetachChildLoading.value).toBe(false);
  });

  // optional: shows error is stored + loading resets
  it("stores a failure result and resets loading", async () => {
    const fakeResult: ApiResult<CategoryResponse[]> = httpFail(
      500,
      "Server sad",
    );
    vi.mocked(categoryApi.getCategories).mockResolvedValue(fakeResult);

    const sut = useCategories();

    await sut.loadCategoryList();
    await nextTick();

    expect(sut.listCategoryResult.value).toStrictEqual(fakeResult);
    expect(sut.isCategoryListLoading.value).toBe(false);
  });
});
