# Seed Images for Products

This folder contains default product images that will be uploaded to MinIO for development.

## Directory Structure

Place your images following this structure:

```
seed-images/products/
├── 1001/smartphone.webp       # Electronics - Smartphone
├── 1002/headphones.webp       # Electronics - Wireless Headphones
├── 1003/laptop.webp           # Electronics - Laptop
├── 2001/clean-code.webp       # Books - Clean Code
├── 2002/pragmatic-programmer.webp  # Books - The Pragmatic Programmer
├── 2003/design-patterns.webp  # Books - Design Patterns
├── 3001/air-fryer.webp        # Home & Kitchen - Air Fryer
├── 3002/blender.webp          # Home & Kitchen - Blender
├── 3003/coffee-maker.webp     # Home & Kitchen - Coffee Maker
├── 4001/tshirt.webp           # Clothing - Men's T-Shirt
├── 4002/jeans.webp            # Clothing - Women's Jeans
├── 4003/hoodie.webp           # Clothing - Hoodie
├── 5001/yoga-mat.webp         # Sports - Yoga Mat
├── 5002/dumbbell-set.webp     # Sports - Dumbbell Set
└── 5003/camping-tent.webp     # Sports - Camping Tent
```

## Getting Sample Images

You can use any of these sources for free product images:

1. **Unsplash** (https://unsplash.com) - High quality, free to use
2. **Pexels** (https://pexels.com) - Free stock photos
3. **Placeholder images** - Use a service like https://placehold.co/400x400.webp

## Upload to MinIO

### Option 1: Using the upload script

```bash
# Make sure MinIO is running
docker compose --profile infra up -d

# Run the upload script
./seed-images/upload-to-minio.sh
```

### Option 2: Using MinIO Console

1. Open http://localhost:9001
2. Login with your credentials (default: minioadmin/minioadmin)
3. Navigate to the `commerce-assets` bucket
4. Create the `products` folder
5. Upload images to their respective product folders

### Option 3: Using mc CLI

```bash
# Configure mc alias
mc alias set local http://localhost:9000 minioadmin minioadmin

# Upload all images
mc cp --recursive seed-images/products/ local/commerce-assets/products/
```

## Image Recommendations

- **Format**: WebP preferred (smaller size, good quality), JPEG/PNG also supported
- **Size**: 400x400 to 800x800 pixels recommended
- **File size**: Keep under 500KB for fast loading
