import { Router } from "express";
import { createPublisher, deletePublisher, getPublisherById, getPublishers, updatePublisher } from "../controllers/publisherController";

const router = Router();

router.route("/").get(getPublishers).post(createPublisher);

router.route("/:id").get(getPublisherById).put(updatePublisher).delete(deletePublisher);

export { router as publisherRouter };
