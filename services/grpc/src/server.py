import grpc # type: ignore 
from concurrent import futures

from src.generated import library_pb2_grpc
from src.services.library_service import LibraryService

PORT = 50051


def serve():
    # ThreadPoolExecutor handles concurrent RPC calls (one thread per call)
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))

    # Register our service implementation with the server
    library_pb2_grpc.add_LibraryServiceServicer_to_server(LibraryService(), server)

    # Listen on all interfaces, port 50051
    server.add_insecure_port(f"[::]:{PORT}")
    server.start()

    print(f"gRPC server listening on port {PORT}")
    server.wait_for_termination()


if __name__ == "__main__":
    serve()
