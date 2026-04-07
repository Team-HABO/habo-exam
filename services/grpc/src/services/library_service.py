import grpc # type: ignore
import threading

# These imports will work after you run the protoc compiler (Step 2)
from src.generated import library_pb2
from src.generated import library_pb2_grpc
from src.db.database import get_connection


class LibraryService(library_pb2_grpc.LibraryServiceServicer):
    """Implements the three RPCs defined in library.proto."""

    def __init__(self):
        # List of active WatchBooks subscriber streams
        self._watchers = []
        # Lock to safely add/remove watchers from multiple threads
        self._watchers_lock = threading.Lock()

    # ---- RPC 1: GetBookById (Unary) ----
    def GetBookById(self, request, context):
        conn = get_connection()
        try:
            cursor = conn.execute(
                "SELECT nBookID, cTitle, nAuthorID, nPublishingCompanyID, nPublishingYear "
                "FROM tbook WHERE nBookID = ?",
                (request.book_id,),
            )
            row = cursor.fetchone()

            if row is None:
                context.set_code(grpc.StatusCode.NOT_FOUND)
                context.set_details(f"Book with ID {request.book_id} not found")
                return library_pb2.Book()

            return library_pb2.Book(
                book_id=row["nBookID"],
                title=row["cTitle"],
                author_id=row["nAuthorID"],
                publishing_company_id=row["nPublishingCompanyID"],
                publishing_year=row["nPublishingYear"],
            )
        finally:
            conn.close()

    # ---- RPC 2: CreateBook (Unary) ----
    def CreateBook(self, request, context):
        conn = get_connection()
        try:
            # Validate: author must exist
            author = conn.execute(
                "SELECT nAuthorID FROM tauthor WHERE nAuthorID = ?",
                (request.author_id,),
            ).fetchone()
            if author is None:
                context.set_code(grpc.StatusCode.INVALID_ARGUMENT)
                context.set_details(f"Author with ID {request.author_id} does not exist")
                return library_pb2.CreateBookResponse()

            # Validate: publisher must exist
            publisher = conn.execute(
                "SELECT nPublishingCompanyID FROM tpublishingcompany WHERE nPublishingCompanyID = ?",
                (request.publishing_company_id,),
            ).fetchone()
            if publisher is None:
                context.set_code(grpc.StatusCode.INVALID_ARGUMENT)
                context.set_details(f"Publisher with ID {request.publishing_company_id} does not exist")
                return library_pb2.CreateBookResponse()

            # Validate: publishing year >= 1900
            if request.publishing_year < 1900:
                context.set_code(grpc.StatusCode.INVALID_ARGUMENT)
                context.set_details("Publishing year must be >= 1900")
                return library_pb2.CreateBookResponse()

            # Validate: title not empty
            if not request.title.strip():
                context.set_code(grpc.StatusCode.INVALID_ARGUMENT)
                context.set_details("Title must not be empty")
                return library_pb2.CreateBookResponse()

            # Insert the book
            cursor = conn.execute(
                "INSERT INTO tbook (cTitle, nAuthorID, nPublishingCompanyID, nPublishingYear) "
                "VALUES (?, ?, ?, ?)",
                (request.title, request.author_id, request.publishing_company_id, request.publishing_year),
            )
            conn.commit()
            new_id = cursor.lastrowid

            # Notify all WatchBooks subscribers
            book = library_pb2.Book(
                book_id=new_id,
                title=request.title,
                author_id=request.author_id,
                publishing_company_id=request.publishing_company_id,
                publishing_year=request.publishing_year,
            )
            self._notify_watchers(book)

            return library_pb2.CreateBookResponse(book_id=new_id)
        finally:
            conn.close()

    # ---- RPC 3: WatchBooks (Server Streaming) ----
    def WatchBooks(self, request, context):
        # Create a queue for this subscriber
        import queue
        q = queue.Queue()

        with self._watchers_lock:
            self._watchers.append(q)

        try:
            # Keep the stream open, yielding books as they arrive
            while context.is_active():
                try:
                    book = q.get(timeout=1.0)
                    yield book
                except queue.Empty:
                    continue
        finally:
            with self._watchers_lock:
                self._watchers.remove(q)

    # ---- Helper: push a new book to all active watchers ----
    def _notify_watchers(self, book):
        with self._watchers_lock:
            for q in self._watchers:
                q.put(book)
