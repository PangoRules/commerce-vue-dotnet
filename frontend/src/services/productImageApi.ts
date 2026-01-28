import { api } from "@/services/apiClient";
import { apiRoutes } from "@/config/apiRoutes";
import type {
  ProductImageResponse,
  ReorderImagesRequest,
} from "@/types/api/productTypes";

export const productImageApi = {
  /**
   * Get all images for a product
   */
  getImages(productId: number) {
    return api.get<ProductImageResponse[]>(apiRoutes.productImages.list(productId));
  },

  /**
   * Get image metadata by ID
   */
  getImageMetadata(imageId: string) {
    return api.get<ProductImageResponse>(apiRoutes.productImages.metadata(imageId));
  },

  /**
   * Get the URL for an image (for use in img src)
   * This returns the proxy URL, not a fetch - use directly in img tags
   */
  getImageUrl(imageId: string) {
    return apiRoutes.productImages.get(imageId);
  },

  /**
   * Upload a new image for a product
   */
  uploadImage(productId: number, file: File) {
    const formData = new FormData();
    formData.append("file", file);

    return api.post<ProductImageResponse>(
      apiRoutes.productImages.upload(productId),
      formData,
      {
        headers: {
          // Let browser set Content-Type with boundary for multipart
          "Content-Type": undefined as unknown as string,
        },
      }
    );
  },

  /**
   * Delete an image
   */
  deleteImage(imageId: string) {
    return api.del<void>(apiRoutes.productImages.delete(imageId));
  },

  /**
   * Set an image as the primary image for its product
   */
  setPrimary(imageId: string) {
    return api.put<void>(apiRoutes.productImages.setPrimary(imageId));
  },

  /**
   * Reorder images for a product
   */
  reorderImages(productId: number, imageIds: string[]) {
    const request: ReorderImagesRequest = { imageIds };
    return api.put<void>(apiRoutes.productImages.reorder(productId), request);
  },
};
