import { prisma } from "../../prisma/prisma.js";

export async function getBooks() {
    return await prisma.book.findMany();
}

export async function getBookById(id: number) {
    return await prisma.book.findUnique({ where: { id } });
}

export async function addBook(title: string, author: string) {
    const existingBook = await prisma.book.findUnique({
        where: { title_author: { title, author } },
    });

    if (existingBook) {
        return {
            success: false,
            message: "A book with this title and author already exists.",
            book: null,
        };
    }

    const newBook = await prisma.book.create({ data: { title, author } });

    return {
        success: true,
        message: "Book added successfully.",
        book: newBook,
    };
}
