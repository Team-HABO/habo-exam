# WebSocket Chat Service

A backend-only WebSocket server built with Node.js, `ws`, Prisma, and SQLite.  
Clients can fetch stored messages (unary) and post new ones that are broadcast to every connected client (bidirectional streaming).

---

## Requirements

- Node.js ≥ 18
- npm

---

## Setup & Running

### 1. Install dependencies

```bash
npm install
```

### 2. Configure the database URL

Create a `.env` file in the project root:

```env
DATABASE_URL="file:./dev.db"
PORT=3000
```

### 3. Run database migrations

```bash
npm run db:migrate
```

This creates the SQLite database and the `Message` table.

### 4. Start the server

**Development (hot-reload):**

```bash
npm run dev
```

**Production:**

```bash
npm run build
npm start
```

The server listens on `ws://localhost:3000` (or the port set in `.env`).

---

## Available npm Scripts

| Script | Description |
|---|---|
| `npm run dev` | Start with `tsx` watch mode |
| `npm run build` | Compile TypeScript to `dist/` |
| `npm start` | Run compiled output |
| `npm run db:migrate` | Apply Prisma migrations |
| `npm run db:generate` | Regenerate Prisma client |
| `npm run db:studio` | Open Prisma Studio GUI |

---

## WebSocket Protocol

All messages are JSON with a `type` field and an optional `payload` field.

### Unary — `getMessages`

The client sends a single request and receives a single response containing all stored messages. Only the requesting client receives the response.

**Send:**
```json
{ "type": "getMessages" }
```

**Receive:**
```json
{
  "type": "messages",
  "data": [
    { "id": 1, "username": "alice", "content": "hello", "createdAt": "..." }
  ]
}
```

---

### Bidirectional Streaming — `sendMessage`

The client sends a message. The server persists it, then broadcasts it to **all** connected clients (including the sender). New clients can join at any time and will receive future broadcasts — demonstrating the bidirectional, streaming nature of the channel.

**Send:**
```json
{
  "type": "sendMessage",
  "payload": { "username": "alice", "content": "hello world" }
}
```

**All connected clients receive:**
```json
{
  "type": "newMessage",
  "data": { "id": 2, "username": "alice", "content": "hello world", "createdAt": "..." }
}
```

---

## Security

### SQL Injection Prevention

All database access goes through **Prisma ORM**, which uses parameterized queries exclusively. User-supplied strings (`username`, `content`) are never interpolated into raw SQL — they are always passed as bound parameters.

```ts
// messageHandlers.ts
await prisma.message.create({
  data: { username, content },  // parameterized — no raw SQL
});
```

### XSS Prevention

All string inputs are passed through the [`xss`](https://github.com/leizongmin/js-xss) library before being stored or broadcast. This strips or escapes HTML tags and JavaScript event attributes, preventing stored XSS attacks where a malicious payload could be replayed to other clients.

```ts
// sanitize.ts
import xss from 'xss';
export function sanitize(input: unknown): string {
  return xss((input as string).trim());
}
```

### CSRF Prevention

WebSockets are not subject to the same cookie-based CSRF as HTTP forms, but browsers still send an `Origin` header on WebSocket upgrade requests. The server validates this header in `verifyClient` and rejects any connection whose origin is not on the allowlist:

```ts
// index.ts
const ALLOWED_ORIGINS = ['http://localhost', 'http://127.0.0.1'];

verifyClient({ origin }) {
  return !origin || ALLOWED_ORIGINS.some((o) => origin.startsWith(o));
}
```

Connections from unknown origins (e.g. a malicious third-party website trying to use a victim's session) are dropped before the WebSocket handshake completes.

---

## System Integration

This service is the real-time communication layer in a larger microservice architecture. Integration happens at several points:

### 1. Client ↔ WebSocket Server (WebSocket / bidirectional streaming)

Frontend clients connect over the WebSocket protocol. This is the primary integration boundary — replacing a traditional HTTP request/response cycle with a persistent, full-duplex channel that enables server-initiated pushes (broadcasts).

### 2. WebSocket Server ↔ Database (Prisma + SQLite)

Incoming messages are persisted via **Prisma ORM** before being broadcast. This decouples message storage from delivery: clients that connect later can call `getMessages` to replay history, integrating the real-time channel with a durable store.

### 3. Broadcast Fan-out (server-to-all-clients)

When a message is saved, the server iterates over `wss.clients` and pushes the `newMessage` event to every open connection. This is the integration point between the unary write path (`sendMessage`) and the streaming read path all other clients are subscribed to.

### Communication Patterns Demonstrated

| Pattern | Where |
|---|---|
| Unary (request/response) | `getMessages` — one request, one response |
| Server-side streaming | `sendMessage` broadcast — one write triggers N pushes |
| Persistent bidirectional channel | WebSocket connection lifetime |
