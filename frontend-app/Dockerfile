FROM node:18-alpine AS base

RUN npm i -g pnpm

WORKDIR /app

COPY package*.json ./
COPY pnpm-lock.yaml ./

RUN --mount=type=cache,id=pnpm,target=/root/.pnpm-store \
    pnpm install

COPY . .

EXPOSE 3000

CMD ["npm", "run", "dev"]
