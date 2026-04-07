FROM python:3.12-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY proto/ ./proto/
COPY src/ ./src/
COPY data/ ./data/

# Generate protobuf/gRPC code at build time
RUN python -m grpc_tools.protoc \
    -I./proto \
    --python_out=./src/generated \
    --grpc_python_out=./src/generated \
    ./proto/library.proto

EXPOSE 50051

CMD ["python", "-m", "src.server"]