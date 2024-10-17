# mypy: ignore-errors
# ruff: disable

import platform
import time
from typing import IO

from loguru import logger

ON_WINDOWS: bool = platform.system() == "Windows"
if ON_WINDOWS:
    import win32file
    import win32pipe
else:
    import os


MAX_BUFFER_SIZE: int = 2048
READ_BUFFER_SIZE: int = 512


class PipeHandler:
    def __init__(
        self,
        pipe_name: str | None = None,
    ) -> None:
        if pipe_name is None:
            pipe_name = "godot-python-pipe"
        self.pipe: int | IO | None = None
        self.pipe_path: str = self._default_pipe_prefix.format(pipe_name=pipe_name)

    def log_time(self, filename, time_elapsed: float) -> None:
        with open(filename, "a") as log_file:
            log_file.write(f"{time_elapsed}\n")

    @property
    def _default_pipe_prefix(self) -> str:
        return r"\\.\pipe\{pipe_name}" if ON_WINDOWS else "/tmp/{pipe_name}"

    def connect(self) -> None:
        if ON_WINDOWS:
            self.pipe = win32pipe.CreateNamedPipe(
                self.pipe_path,
                win32pipe.PIPE_ACCESS_DUPLEX,
                win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
                1,
                MAX_BUFFER_SIZE,
                MAX_BUFFER_SIZE,
                0,
                None,
            )
            win32pipe.ConnectNamedPipe(self.pipe, None)
        else:
            os.mkfifo(self.pipe_path)
            self.pipe = open(self.pipe_path, "r+")  # noqa: SIM115
        logger.info(f"Connected to the {self.pipe_path} pipe.")

    def disconnect(self) -> None:
        if ON_WINDOWS:
            win32file.CloseHandle(self.pipe)
        else:
            self.pipe.close()
        self.pipe = None

    def send(self, data_bytes) -> None:
        if ON_WINDOWS:
            start_time = time.perf_counter()
            win32file.WriteFile(self.pipe, data_bytes)
            end_time = time.perf_counter()
            elapsed_time = end_time - start_time
        else:
            self.pipe.write(data_bytes)
            self.pipe.flush()
            elapsed_time = 0
        self.log_time(
            "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\python_pipe_send.txt",
            elapsed_time,
        )

    def receive(self) -> bytes:
        data: bytes
        if ON_WINDOWS:
            start_time = time.perf_counter()
            _, data = win32file.ReadFile(self.pipe, READ_BUFFER_SIZE)
            end_time = time.perf_counter()
            elapsed_time = end_time - start_time
        else:
            data = self.pipe.read(READ_BUFFER_SIZE)
            elapsed_time = 0

        self.log_time(
            "C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\python_pipe_recv.txt",
            elapsed_time,
        )
        return data
