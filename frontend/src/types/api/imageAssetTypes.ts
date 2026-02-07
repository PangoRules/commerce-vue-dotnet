export type ImageAssetResponse = {
  id: string;
  type: "Product" | "Category";
  ownerId: number;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  displayOrder: number;
  isPrimary: boolean;
  uploadedAt: string;
  url: string;
};

export type ReorderImagesRequest = {
  imageIds: string[];
};
