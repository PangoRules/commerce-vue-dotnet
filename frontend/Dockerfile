# ---- Base image ----
FROM node:20-alpine

# ---- Set working directory ----
WORKDIR /app

# ---- Copy package files first (better caching) ----
COPY package*.json ./

# ---- Install dependencies ----
RUN npm install

# ---- Copy the rest of the app ----
COPY . .

# ---- Vite dev server port ----
EXPOSE 5173

# ---- Run Vite in dev mode ----
CMD ["npm", "run", "dev", "--", "--host"]
