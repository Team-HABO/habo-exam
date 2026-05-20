import grpc  # type: ignore
from concurrent import futures
from grpc_reflection.v1alpha import reflection  # type: ignore

from src.generated import chat_pb2
from src.generated import chat_pb2_grpc
from src.services.chat_service import ChatService
from src.db.database import init_db

PORT = 50052


def serve():
    # Initialise the database (creates table if needed)
    init_db()

    # ThreadPoolExecutor handles concurrent RPC calls
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))

    # Register our service implementation with the server
    chat_pb2_grpc.add_ChatServiceServicer_to_server(ChatService(), server)

    # Enable server reflection so tools like Bruno can auto-discover methods
    SERVICE_NAMES = (
        chat_pb2.DESCRIPTOR.services_by_name["ChatService"].full_name,
        reflection.SERVICE_NAME,
    )
    reflection.enable_server_reflection(SERVICE_NAMES, server)

    server.add_insecure_port(f"[::]:{PORT}")
    server.start()

    print(f"gRPC Chat server listening on port {PORT}")
    server.wait_for_termination()


if __name__ == "__main__":
    serve()
