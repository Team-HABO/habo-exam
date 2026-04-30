import { prisma } from "../../prisma/prisma.js";
import { sanitizeInput } from "../utils/sanitize.js";

export async function getTasks() {
    return prisma.task.findMany();
}

export async function getTaskById(id: number) {
    return prisma.task.findUnique({ where: { id } });
}

export async function addTask(title: string) {
    try {
        // Sanitize input before storing
        const sanitizedTitle = sanitizeInput(title);

        const existingTask = await prisma.task.findUnique({ where: { title: sanitizedTitle } });

        if (existingTask) {
            return {
                success: false,
                message: "A task with this title already exists.",
                data: null,
            };
        }

        const newTask = await prisma.task.create({ data: { title: sanitizedTitle, completed: false } });

        return {
            success: true,
            message: "Task added successfully.",
            data: newTask,
        };
    } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to add task";
        return {
            success: false,
            message,
            data: null,
        };
    }
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
    try {
        const existingTask = await prisma.task.findUnique({ where: { id } });
        if (!existingTask) {
            return {
                success: false,
                message: "A task with this id does not exist.",
                data: null,
            };
        }

        // Sanitize input before updating
        const sanitizedTitle = sanitizeInput(title);

        const updatedTask = await prisma.task.update({
            where: { id },
            data: { title: sanitizedTitle, completed },
        });

        return {
            success: true,
            message: "Task updated successfully.",
            data: updatedTask,
        };
    } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to update task";
        return {
            success: false,
            message,
            data: null,
        };
    }
}
