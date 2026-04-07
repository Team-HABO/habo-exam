import { createSchema } from "graphql-yoga";
import { mutations } from "./schema/mutations.js";
import { queries } from "./schema/queries.js";
import { typeDefinitions } from "./schema/typeDefinitions.js";
import { createServer } from "node:http";
import { createYoga } from "graphql-yoga";
import { createContext, GraphQLContext } from "./context.js";

export const schema = createSchema({
  typeDefs: typeDefinitions,
  resolvers: {
    Query: queries,
    Mutation: mutations,
  },
});

/**
 * Main entry point to start the GraphQL server
 * * @remarks
 * /live is a health check endpoint that returns 200 OK if the server is running.
 */
function main() {
  const yoga = createYoga<GraphQLContext>({
    schema,
    healthCheckEndpoint: "/live",
    context: createContext,
  });

  const server = createServer(yoga);

  server.listen(4000, () => {
    console.info("Server is running on http://localhost:4000/graphql");
  });
}

main();
