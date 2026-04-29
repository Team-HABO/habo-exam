# Graph Service

A GraphQL API built with **Apollo Server** and **Prisma ORM** for managing books and tasks over a PostgreSQL database.

## Tech Stack

- [Apollo Server](https://www.apollographql.com/docs/apollo-server/) v5
- [Prisma](https://www.prisma.io/) v7 (PostgreSQL)
- [GraphQL Codegen](https://the-guild.dev/graphql/codegen) for TypeScript type generation
- TypeScript, Node.js

## Prerequisites

- Docker
- vscode

## Getting Started

Start your dev container by first filling out the `.env`

```bash
cp .devcontainer/.env-sample .devcontainer/.env
```

Open the folder in VS Code and run

```
Dev Containers: Reopen in Container
```

Once inside the container, run:

```bash
npm install
npm run prepare   # Run migrations, generate Prisma client, seed database
npm run start     # Generate types, compile, and start the server
```

The server starts on **http://localhost:4000**.

### Development

```bash
npm run watch
```

Runs code generation, type checking, and the server concurrently with live reload.

## Database Models

| Model    | Fields                     | Constraints                   |
| -------- | -------------------------- | ----------------------------- |
| **Book** | `id`, `title`, `author`    | Unique on (`title`, `author`) |
| **Task** | `id`, `title`, `completed` | Unique on `title`             |

## API

### Queries

| Query   | Arguments  | Return    |
| ------- | ---------- | --------- |
| `books` | —          | `[Book]!` |
| `book`  | `id: Int!` | `Book`    |
| `tasks` | —          | `[Task]!` |
| `task`  | `id: Int!` | `Task`    |

### Mutations

| Mutation     | Arguments                                           | Return                  |
| ------------ | --------------------------------------------------- | ----------------------- |
| `addBook`    | `title: String!`, `author: String!`                 | `BookMutationResponse!` |
| `addTask`    | `title: String!`, `completed: Boolean`              | `TaskMutationResponse!` |
| `updateTask` | `id: Int!`, `title: String!`, `completed: Boolean!` | `TaskMutationResponse!` |
| `deleteTask` | `id: Int!`                                          | `TaskMutationResponse!` |

Mutation responses include `success: Boolean!`, `message: String!`, and `data` (the affected entity).

## Scripts

| Script             | Description                                               |
| ------------------ | --------------------------------------------------------- |
| `npm run prepare`  | Run migrations, generate Prisma client, seed the database |
| `npm run generate` | Run GraphQL Codegen                                       |
| `npm run compile`  | Generate types and compile TypeScript                     |
| `npm run start`    | Compile and start the server                              |
| `npm run watch`    | Watch mode with live reload                               |

## Project Structure

```
src/
├── schema.graphql        # GraphQL schema definition
├── server.ts             # Apollo Server entrypoint
├── generated/            # Auto-generated TypeScript types
├── resolvers/            # Query and mutation resolvers
└── services/             # Business logic (booksService, tasksService)
prisma/
├── schema.prisma         # Database schema
├── seed.ts               # Seed script
├── data.ts               # Seed data
└── migrations/           # Prisma migrations
```
