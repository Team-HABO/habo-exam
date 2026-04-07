import type { Request, Response, NextFunction } from "express";
import * as service from "../services/authorService";
import { addAuthorLinks, withLinks, withLinksArray } from "../utils/hateoas";

export const createAuthor = async (req: Request<{}, {}, { name: string; surname: string }, {}>, res: Response, next: NextFunction) => {
	try {
		const { name, surname } = req.body;
		const author = await service.createAuthor(name, surname);
		res.status(201).json(withLinks(author, addAuthorLinks(author)));
	} catch (error) {
		next(error);
	}
};

export const getAuthors = async (_req: Request, res: Response, next: NextFunction) => {
	try {
		const authors = await service.getAuthors();
		res.status(200).json(withLinksArray(authors, addAuthorLinks));
	} catch (error) {
		next(error);
	}
};

export const getAuthorById = async (req: Request<{ id: string }>, res: Response, next: NextFunction) => {
	try {
		const id = parseInt(req.params.id);

		const author = await service.getAuthorById(id);

		res.status(200).json(withLinks(author, addAuthorLinks(author)));
	} catch (error) {
		next(error);
	}
};

export const updateAuthor = async (
	req: Request<{ id: string }, {}, { name: string; surname: string }, {}>,
	res: Response,
	next: NextFunction
) => {
	try {
		const id = parseInt(req.params.id);
		const { name, surname } = req.body;

		const author = await service.updateAuthor(id, name, surname);

		res.status(200).json(withLinks(author, addAuthorLinks(author)));
	} catch (error) {
		next(error);
	}
};

export const deleteAuthor = async (req: Request<{ id: string }>, res: Response, next: NextFunction) => {
	try {
		const id = parseInt(req.params.id);

		await service.deleteAuthor(id);

		res.status(204).end();
	} catch (error) {
		next(error);
	}
};
