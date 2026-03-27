import type { NextFunction, Request, Response } from "express";

export class AppError extends Error {
	status?: number;

	constructor(status: number, message: string) {
		super(message);
		this.status = status;
	}
}

export const errorHandler = (err: unknown, _req: Request, res: Response, _next: NextFunction) => {
	console.error(err);
	if (err instanceof AppError) {
		res.status(err.status ?? 400).json({
			message: err.message || "Bad Request"
		});
	} else {
		res.status(500).json({
			message: "Internal Server Error"
		});
	}
};
