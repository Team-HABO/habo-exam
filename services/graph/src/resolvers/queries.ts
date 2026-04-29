// Resolvers define how to fetch the types defined in your schema.
import { QueryResolvers } from "../generated/graphql.js";
import { getBookById, getBooks } from "../services/booksService.js";
import { getTaskById, getTasks } from "../services/tasksService.js";

const queries: QueryResolvers = {
    books: async () => await getBooks(),
    book: async (_, { id }) => await getBookById(id),
    tasks: async () => await getTasks(),
    task: async (_, { id }) => await getTaskById(id),
};

export default queries;
