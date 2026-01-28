// src/composables/__tests__/useCategories.spec.ts
// @vitest-environment jsdom

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
    const res: ApiResult<CategoryResponse[]> = httpOk([]);
    vi.mocked(categoryApi.getCategories).mockResolvedValue(res);

    const sut = useCategories();

    const p = sut.loadCategoryList({ page: 1, pageSize: 10 });
    expect(sut.isCategoryListLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.getCategories).toHaveBeenCalledWith({
      page: 1,
      pageSize: 10,
    });
    expect(sut.listCategoryResult.value).toEqual(res);
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

    const res: ApiResult<CategoryAdminDetailsResponse> = httpOk(details);
    vi.mocked(categoryApi.getCategoryById).mockResolvedValue(res);

    const sut = useCategories();

    const p = sut.loadByCategoryId(1);
    expect(sut.isByCategoryIdLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.getCategoryById).toHaveBeenCalledWith(1);
    expect(sut.byCategoryIdResult.value).toEqual(res);
    expect(sut.isByCategoryIdLoading.value).toBe(false);
  });

  it("createCategory sets loading and stores result", async () => {
    const res: ApiResult<void> = httpOk(undefined, 201);
    vi.mocked(categoryApi.postCategory).mockResolvedValue(res);

    const sut = useCategories();
    const payload: CategoryRequest = {
      name: "New",
      description: "Desc",
      parentCategoryIds: [1],
    };

    const p = sut.createCategory(payload);
    expect(sut.isCategoryCreatedLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.postCategory).toHaveBeenCalledWith(payload);
    expect(sut.createdCategoryResult.value).toEqual(res);
    expect(sut.isCategoryCreatedLoading.value).toBe(false);
  });

  it("updateCategory sets loading and stores result", async () => {
    const res: ApiResult<void> = httpOk(undefined, 204);
    vi.mocked(categoryApi.putCategory).mockResolvedValue(res);

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
    expect(sut.updatedCategoryResult.value).toEqual(res);
    expect(sut.isUpdatedCategoryLoading.value).toBe(false);
  });

  it("toggleCategoryStatus sets loading and stores result", async () => {
    const res: ApiResult<void> = httpOk(undefined, 204);
    vi.mocked(categoryApi.toggleCategory).mockResolvedValue(res);

    const sut = useCategories();

    const p = sut.toggleCategoryStatus(10);
    expect(sut.isPatchToggleCategoryStatusLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.toggleCategory).toHaveBeenCalledWith(10);
    expect(sut.patchToggleCategoryStatusResult.value).toEqual(res);
    expect(sut.isPatchToggleCategoryStatusLoading.value).toBe(false);
  });

  it("loadRoots sets loading and stores result", async () => {
    const res: ApiResult<CategoryResponse[]> = httpOk([]);
    vi.mocked(categoryApi.getRoots).mockResolvedValue(res);

    const sut = useCategories();

    const p = sut.loadRoots(true);
    expect(sut.isRootsLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.getRoots).toHaveBeenCalledWith(true);
    expect(sut.rootsResult.value).toEqual(res);
    expect(sut.isRootsLoading.value).toBe(false);
  });

  it("loadChildren sets loading and stores result", async () => {
    const res: ApiResult<CategoryResponse[]> = httpOk([]);
    vi.mocked(categoryApi.getChildren).mockResolvedValue(res);

    const sut = useCategories();

    const p = sut.loadChildren(5, true);
    expect(sut.isChildrenLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.getChildren).toHaveBeenCalledWith(5, true);
    expect(sut.childrenResult.value).toEqual(res);
    expect(sut.isChildrenLoading.value).toBe(false);
  });

  it("attachChild sets loading and stores result", async () => {
    const res: ApiResult<void> = httpOk(undefined, 204);
    vi.mocked(categoryApi.attachChild).mockResolvedValue(res);

    const sut = useCategories();

    const p = sut.attachChild(1, 99);
    expect(sut.isAttachChildLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.attachChild).toHaveBeenCalledWith(1, 99);
    expect(sut.attachChildResult.value).toEqual(res);
    expect(sut.isAttachChildLoading.value).toBe(false);
  });

  it("detachChild sets loading and stores result", async () => {
    const res: ApiResult<void> = httpOk(undefined, 204);
    vi.mocked(categoryApi.detachChild).mockResolvedValue(res);

    const sut = useCategories();

    const p = sut.detachChild(1, 99);
    expect(sut.isDetachChildLoading.value).toBe(true);

    await p;
    await nextTick();

    expect(categoryApi.detachChild).toHaveBeenCalledWith(1, 99);
    expect(sut.detachChildResult.value).toEqual(res);
    expect(sut.isDetachChildLoading.value).toBe(false);
  });

  it("stores a failure result too (example)", async () => {
    const res: ApiResult<CategoryResponse[]> = httpFail(500, "Server died");
    vi.mocked(categoryApi.getCategories).mockResolvedValue(res);

    const sut = useCategories();

    await sut.loadCategoryList();
    await nextTick();

    expect(sut.listCategoryResult.value).toEqual(res);
    expect(sut.isCategoryListLoading.value).toBe(false);
  });
});
