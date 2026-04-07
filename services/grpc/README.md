# gRPC Library Service

A gRPC service for the library domain — fetch books, create new ones, and subscribe to live updates when books are added. Built with Python and grpcio, using SQLite as the database.

---

## What is gRPC?

**gRPC** (Google Remote Procedure Call) is a high-performance framework for communication between services. Unlike REST (JSON over HTTP) or SOAP (XML over HTTP), gRPC:

- Uses **Protocol Buffers (protobuf)** for compact binary serialization — smaller and faster than JSON/XML
- Defines its API contract in a **`.proto` file**, which is used to generate server and client code in any language
- Supports **streaming** — the server can push multiple messages to the client over a single connection
- Runs on top of **HTTP/2**, enabling multiplexed connections

### Key concepts

| Term | What it means |
|------|--------------|
| **`.proto` file** | Defines the service contract — messages (data shapes) and RPCs (operations). Both server and client code are generated from this. |
| **Unary RPC** | Standard request-response — client sends one message, server replies with one message. |
| **Server streaming RPC** | Client sends one request, server sends back a stream of messages over time. |
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
grpc/
├── .devcontainer/
│   ├── devcontainer.json     # VS Code dev container config
│   └── docker-compose.yml    # Container definition
├── proto/
│   └── library.proto         # Service contract — defines RPCs and messages
├── src/
│   ├── server.py             # Entry point — starts the gRPC server
│   ├── services/
│   │   └── library_service.py # RPC handler implementations
│   ├── db/
│   │   └── database.py       # SQLite connection helper
│   └── generated/            # Auto-generated protobuf/gRPC code (do not edit)
│       ├── library_pb2.py
│       └── library_pb2_grpc.py
├── data/
│   └── library.db            # SQLite database
├── Dockerfile
├── requirements.txt
└── README.md
```

---

## Getting Started

### Prerequisites

- Docker

### Run with Dev Container

1. Open VS Code and run **Dev Containers: Reopen in Container** (select the gRPC Service container).

2. Dependencies are installed automatically via `postCreateCommand`.

3. Generate protobuf code (only needed once, or after changing the `.proto` file):

```bash
python -m grpc_tools.protoc \
  -I./proto \
  --python_out=./src/generated \
  --grpc_python_out=./src/generated \
  ./proto/library.proto
```

4. Fix the generated import (protoc generates flat imports):

```bash
sed -i 's/import library_pb2/from src.generated import library_pb2/' ./src/generated/library_pb2_grpc.py
```

5. Start the server:

```bash
python -m src.server
```

Server listens on `0.0.0.0:50051`.

---

## Available RPCs

| RPC | Type | Description |
|-----|------|-------------|
| `GetBookById` | Unary | Fetch a book by its ID. Returns `NOT_FOUND` if it doesn't exist. |
| `CreateBook` | Unary | Create a new book. Validates author/publisher exist and year >= 1900. Returns the generated ID. |
| `WatchBooks` | Server streaming | Subscribe to live updates — receives a `Book` message every time any client creates a new book. |

### GetBookById

```json
// Request
{ "book_id": 1000 }

// Response
{
  "book_id": 1000,
  "title": "Harry Potter and the Sorcerer's Stone (Book 1)",
  "author_id": 394,
  "publishing_company_id": 74,
  "publishing_year": 1997
}
```

### CreateBook

```json
// Request
{
  "title": "Test Book",
  "author_id": 1,
  "publishing_company_id": 1,
  "publishing_year": 2024
}

// Response
{ "book_id": 2002 }
```

### WatchBooks

```json
// Request
{}

// Response (streamed — one message per new book created by any client)
{ "book_id": 2002, "title": "Test Book", "author_id": 1, ... }
```

---

## Error Handling

gRPC uses **status codes** instead of HTTP status codes:

| Status Code | When it is returned |
|-------------|-------------------|
| `NOT_FOUND` | Book ID does not exist (GetBookById) |
| `INVALID_ARGUMENT` | Missing/invalid fields, nonexistent author or publisher, year < 1900 (CreateBook) |

---

## Testing

### Postman

1. Open Postman → **New** → **gRPC**
2. Server URL: `localhost:50051`
3. Import `proto/library.proto`
4. Select an RPC and send a request

### Database

The SQLite database contains 998 books (IDs 1000–1999), authors, and publishing companies.
