FROM node:20-alpine

# better-sqlite3 requires native compilation
RUN apk add --no-cache python3 make g++

WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm install

COPY prisma ./prisma
COPY prisma.config.ts tsconfig.json ./

# DATABASE_URL must be set for prisma.config.ts to load without error at build time
ENV DATABASE_URL="file:./chat.db"
ENV PORT=3000

RUN npx prisma generate

COPY src ./src

EXPOSE 3000

CMD ["sh", "-c", "npx prisma migrate deploy && npx tsx src/index.ts"]
