#!/bin/bash

# Upload seed images to MinIO
# Requires: MinIO running, mc CLI installed or Docker available

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PRODUCTS_DIR="$SCRIPT_DIR/products"

# MinIO configuration
# Port 9000 = S3 API (for mc), Port 9001 = Web Console (for browser)
MINIO_ENDPOINT="${MINIO_ENDPOINT:-http://localhost:9000}"
MINIO_ACCESS_KEY="${MINIO_ROOT_USER:-minioadmin}"
MINIO_SECRET_KEY="${MINIO_ROOT_PASSWORD:-minioadmin123}"
BUCKET="${MINIO_BUCKET:-commerce-assets}"

echo "=== MinIO Seed Image Uploader ==="
echo "Endpoint: $MINIO_ENDPOINT"
echo "Bucket: $BUCKET"
echo ""

# Check if products directory exists and has content
if [ ! -d "$PRODUCTS_DIR" ]; then
    echo "Error: Products directory not found at $PRODUCTS_DIR"
    echo "Please create the directory structure as described in README.md"
    exit 1
fi

# Count images
IMAGE_COUNT=$(find "$PRODUCTS_DIR" -type f \( -name "*.webp" -o -name "*.jpg" -o -name "*.jpeg" -o -name "*.png" \) 2>/dev/null | wc -l)

if [ "$IMAGE_COUNT" -eq 0 ]; then
    echo "No images found in $PRODUCTS_DIR"
    echo ""
    echo "Expected structure:"
    echo "  products/1001/smartphone.webp"
    echo "  products/1002/headphones.webp"
    echo "  ..."
    echo ""
    echo "See README.md for full list of expected images."
    exit 1
fi

echo "Found $IMAGE_COUNT image(s) to upload"
echo ""

# Try using mc CLI directly, fall back to Docker
if command -v mc &> /dev/null; then
    echo "Using local mc CLI..."

    # Configure alias
    mc alias set seedupload "$MINIO_ENDPOINT" "$MINIO_ACCESS_KEY" "$MINIO_SECRET_KEY" --api S3v4

    # Upload
    mc cp --recursive "$PRODUCTS_DIR/" "seedupload/$BUCKET/products/"

    echo ""
    echo "Upload complete!"

else
    echo "mc CLI not found, using Docker..."

    # Note: minio/mc has 'mc' as entrypoint, so we override with --entrypoint
    # Also need to use host network to reach localhost:9000
    docker run --rm \
        --network host \
        --entrypoint /bin/sh \
        -v "$PRODUCTS_DIR:/products:ro" \
        minio/mc:latest \
        -c "
            mc alias set seedupload $MINIO_ENDPOINT $MINIO_ACCESS_KEY $MINIO_SECRET_KEY --api S3v4 &&
            mc cp --recursive /products/ seedupload/$BUCKET/products/
        "

    echo ""
    echo "Upload complete!"
fi

echo ""
echo "Verify at: http://localhost:9001 (MinIO Console)"
echo "Or via API: $MINIO_ENDPOINT/$BUCKET/products/"
