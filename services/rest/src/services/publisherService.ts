import * as repository from "../repositories/publisherRepository";
import { AppError } from "../middlewares/errorHandler";

export const createPublisher = async (name: string) => {
	if (!name) {
		throw new AppError(400, "name is undefined");
	}

	const newPublisher = await repository.createPublisher(name);
	return newPublisher;
};

export const getPublishers = async () => {
	return await repository.getPublishers();
};

export const getPublisherById = async (id: number) => {
	const publisher = await repository.getPublisherById(id);

	if (!publisher) {
		throw new AppError(404, "Publisher not found");
	}

	return publisher;
};

export const updatePublisher = async (id: number, name: string) => {
	if (!name) {
		throw new AppError(400, "name is undefined");
	}

	return await repository.updatePublisher(id, name);
};

export const deletePublisher = async (id: number) => {
	await repository.deletePublisher(id);
};
