FROM node:alpine AS build
WORKDIR /app
COPY package*.json ./
RUN --mount=type=cache,id=npm-admin,target=/root/.npm \
    npm ci
COPY . .
RUN npm run build

FROM node:alpine AS runtime
WORKDIR /app
COPY --from=build /app/.next ./.next
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/package.json ./
COPY --from=build /app/public ./public
EXPOSE 3000
ENV NODE_ENV=production
ENV HOST=0.0.0.0
ENV PORT=3000
CMD ["node", "node_modules/.bin/next", "start"]
