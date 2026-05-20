import grpc  # type: ignore
import threading
from datetime import datetime, timezone

from src.generated import chat_pb2
from src.generated import chat_pb2_grpc
from src.db.database import get_connection


class ChatService(chat_pb2_grpc.ChatServiceServicer):
    """Implements the two RPCs defined in chat.proto."""

    def __init__(self):
        # List of (queue, context) pairs for active bidirectional Chat streams
        self._subscribers: list[tuple] = []
        self._lock = threading.Lock()

    # ---- RPC 1: SendMessage (Unary) ----
    def SendMessage(self, request, context):
        """Store a single message and return its ID and timestamp."""

        if not request.username.strip():
            context.set_code(grpc.StatusCode.INVALID_ARGUMENT)
            context.set_details("Username must not be empty")
            return chat_pb2.SendMessageResponse()

        if not request.content.strip():
            context.set_code(grpc.StatusCode.INVALID_ARGUMENT)
            context.set_details("Message content must not be empty")
            return chat_pb2.SendMessageResponse()

        timestamp = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

        conn = get_connection()
        try:
            cursor = conn.execute(
                "INSERT INTO tmessage (cUsername, cContent, cTimestamp) VALUES (?, ?, ?)",
                (request.username.strip(), request.content.strip(), timestamp),
            )
            conn.commit()
            new_id = cursor.lastrowid
        finally:
            conn.close()

        # Also broadcast to all active Chat() subscribers
        msg = chat_pb2.ChatMessage(
            message_id=new_id,
            username=request.username.strip(),
            content=request.content.strip(),
            timestamp=timestamp,
        )
        self._broadcast(msg)

        return chat_pb2.SendMessageResponse(message_id=new_id, timestamp=timestamp)

    # ---- RPC 2: Chat (Bidirectional Streaming) ----
    def Chat(self, request_iterator, context):
        """
        Each connected client sends a stream of ChatMessage requests.
        Every message is saved to the DB and broadcast to ALL connected clients,
        so each client's response stream receives every message sent by any participant.
        """
        # Thread-safe queue used to push outbound messages to this client
        import queue
        outbound: queue.Queue = queue.Queue()

        # Register this client as a subscriber so it receives broadcasts
        with self._lock:
            self._subscribers.append(outbound)

        def _read_incoming():
            """Read messages from this client and broadcast them."""
            try:
                for msg in request_iterator:
                    if not msg.username.strip() or not msg.content.strip():
                        continue  # silently skip malformed messages

                    timestamp = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

                    conn = get_connection()
                    try:
                        cursor = conn.execute(
                            "INSERT INTO tmessage (cUsername, cContent, cTimestamp) VALUES (?, ?, ?)",
                            (msg.username.strip(), msg.content.strip(), timestamp),
                        )
                        conn.commit()
                        new_id = cursor.lastrowid
                    finally:
                        conn.close()

                    saved_msg = chat_pb2.ChatMessage(
                        message_id=new_id,
                        username=msg.username.strip(),
                        content=msg.content.strip(),
                        timestamp=timestamp,
                    )
                    self._broadcast(saved_msg)
            finally:
                # Signal this client's output loop to stop
                outbound.put(None)

        # Run the incoming reader in a background thread so we can yield outbound simultaneously
        reader_thread = threading.Thread(target=_read_incoming, daemon=True)
        reader_thread.start()

        try:
            while True:
                item = outbound.get()
                if item is None:
                    break  # client disconnected
                if context.is_active():
                    yield item
        finally:
            with self._lock:
                try:
                    self._subscribers.remove(outbound)
                except ValueError:
                    pass

    # ---- Internal helpers ----

    def _broadcast(self, msg: chat_pb2.ChatMessage):
        """Push a message to every active Chat subscriber's queue."""
        with self._lock:
            active = list(self._subscribers)
        for q in active:
            q.put(msg)
