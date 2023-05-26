FROM node:18-alpine AS base

RUN npm i -g pnpm

WORKDIR /app

COPY package*.json ./

RUN pnpm install

COPY . .

EXPOSE 3000

CMD ["npm", "run", "dev"]
