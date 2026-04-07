import type { Request, Response, NextFunction } from "express";
import * as service from "../services/publisherService";
import { addPublisherLinks, withLinks, withLinksArray } from "../utils/hateoas";

export const createPublisher = async (req: Request<{}, {}, { name: string }, {}>, res: Response, next: NextFunction) => {
	try {
		const { name } = req.body;
		const publisher = await service.createPublisher(name);
		res.status(201).json(withLinks(publisher, addPublisherLinks(publisher)));
	} catch (error) {
		next(error);
	}
};

export const getPublishers = async (_req: Request, res: Response, next: NextFunction) => {
	try {
		const publishers = await service.getPublishers();
		res.status(200).json(withLinksArray(publishers, addPublisherLinks));
	} catch (error) {
		next(error);
	}
};

export const getPublisherById = async (req: Request<{ id: string }>, res: Response, next: NextFunction) => {
	try {
		const id = parseInt(req.params.id);

		const publisher = await service.getPublisherById(id);

		res.status(200).json(withLinks(publisher, addPublisherLinks(publisher)));
	} catch (error) {
		next(error);
	}
};

export const updatePublisher = async (req: Request<{ id: string }, {}, { name: string }, {}>, res: Response, next: NextFunction) => {
	try {
		const id = parseInt(req.params.id);
		const { name } = req.body;

		const publisher = await service.updatePublisher(id, name);

		res.status(200).json(withLinks(publisher, addPublisherLinks(publisher)));
	} catch (error) {
		next(error);
	}
};

export const deletePublisher = async (req: Request<{ id: string }>, res: Response, next: NextFunction) => {
	try {
		const id = parseInt(req.params.id);

		await service.deletePublisher(id);

		res.status(204).end();
	} catch (error) {
		next(error);
	}
};
