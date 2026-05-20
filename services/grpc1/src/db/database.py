import sqlite3
import os

# Path to the shared SQLite database
DB_PATH = os.path.join(os.path.dirname(__file__), "../../data/chat.db")


def get_connection() -> sqlite3.Connection:
    """Return a connection to the SQLite chat database."""
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row  # allows dict-like access: row["cUsername"]
    return conn


def init_db():
    """Create tables if they don't exist yet."""
    conn = get_connection()
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS tmessage (
            nMessageID INTEGER PRIMARY KEY AUTOINCREMENT,
            cUsername  TEXT NOT NULL,
            cContent   TEXT NOT NULL,
            cTimestamp TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%SZ', 'now'))
        )
        """
    )
    conn.commit()
    conn.close()
