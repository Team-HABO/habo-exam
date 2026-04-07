import sqlite3
import os

# Path to the shared SQLite database.
DB_PATH = os.path.join(os.path.dirname(__file__), "../../data/library.db")


def get_connection() -> sqlite3.Connection:
    """Return a connection to the SQLite library database."""
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row  # allows dict-like access: row["cTitle"]
    conn.execute("PRAGMA foreign_keys = ON")  # enforce foreign key constraints
    return conn
