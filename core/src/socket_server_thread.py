import socket
import threading

from loguru import logger


class SocketServerThread(threading.Thread):
    def __init__(self, host, port):
        super().__init__()
        self.host = host
        self.port = port
        self.server_socket = None
        self.received_request = False
        self.data = None
        self.connection: socket = None

    def get_request(self) -> bytes:
        while not self.received_request:
            ...
        self.received_request = False
        return self.data

    def run(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.bind((self.host, self.port))
        self.server_socket.listen(1)

        logger.info(f"Server is listening on {self.host}:{self.port}")

        self.connection, client_address = self.server_socket.accept()

        with self.connection:
            logger.success(f"Connected to: {':'.join(map(str, client_address))}")
            while True:
                data = self.connection.recv(1024)
                if data:
                    self.data = data
                    self.received_request = True

    @logger.catch(message="Exception occurred during send")
    def send(self, data):
        if self.connection:
            self.connection.sendall(data)
