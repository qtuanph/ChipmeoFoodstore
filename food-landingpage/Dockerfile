FROM node:alpine AS build
WORKDIR /app
COPY package*.json ./
RUN --mount=type=cache,id=npm-landing,target=/root/.npm \
    npm ci
COPY . .
RUN npm run build

FROM node:alpine AS runtime
WORKDIR /app
COPY --from=build /app/dist ./dist
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/package.json ./
EXPOSE 4321
ENV HOST=0.0.0.0
ENV PORT=4321
CMD ["npx", "astro", "preview", "--host", "0.0.0.0", "--port", "4321"]
