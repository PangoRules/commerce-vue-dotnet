import { ref } from "vue";
import type { ApiResult } from "@/lib/http";
import type { ProductImageResponse } from "@/types/api/productTypes";
import { productImageApi } from "@/services/productImageApi";

export function useProductImages() {
  // List images for a product
  const imagesResult = ref<ApiResult<ProductImageResponse[]> | null>(null);
  const isImagesLoading = ref(false);

  async function loadImages(productId: number) {
    isImagesLoading.value = true;
    imagesResult.value = await productImageApi.getImages(productId);
    isImagesLoading.value = false;
  }

  // Get image metadata
  const imageMetadataResult = ref<ApiResult<ProductImageResponse> | null>(null);
  const isImageMetadataLoading = ref(false);

  async function loadImageMetadata(imageId: string) {
    isImageMetadataLoading.value = true;
    imageMetadataResult.value = await productImageApi.getImageMetadata(imageId);
    isImageMetadataLoading.value = false;
  }

  // Upload image
  const uploadResult = ref<ApiResult<ProductImageResponse> | null>(null);
  const isUploading = ref(false);

  async function uploadImage(productId: number, file: File) {
    isUploading.value = true;
    uploadResult.value = await productImageApi.uploadImage(productId, file);
    isUploading.value = false;
    return uploadResult.value;
  }

  // Delete image
  const deleteResult = ref<ApiResult<void> | null>(null);
  const isDeleting = ref(false);

  async function deleteImage(imageId: string) {
    isDeleting.value = true;
    deleteResult.value = await productImageApi.deleteImage(imageId);
    isDeleting.value = false;
    return deleteResult.value;
  }

  // Set primary
  const setPrimaryResult = ref<ApiResult<void> | null>(null);
  const isSettingPrimary = ref(false);

  async function setPrimary(imageId: string) {
    isSettingPrimary.value = true;
    setPrimaryResult.value = await productImageApi.setPrimary(imageId);
    isSettingPrimary.value = false;
    return setPrimaryResult.value;
  }

  // Reorder images
  const reorderResult = ref<ApiResult<void> | null>(null);
  const isReordering = ref(false);

  async function reorderImages(productId: number, imageIds: string[]) {
    isReordering.value = true;
    reorderResult.value = await productImageApi.reorderImages(productId, imageIds);
    isReordering.value = false;
    return reorderResult.value;
  }

  // Helper to get image URL (for use in templates)
  function getImageUrl(imageId: string) {
    return productImageApi.getImageUrl(imageId);
  }

  return {
    // List
    imagesResult,
    isImagesLoading,
    loadImages,

    // Metadata
    imageMetadataResult,
    isImageMetadataLoading,
    loadImageMetadata,

    // Upload
    uploadResult,
    isUploading,
    uploadImage,

    // Delete
    deleteResult,
    isDeleting,
    deleteImage,

    // Set primary
    setPrimaryResult,
    isSettingPrimary,
    setPrimary,

    // Reorder
    reorderResult,
    isReordering,
    reorderImages,

    // Helper
    getImageUrl,
  };
}
