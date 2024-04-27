import socket
import threading


class SocketServerThread(threading.Thread):
    def __init__(self, host, port):
        super().__init__()
        self.host = host
        self.port = port
        self.server_socket = None
        self.received_request = False
        self.data = None
        self.connection: socket = None

    def get_request(self) -> bytes | None:
        if self.received_request:
            self.received_request = False
            return self.data
        else:
            return None

    def run(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.bind((self.host, self.port))
        self.server_socket.listen(1)
        print("Server is listening on {}:{}".format(self.host, self.port))
        self.connection, client_address = self.server_socket.accept()

        with self.connection:
            print("Connected to:", client_address)
            while True:
                data = self.connection.recv(1024)
                if data:
                    self.data = data
                    self.received_request = True

    def send(self, data):
        try:
            if self.connection:
                self.connection.sendall(data)
        except Exception as e:
            print("Exception occurred during send:", e)
