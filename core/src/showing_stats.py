# type: ignore

import matplotlib.pyplot as plt
import numpy as np
import seaborn as sns


def load_data_from_file(file_path):
    with open(file_path) as file:
        return [float(line.strip()) for line in file.readlines()]


def plot_data(data):
    plt.plot(data)
    plt.title("Wykres czasu wykonania")
    plt.xlabel("Iteracja")
    plt.ylabel("Czas wykonania (s)")
    plt.grid(True)
    plt.show()


log_file_pipe = "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\pipe.txt"
log_file_socket = "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\socket.txt"


def calculate_average(data):
    if len(data) == 0:
        return 0
    return sum(data) / len(data)


data_pipe = load_data_from_file(log_file_pipe)
average_pipe = calculate_average(data_pipe)
data_socket = load_data_from_file(log_file_socket)
average_socket = calculate_average(data_socket)
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
labels = ["Socket", "Pipe"]

plt.figure(figsize=(8, 6))
sns.violinplot(data=data, palette=["#D0FFFF", "#FFEBCD"])

mean_socket = np.mean(cleaned_data_socket)
mean_pipe = np.mean(cleaned_data_pipe)

plt.xticks([0, 1], labels)
plt.title("Porównanie rozkładu czasu przesyłu: Socket vs Pipe")
plt.ylabel("Czas przesyłu (s)")

plt.legend([f"Średnia Socket: {mean_socket:.4f}s", f"Średnia Pipe: {mean_pipe:.4f}s"], loc="upper right")

plt.show()
