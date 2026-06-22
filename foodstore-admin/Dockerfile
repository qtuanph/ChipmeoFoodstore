# ============================================
# Stage 1: Install dependencies
# ============================================
FROM node:alpine AS deps
WORKDIR /app
COPY package*.json ./
RUN --mount=type=cache,id=npm-admin,target=/root/.npm \
    npm install --legacy-peer-deps --no-audit --no-fund

# ============================================
# Stage 2: Build
# ============================================
FROM node:alpine AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1
ARG API_PROXY_URL=http://api:8080/v2
ENV API_PROXY_URL=${API_PROXY_URL}
RUN npm run build

# ============================================
# Stage 3: Runtime
# ============================================
FROM node:alpine AS runtime
WORKDIR /app
ENV NODE_ENV=production
ENV HOST=0.0.0.0
ENV PORT=3000
RUN addgroup --system --gid 1001 nodejs && \
    adduser --system --uid 1001 nextjs

COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static
COPY --from=builder --chown=nextjs:nodejs /app/public ./public
USER nextjs
EXPOSE 3000
CMD ["node", "server.js"]