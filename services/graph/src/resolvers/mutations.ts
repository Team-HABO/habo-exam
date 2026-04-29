// Resolvers define how to fetch the types defined in your schema.
import { MutationResolvers } from "../generated/graphql.js";
import { addBook } from "../services/booksService.js";
import { addTask, deleteTask, updateTask } from "../services/tasksService.js";

const mutations: MutationResolvers = {
    addBook: async (_, { title, author }) => await addBook(title, author),
    addTask: async (_, { title }) => await addTask(title),
    updateTask: async (_, { id, title, completed }) => await updateTask(id, title, completed),
    deleteTask: async (_, { id }) => await deleteTask(id),
};

export default mutations;
