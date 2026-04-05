# Build stage
FROM oven/bun:latest AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
COPY bun.lock ./

# Copy environment file (needed for PUBLIC_* variables during build)
COPY .env* ./

# Install dependencies
RUN bun install

# Copy source code
COPY . .

# Build the SvelteKit app
RUN bun run build

# Runtime stage
FROM oven/bun:alpine
WORKDIR /app

# Copy only necessary files from build stage
COPY --from=build /app/package*.json ./
COPY --from=build /app/build ./build
COPY --from=build /app/node_modules ./node_modules

# Set environment to production
ENV NODE_ENV=production

# Expose port (SvelteKit adapter serves on 3000 by default)
EXPOSE 3000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD bun -e "fetch('http://localhost:3000').then(r => {if (r.status !== 200) throw new Error(r.status)})"

# Start the app
CMD ["bun", "run", "build/index.js"]
