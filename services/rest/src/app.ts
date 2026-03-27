import express from "express";
import { errorHandler } from "./middlewares/errorHandler";
import { authorRouter } from "./routes/authorRouter";
import { publisherRouter } from "./routes/publisherRouter";
import { bookRouter } from "./routes/bookRouter";

const app = express();

app.use(express.json());

// Routes
app.use("/authors", authorRouter);
app.use("/publishers", publisherRouter);
app.use("/books", bookRouter);

// Global error handler
app.use(errorHandler);

export default app;
