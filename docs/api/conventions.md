# API Conventions

## Base URL

| Environment | URL                            |
| ----------- | ------------------------------ |
| Development | http://localhost:8080          |
| Docker      | http://backend:8080 (internal) |

## RESTful Endpoints

### URL Patterns

```
GET    /api/{resource}              # List all
GET    /api/{resource}/{id}         # Get one
POST   /api/{resource}              # Create
PUT    /api/{resource}/{id}         # Update
PUT    /api/{resource}/toggle/{id}  # Update active status field of resource
DELETE /api/{resource}/{id}         # Delete
```

> Note: We only delete relations, everything else is soft-deleted.

### Current Endpoints

| Resource       | Endpoint                                                      | Methods      |
| -------------- | ------------------------------------------------------------- | ------------ |
| Health         | `/api/health`                                                 | GET          |
| Health         | `/api/health/db`                                              | GET          |
| Products       | `/api/product`                                                | GET, POST    |
| Products       | `/api/product/{productId}`                                    | GET, PUT     |
| Products       | `/api/product/toggle/{productId}`                             | PATCH        |
| Categories     | `/api/category`                                               | GET, POST    |
| Categories     | `/api/category/{categoryId}`                                  | GET, PUT     |
| Categories     | `/api/category/toggle/{categoryId}`                           | PATCH        |
| Categories     | `/api/category/roots`                                         | GET          |
| Categories     | `/api/category/{parentCategoryId}/children`                   | GET          |
| Categories     | `/api/category/{parentCategoryId}/children/{childCategoryId}` | POST, DELETE |
| Product Images | `/api/product/{productId}/images`                             | GET, POST    |
| Product Images | `/api/product/{productId}/images/reorder`                     | PUT          |
| Product Images | `/api/productimage/{id}`                                      | GET, DELETE  |
| Product Images | `/api/productimage/{id}/metadata`                             | GET          |
| Product Images | `/api/productimage/{id}/primary`                              | PUT          |

## Request Formats

### Content Type

All requests with body must use:

```
Content-Type: application/json
```

### Create Product Example

```http
POST /api/products
Content-Type: application/json

{
  "name": "Product Name",
  "description": "Product description",
  "price": 99.99,
  "categoryId": 1,
  "stockQuantity": 90
}
```

### Update Product Example

```http
PUT /api/products/1
Content-Type: application/json

{
  "name": "Updated Name",
  "description": "Updated description",
  "price": 149.99,
  "categoryId": 2,
  "stockQuantity": 70
}
```

## Response Formats

### Success Response (Single Item)

```json
{
  "id": 0,
  "categoryId": 0,
  "name": "string",
  "description": "string",
  "price": 0,
  "stockQuantity": 0,
  "isActive": true,
  "category": {
    "id": 0,
    "name": "string",
    "description": "string"
  },
  "images": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "productId": 0,
      "fileName": "string",
      "contentType": "string",
      "sizeBytes": 0,
      "displayOrder": 0,
      "isPrimary": true,
      "uploadedAt": "2026-01-29T04:59:03.053Z",
      "url": "string"
    }
  ],
  "primaryImageUrl": "string"
}
```

### Success Response (Collection)

```json
[
  {
    "id": 0,
    "categoryId": 0,
    "name": "string",
    "description": "string",
    "price": 0,
    "stockQuantity": 0,
    "isActive": true,
    "category": {
      "id": 0,
      "name": "string",
      "description": "string"
    },
    "images": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "productId": 0,
        "fileName": "string",
        "contentType": "string",
        "sizeBytes": 0,
        "displayOrder": 0,
        "isPrimary": true,
        "uploadedAt": "2026-01-29T04:59:41.889Z",
        "url": "string"
      }
    ],
    "primaryImageUrl": "string"
  }
]
```

### Error Response

```json
{
  "type": "string",
  "title": "string",
  "status": 0,
  "detail": "string",
  "instance": "string",
  "additionalProp1": "string",
  "additionalProp2": "string",
  "additionalProp3": "string"
}
```

## HTTP Status Codes

### Success Codes

| Code | Meaning    | Use Case         |
| ---- | ---------- | ---------------- |
| 200  | OK         | GET, PUT success |
| 201  | Created    | POST success     |
| 204  | No Content | DELETE success   |

### Client Error Codes

| Code | Meaning      | Use Case                          |
| ---- | ------------ | --------------------------------- |
| 400  | Bad Request  | Validation failed                 |
| 401  | Unauthorized | Auth required (TODO)              |
| 403  | Forbidden    | Permission denied (TODO)          |
| 404  | Not Found    | Resource doesn't exist            |
| 409  | Conflict     | Duplicate or constraint violation |

### Server Error Codes

| Code | Meaning               | Use Case            |
| ---- | --------------------- | ------------------- |
| 500  | Internal Server Error | Unhandled exception |

## Frontend ApiResult Pattern

All API services wrap responses in `ApiResult<T>`:

```typescript
// @/lib/http.ts
export type ApiResult<T> = ApiOk<T> | ApiFail;
```

### Service Implementation

```typescript
// services/productsApi.ts
export const productsApi = {
  getProducts(query?: ProductListQuery) {
    return api.get<ProductMap>(apiRoutes.products.list, { query });
  },

  postProduct(request: ProductRequest) {
    return api.post<void>(apiRoutes.products.create, request);
  },
};
```

> Note: Error handling happens at our local @/lib/http.ts file

### Composable Usage

```typescript
// composables/useProducts.ts
export function useProducts() {
  const listProductResult = ref<ApiResult<ProductMap> | null>(null);
  const isProductListLoading = ref(false);
  async function loadProductList(query?: ProductListQuery) {
    isProductListLoading.value = true;
    listProductResult.value = await productsApi.getProducts(query);
    isProductListLoading.value = false;
  }

  return { listProductResult, isProductListLoading, loadProductList };
}
```

## File Upload (Product Images)

### Endpoint

```http
POST /api/product/{productId}/images
Content-Type: multipart/form-data
```

> Form field name: file <br>
> Max file size: 10 MB <br>
> Allowed content types: image/jpeg, image/png, image/webp, image/gif

### Request

```typescript
// services/productImageApi.ts
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
```

### Response

```json
{
  "id": "3f4c1b4b-2f7b-4a1c-9c0a-7f9e2f3a1c11",
  "productId": 1001,
  "fileName": "iphone.webp",
  "contentType": "image/webp",
  "sizeBytes": 50000,
  "displayOrder": 1,
  "isPrimary": false,
  "uploadedAt": "2026-01-01T00:00:00Z",
  "url": "/api/productimage/3f4c1b4b-2f7b-4a1c-9c0a-7f9e2f3a1c11"
}
```

## Pagination (TODO)

When implemented, paginated endpoints will use:

```
GET /api/products?page=1&pageSize=20
```

Response:

```json
{
  "items": [...],
  "page": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8
}
```

## API Documentation

Swagger UI available at:

- http://localhost:8080/swagger

OpenAPI specification at:

- http://localhost:8080/swagger/v1/swagger.json
