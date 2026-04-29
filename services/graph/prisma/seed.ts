import { prisma } from "./prisma";
import { books, tasks } from "./data";

async function main() {
    console.log("Seeding...");
    
    for (const book of books) {
        await prisma.book.upsert({
            where: { title_author: { title: book.title, author: book.author } },
            update: {},
            create: book,
        });
    }

    for (const task of tasks) {
        await prisma.task.upsert({
            where: { title: task.title },
            update: { completed: task.completed },
            create: task,
        });
    }

    console.log("Seeding complete.");
}

main()
    .then(() => prisma.$disconnect())
    .catch(async (e) => {
        console.error(e);
        await prisma.$disconnect();
        process.exit(1);
    });
