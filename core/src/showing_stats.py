# type: ignore

import matplotlib.pyplot as plt
import numpy as np
import seaborn as sns


def load_data_from_file(file_path):
    with open(file_path) as file:
        return [float(line.strip().replace(",", ".")) for line in file.readlines()]


def plot_data(data):
    plt.plot(data)
    plt.title("Wykres czasu wykonania")
    plt.xlabel("Iteracja")
    plt.ylabel("Czas wykonania (s)")
    plt.grid(True)
    plt.show()


godot_pipe_send = load_data_from_file(
    "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\godot_pipe_send.txt"
)
godot_pipe_recv = load_data_from_file(
    "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\godot_pipe_recv.txt"
)
python_pipe_send = load_data_from_file(
    "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\python_pipe_send.txt"
)
python_pipe_recv = load_data_from_file(
    "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\python_pipe_recv.txt"
)

godot_socket_send = load_data_from_file(
    "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\godot_socket_send.txt"
)
godot_socket_recv = load_data_from_file(
    "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\godot_socket_recv.txt"
)
python_socket_send = load_data_from_file(
    "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\python_socket_send.txt"
)
python_socket_recv = load_data_from_file(
    "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\python_socket_recv.txt"
)


def calculate_average(data):
    if len(data) == 0:
        return 0
    return sum(data) / len(data)


data_socket = []
data_pipe = []

for i in range(5000):
    data_pipe.append(godot_pipe_send[i] + python_pipe_send[i] + godot_pipe_recv[i] + python_pipe_recv[i])
    data_socket.append(python_socket_send[i] + godot_socket_send[i] + python_socket_recv[i] + godot_socket_recv[i])

# print(f"Średnia wartość czasu: {average:.5f} sekundy")
"""
methods = ['Socket', 'Pipe']
times = [average_socket, average_pipe]

plt.figure(figsize=(8, 6))
plt.bar(methods, times, color=['#808080', '#d3d3d3'])

plt.ylabel('Czas przesyłu (s)')
plt.title('Porównanie szybkości przesyłu: Socket vs Pipe')

for i, time in enumerate(times):
    plt.text(i, time + 0.0001, f'{time:.4f}s', ha='center', fontsize=12)

"""


def remove_outliers(data_, lower_percentile=1, upper_percentile=99):
    lower_bound = np.percentile(data_, lower_percentile)
    upper_bound = np.percentile(data_, upper_percentile)
    return [x for x in data_ if lower_bound <= x <= upper_bound]


cleaned_data_socket = remove_outliers(data_socket)
cleaned_data_pipe = remove_outliers(data_pipe)

data = [cleaned_data_socket, cleaned_data_pipe]
labels = ["Gniazdo", "Potok"]

plt.figure(figsize=(8, 8))
sns.violinplot(data=data, palette=["#D0FFFF", "#FFEBCD"])

mean_socket = np.mean(cleaned_data_socket)
mean_pipe = np.mean(cleaned_data_pipe)

plt.xticks([0, 1], labels)
plt.title("Porównanie rozkładu czasu przesyłu")
plt.ylabel("Czas przesyłu (s)")

plt.legend([f"Średnia dla gniazda: {mean_socket:.4f}s", f"Średnia dla potoku: {mean_pipe:.4f}s"], loc="upper right")

plt.show()
