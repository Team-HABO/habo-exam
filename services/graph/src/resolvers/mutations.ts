// Resolvers define how to fetch the types defined in your schema.
import { MutationResolvers } from "../generated/graphql.js";
import { addBook } from "../services/booksService.js";
import { addTask, deleteTask, updateTask } from "../services/todosService.js";

const mutations: MutationResolvers = {
    addBook: (_, { title, author }) => addBook(title, author),
    addTask: (_, { title }) => addTask(title),
    updateTask: (_, { title, newTitle, completed }) => updateTask(title, newTitle, completed),
    deleteTask: (_, { title }) => deleteTask(title)
};

export default mutations;
