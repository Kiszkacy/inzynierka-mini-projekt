import socket
import threading

class SocketServerThread(threading.Thread):
    def __init__(self, host, port):
        super().__init__()
        self.host = host
        self.port = port
        self.server_socket = None

    def run(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.bind((self.host, self.port))
        self.server_socket.listen(1)
        print("Server is listening on {}:{}".format(self.host, self.port))
        connection, client_address = self.server_socket.accept()

        with connection:
            print("Connected to:", client_address)
            while True: 
                data = connection.recv(1024)
                if data:
                    print("Received:", data.decode())
                    self.send(b"This message is sent from server: " + data)

    def send(self, data):
        try:
            if self.connection:
                self.connection.sendall(data)
        except Exception as e:
            print("Exception occurred during send:", e)

# Użycie klasy SocketServerThread
server_thread = SocketServerThread('localhost', 12345)
server_thread.start()