import { Router } from "express";
import { createAuthor, deleteAuthor, getAuthorById, getAuthors, updateAuthor } from "../controllers/authorController";

const router = Router();

router.route("/").get(getAuthors).post(createAuthor);
router.route("/:id").get(getAuthorById).put(updateAuthor).delete(deleteAuthor);

export { router as authorRouter };
