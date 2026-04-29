import { prisma } from "../../prisma/prisma.js";

export async function getTasks() {
    return prisma.task.findMany();
}

export async function getTaskById(id: number) {
    return prisma.task.findUnique({ where: { id } });
}

export async function addTask(title: string) {
    const existingTask = await prisma.task.findUnique({ where: { title } });

    if (existingTask) {
        return {
            success: false,
            message: "A task with this title already exists.",
            data: null,
        };
    }

    const newTask = await prisma.task.create({ data: { title, completed: false } });

    return {
        success: true,
        message: "Task added successfully.",
        data: newTask,
    };
}

export async function deleteTask(id: number) {
    const existingTask = await prisma.task.findUnique({ where: { id } });
    if (!existingTask) {
        return {
            success: false,
            message: "A task with this id does not exist.",
            data: null,
        };
    }

    await prisma.task.delete({ where: { id } });

    return {
        success: true,
        message: "Task removed successfully.",
        data: existingTask,
    };
}

export async function updateTask(id: number, title: string, completed: boolean) {
    const existingTask = await prisma.task.findUnique({ where: { id } });
    if (!existingTask) {
        return {
            success: false,
            message: "A task with this id does not exist.",
            data: null,
        };
    }

    const updatedTask = await prisma.task.update({
        where: { id },
        data: { title, completed },
    });

    return {
        success: true,
        message: "Task updated successfully.",
        data: updatedTask,
    };
}
