# gRPC Chat Service

A gRPC chat service — send messages and join a live bidirectional chat stream. Built with Python and grpcio, using SQLite as the database.

---

## What is gRPC?

**gRPC** (Google Remote Procedure Call) is a high-performance framework for communication between services. Unlike REST (JSON over HTTP) or SOAP (XML over HTTP), gRPC:

- Uses **Protocol Buffers (protobuf)** for compact binary serialization — smaller and faster than JSON/XML
- Defines its API contract in a **`.proto` file**, which is used to generate server and client code in any language
- Supports **streaming** — both client and server can send multiple messages over a single connection
- Runs on top of **HTTP/2**, enabling multiplexed connections

### Key concepts

| Term | What it means |
|------|--------------|
| **`.proto` file** | Defines the service contract — messages (data shapes) and RPCs (operations). Both server and client code are generated from this. |
| **Unary RPC** | Standard request-response — client sends one message, server replies with one message. |
| **Bidirectional streaming RPC** | Both client and server send streams of messages simultaneously over a single connection. |
| **Protobuf message** | A typed data structure defined in the `.proto` file. Serialized to compact binary on the wire. |
| **Status codes** | gRPC uses its own status codes (`NOT_FOUND`, `INVALID_ARGUMENT`, etc.) instead of HTTP status codes. |

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | Python 3.12 |
| gRPC framework | grpcio |
| Code generation | grpcio-tools (protoc compiler) |
| Database | SQLite (built-in `sqlite3` module) |

---

## Project Structure

```
grpc1/
├── .devcontainer/
│   ├── devcontainer.json     # VS Code dev container config
│   └── docker-compose.yml    # Container definition
├── proto/
│   └── chat.proto            # Service contract — defines RPCs and messages
├── src/
│   ├── server.py             # Entry point — starts the gRPC server
│   ├── services/
│   │   └── chat_service.py   # RPC handler implementations
│   ├── db/
│   │   └── database.py       # SQLite connection helper + table init
│   └── generated/            # Auto-generated protobuf/gRPC code (do not edit)
│       ├── chat_pb2.py
│       └── chat_pb2_grpc.py
├── data/
│   └── chat.db               # SQLite database (created at runtime)
├── Dockerfile
├── requirements.txt
└── README.md
```

---

## Getting Started

### Prerequisites

- Docker

### Run with Dev Container

1. Open VS Code and run **Dev Containers: Reopen in Container** (select the gRPC Chat Service container).

2. Dependencies are installed automatically via `postCreateCommand`, which also runs the protoc compiler.

3. If you need to regenerate protobuf code manually (after changing the `.proto` file):

```bash
python -m grpc_tools.protoc \
  -I./proto \
  --python_out=./src/generated \
  --grpc_python_out=./src/generated \
  ./proto/chat.proto
```

4. Fix the generated import (protoc generates flat imports):

```bash
sed -i 's/import chat_pb2/from src.generated import chat_pb2/' ./src/generated/chat_pb2_grpc.py
```

5. Start the server:

```bash
python -m src.server
```

Server listens on `0.0.0.0:50052`.

---

## Available RPCs

| RPC | Type | Description |
|-----|------|-------------|
| `SendMessage` | Unary | Store a single chat message. Validates non-empty username and content. Returns the generated ID and timestamp. |
| `Chat` | Bidirectional streaming | Client streams messages in; server broadcasts every message to **all** connected clients in real-time. |

### SendMessage

```json
// Request
{
  "username": "alice",
  "content": "Hello everyone!"
}

// Response
{
  "message_id": 1,
  "timestamp": "2026-05-20T10:00:00Z"
}
```

### Chat

```json
// Each message sent by the client (stream)
{
  "username": "alice",
  "content": "Hey everyone!"
}

// Response stream — one message broadcast per message sent by any connected client
{
  "message_id": 1,
  "username": "alice",
  "content": "Hey everyone!",
  "timestamp": "2026-05-20T10:00:00Z"
}
```

---

## Error Handling

gRPC uses **status codes** instead of HTTP status codes:

| Status Code | When it is returned |
|-------------|-------------------|
| `INVALID_ARGUMENT` | Empty username or empty content (SendMessage) |

---

## Testing

### Postman

1. Open Postman → **New** → **gRPC**
2. Server URL: `localhost:50052`
3. Import `proto/chat.proto`
4. Select an RPC and send a request

### Database Schema

```sql
CREATE TABLE tmessage (
    nMessageID INTEGER PRIMARY KEY AUTOINCREMENT,
    cUsername  TEXT NOT NULL,
    cContent   TEXT NOT NULL,
    cTimestamp TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%SZ', 'now'))
);
```
