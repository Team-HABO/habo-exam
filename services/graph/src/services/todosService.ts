import { tasks } from "../data.js";

export function getTasks() {
    return tasks;
}

export function getTaskByTitle(title: string) {
    return tasks.find((t) => t.title.trim().toUpperCase() === title.trim().toUpperCase()) ?? null;
}

export function addTask(title: string) {
    const existingTask = getTaskByTitle(title);
    if (existingTask) {
        return {
            success: false,
            message: "A task with this title already exists.",
            task: null,
        };
    }

    const newTask = { title, completed: false };
    tasks.push(newTask);

    return {
        success: true,
        message: "Task added successfully.",
        book: newTask,
    };
}

export function deleteTask(title: string) {
    const existingTask = getTaskByTitle(title);
    if (!existingTask) {
        return {
            success: false,
            message: "A task with this title does not exist.",
            task: null,
        };
    }

    // Remove task
    const taskIndex = tasks.indexOf(existingTask);
    tasks.splice(taskIndex, 1);

    return {
        success: true,
        message: "Task removed successfully.",
        book: existingTask,
    };
}

export function updateTask(title: string, newTitle: string, completed: boolean) {
    const existingTask = getTaskByTitle(title);
    if (!existingTask) {
        return {
            success: false,
            message: "A task with this title does not exist.",
            task: null,
        };
    }

    const updatedTask = { title: newTitle, completed };

    const taskIndex = tasks.indexOf(existingTask);
    tasks[taskIndex] = updatedTask;

    return {
        success: true,
        message: "Task updated successfully.",
        book: updatedTask,
    };
}
