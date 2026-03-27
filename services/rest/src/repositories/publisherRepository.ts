import { prisma } from "../../prisma/prisma";

export const createPublisher = async (name: string) => {
	return await prisma.publishingCompany.create({
		data: {
			name
		}
	});
};

export const getPublishers = async () => {
	return await prisma.publishingCompany.findMany();
};

export const getPublisherById = async (id: number) => {
	return await prisma.publishingCompany.findUnique({
		where: {
			id: id
		}
	});
};

export const updatePublisher = async (id: number, name: string) => {
	return await prisma.publishingCompany.update({
		where: { id },
		data: { name }
	});
};

export const deletePublisher = async (id: number) => {
	return await prisma.publishingCompany.delete({
		where: {
			id: id
		}
	});
};
