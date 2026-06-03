FROM node:24-alpine

WORKDIR /app

COPY package.json package-lock.json* ./
# --ignore-scripts prevents the `prepare` hook (prisma migrate/seed) from running
# at build time when there is no database available
RUN npm install --ignore-scripts

COPY prisma.config.ts tsconfig.json codegen.ts ./
COPY prisma ./prisma
COPY src ./src

# DATABASE_URL must be present for prisma.config.ts at build time
ARG DATABASE_URL=postgresql://postgres:secret@postgres:5432/graph
ENV DATABASE_URL=${DATABASE_URL}

RUN npx prisma generate

EXPOSE 4000

# At startup: apply migrations, seed, then compile and start
CMD ["sh", "-c", "npx prisma migrate deploy && npx prisma db seed && npm run start"]
