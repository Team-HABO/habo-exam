/**
 * Type definitions that describe what data can be retrieved from the schema and what mutations and queries can be performed.
 * This file is used by the GraphQL server to understand the structure of the data and the operations that can be performed on it.
 * It also serves as documentation for developers to understand what queries and mutations are available in the GraphQL API.
 * The type definitions are written in GraphQL SDL (Schema Definition Language)
 */
export const typeDefinitions = `
  type Book {
    bookId: Int!
    title: String!
    authorId: Int!
    publishingYear: Int
    publishingCompanyId: Int!
  }
  type Author {
    authorId: Int!
    firstName: String!
    lastName: String!
  }
  type Publisher {
    publishingCompanyId: Int!
    name: String!
  }

  type Query {
    books: [Book!]!
    authors: [Author!]!
    publishers: [Publisher!]!
    book(bookId: Int!): Book
  }

  type Mutation {
    addBook(
      title: String!
      authorId: Int!
      publishingYear: Int
      publishingCompanyId: Int!
    ): Book!
    addAuthor(firstName: String!, lastName: String): Author!
    addPublisher(name: String!): Publisher!
    updateBook(
      bookId: Int!
      title: String!
      authorId: Int!
      publishingYear: Int
      publishingCompanyId: Int!
    ): Book!
    updateAuthor(authorId: Int!, firstName: String!, lastName: String): Author!
    updatePublisher(publishingCompanyId: Int!, name: String!): Publisher!
    deleteBook(bookId: Int!): Book!
    deleteAuthor(authorId: Int!): Author!
    deletePublisher(publishingCompanyId: Int!): Publisher!
  }
`;
