import socket

server_address = ('localhost', 12345)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(server_address)
server_socket.listen(1)

connection, client_address = server_socket.accept()
try:
    print("Start")
    while True:
        data = connection.recv(1024)
        if data:
            print("Received:", data.decode())
            connection.sendall(b"This message is sent from server: " + data)
        else:
            print("No data")
            break

finally:
    connection.close()