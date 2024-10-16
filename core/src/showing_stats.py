import matplotlib.pyplot as plt


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


log_file = "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\pipe.txt"


def calculate_average(data):
    if len(data) == 0:
        return 0
    return sum(data) / len(data)


data = load_data_from_file(log_file)
average = calculate_average(data)
# print(f"Średnia wartość czasu: {average:.5f} sekundy")
