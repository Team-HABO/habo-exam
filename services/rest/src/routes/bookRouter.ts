import { Router } from "express";
import { createBook, deleteBook, getBookById, getBooks, updateBook } from "../controllers/bookController";

const router = Router();

router.route("/").get(getBooks).post(createBook);

router.route("/:id").get(getBookById).put(updateBook).delete(deleteBook);

export { router as bookRouter };
