#!/bin/bash

# Upload seed images to MinIO
# Automatically discovers and uploads all subfolders (products/, categories/, etc.)
# Requires: MinIO running, mc CLI installed or Docker available

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

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

# Discover all image directories (top-level folders under seed-images/)
UPLOAD_DIRS=()
for dir in "$SCRIPT_DIR"/*/; do
    [ -d "$dir" ] && UPLOAD_DIRS+=("$dir")
done

if [ ${#UPLOAD_DIRS[@]} -eq 0 ]; then
    echo "Error: No subdirectories found in $SCRIPT_DIR"
    echo "Please create the directory structure as described in README.md"
    echo ""
    echo "Expected structure:"
    echo "  seed-images/products/1001/smartphone.webp"
    echo "  seed-images/categories/1/electronics.webp"
    echo "  ..."
    exit 1
fi

# Count all images across all directories
IMAGE_COUNT=$(find "${UPLOAD_DIRS[@]}" -type f \( -name "*.webp" -o -name "*.jpg" -o -name "*.jpeg" -o -name "*.png" -o -name "*.gif" \) 2>/dev/null | wc -l)

if [ "$IMAGE_COUNT" -eq 0 ]; then
    echo "No images found in any subdirectory"
    echo ""
    echo "Expected structure:"
    echo "  seed-images/products/1001/smartphone.webp"
    echo "  seed-images/categories/1/electronics.webp"
    echo "  ..."
    echo ""
    echo "See README.md for full list of expected images."
    exit 1
fi

# List what will be uploaded
echo "Found $IMAGE_COUNT image(s) across directories:"
for dir in "${UPLOAD_DIRS[@]}"; do
    dirname=$(basename "$dir")
    count=$(find "$dir" -type f \( -name "*.webp" -o -name "*.jpg" -o -name "*.jpeg" -o -name "*.png" -o -name "*.gif" \) 2>/dev/null | wc -l)
    echo "  $dirname/ — $count image(s)"
done
echo ""

# Try using mc CLI directly, fall back to Docker
if command -v mc &> /dev/null; then
    echo "Using local mc CLI..."

    # Configure alias
    mc alias set seedupload "$MINIO_ENDPOINT" "$MINIO_ACCESS_KEY" "$MINIO_SECRET_KEY" --api S3v4

    # Upload each directory, preserving folder structure
    for dir in "${UPLOAD_DIRS[@]}"; do
        dirname=$(basename "$dir")
        echo "Uploading $dirname/..."
        mc cp --recursive "$dir" "seedupload/$BUCKET/$dirname/"
    done

    echo ""
    echo "Upload complete!"

else
    echo "mc CLI not found, using Docker..."

    # Build volume mounts and upload commands for all directories
    VOLUME_MOUNTS=""
    UPLOAD_CMDS=""
    for dir in "${UPLOAD_DIRS[@]}"; do
        dirname=$(basename "$dir")
        VOLUME_MOUNTS="$VOLUME_MOUNTS -v $dir:/seed/$dirname:ro"
        UPLOAD_CMDS="$UPLOAD_CMDS mc cp --recursive /seed/$dirname/ seedupload/$BUCKET/$dirname/ &&"
    done
    # Remove trailing ' &&'
    UPLOAD_CMDS="${UPLOAD_CMDS% &&}"

    # Note: minio/mc has 'mc' as entrypoint, so we override with --entrypoint
    # Also need to use host network to reach localhost:9000
    eval docker run --rm \
        --network host \
        --entrypoint /bin/sh \
        $VOLUME_MOUNTS \
        minio/mc:latest \
        -c "\"mc alias set seedupload $MINIO_ENDPOINT $MINIO_ACCESS_KEY $MINIO_SECRET_KEY --api S3v4 && $UPLOAD_CMDS\""

    echo ""
    echo "Upload complete!"
fi

echo ""
echo "Verify at: http://localhost:9001 (MinIO Console)"
echo "Or via API: $MINIO_ENDPOINT/$BUCKET/"
