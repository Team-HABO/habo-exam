FROM node:20-bullseye-slim

WORKDIR /app

COPY package*.json ./
RUN npm install --ignore-scripts

COPY . .

RUN npx prisma generate
RUN npm run generate && npx tsc

CMD sh -c "npx prisma migrate deploy && npx prisma db seed && npx tsx src/server.ts"
