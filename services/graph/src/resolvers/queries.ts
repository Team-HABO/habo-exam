// Resolvers define how to fetch the types defined in your schema.
import { QueryResolvers } from "../generated/graphql.js";
import { getBookByTitle, getBooks } from "../services/booksService.js";
import { getTaskByTitle, getTasks } from "../services/todosService.js";

// This resolver retrieves books from the "books" array above.
const queries: QueryResolvers = {
    books: () => getBooks(),
    book: (_, { title }) => getBookByTitle(title),
    tasks: () => getTasks(),
    task: (_, { title }) => getTaskByTitle(title),
};

export default queries;
