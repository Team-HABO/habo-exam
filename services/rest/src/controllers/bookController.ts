import type { Request, Response, NextFunction } from "express";
import * as service from "../services/bookService";
import { addBookLinks, withLinks, withLinksArray } from "../utils/hateoas";

export const createBook = async (
	req: Request<{}, {}, { title: string; publishingYear: number; authorId: number; publishingCompanyId: number }, {}>,
	res: Response,
	next: NextFunction
) => {
	try {
		const { title, publishingYear, authorId, publishingCompanyId } = req.body;
		const book = await service.createBook(title, publishingYear, authorId, publishingCompanyId);
		res.status(201).json(withLinks(book, addBookLinks(book)));
	} catch (error) {
		next(error);
	}
};

export const getBooks = async (req: Request<{}, {}, {}, { pageNum: string; pageSize: string }>, res: Response, next: NextFunction) => {
	try {
		const pageNum = parseInt(req.query.pageNum);
		const pageSize = parseInt(req.query.pageSize);

		const books = await service.getBooks(pageNum, pageSize);
		res.status(200).json(withLinksArray(books, addBookLinks));
	} catch (error) {
		next(error);
	}
};

export const getBookById = async (req: Request<{ id: string }>, res: Response, next: NextFunction) => {
	try {
		const id = parseInt(req.params.id);

		const book = await service.getBookById(id);

		res.status(200).json(withLinks(book, addBookLinks(book)));
	} catch (error) {
		next(error);
	}
};

export const updateBook = async (
	req: Request<{ id: string }, {}, { title: string; publishingYear: number; authorId: number; publishingCompanyId: number }, {}>,
	res: Response,
	next: NextFunction
) => {
	try {
		const id = parseInt(req.params.id);
		const { title, publishingYear, authorId, publishingCompanyId } = req.body;

		const book = await service.updateBook(id, title, publishingYear, authorId, publishingCompanyId);

		res.status(200).json(withLinks(book, addBookLinks(book)));
	} catch (error) {
		next(error);
	}
};

export const deleteBook = async (req: Request<{ id: string }>, res: Response, next: NextFunction) => {
	try {
		const id = parseInt(req.params.id);

		await service.deleteBook(id);

		res.status(204).end();
	} catch (error) {
		next(error);
	}
};
