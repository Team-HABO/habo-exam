import { prisma } from "../../prisma/prisma.js";
import { sanitizeInput } from "../utils/sanitize.js";

export async function getBooks() {
    return await prisma.book.findMany();
}

export async function getBookById(id: number) {
    return await prisma.book.findUnique({ where: { id } });
}

export async function addBook(title: string, author: string) {
    try {
        // Sanitize inputs before storing
        const sanitizedTitle = sanitizeInput(title);
        const sanitizedAuthor = sanitizeInput(author);

        const existingBook = await prisma.book.findUnique({
            where: { title_author: { title: sanitizedTitle, author: sanitizedAuthor } },
        });

        if (existingBook) {
            return {
                success: false,
                message: "A book with this title and author already exists.",
                data: null,
            };
        }

        const newBook = await prisma.book.create({
            data: { title: sanitizedTitle, author: sanitizedAuthor },
        });

        return {
            success: true,
            message: "Book added successfully.",
            data: newBook,
        };
    } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to add book";
        return {
            success: false,
            message,
            data: null,
        };
    }
}
