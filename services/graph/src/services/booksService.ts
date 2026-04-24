import { books } from "../data.js";

export function getBooks(){
    return books;
}

export function getBookByTitle(title: string) {
    return books.find((book) => book.title.toLowerCase() === title.toLowerCase()) ?? null;
}

export function addBook(title: string, author: string) {
    const existingBook = getBookByTitle(title);
    if (existingBook) {
        return {
            success: false,
            message: "A book with this title already exists.",
            book: null,
        };
    }

    const newBook = { title, author };
    books.push(newBook);

    return {
        success: true,
        message: "Book added successfully.",
        book: newBook,
    };
}
