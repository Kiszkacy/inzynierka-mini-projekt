import socket

server_address = ('localhost', 12345)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(server_address)
server_socket.listen(1)

connection, client_address = server_socket.accept()
try:
    
    while True:
        data = connection.recv(1024)
        if data:
            print("Otrzymano:", data.decode())
            connection.sendall(b"Dane odebrane przez serwer: " + data)
        else:
            print("Brak danych")
            break

finally:
    # Zamykanie połączenia
    connection.close()