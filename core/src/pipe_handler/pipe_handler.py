
import win32file
import win32pipe
from loguru import logger

MAX_BUFFER_SIZE: int = 2048
READ_BUFFER_SIZE: int = 512


class PipeHandler:
    def __init__(self, pipe_name: str = r"\\.\pipe\godot-python-pipe") -> None:
        self.pipe: int = win32pipe.CreateNamedPipe(
            pipe_name,
            win32pipe.PIPE_ACCESS_DUPLEX,
            win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
            1, MAX_BUFFER_SIZE, MAX_BUFFER_SIZE,
            0,
            None
        )
        self.pipe_name: str = pipe_name

    def connect(self) -> None:
        win32pipe.ConnectNamedPipe(self.pipe, None)
        logger.info(f"Connected to the {self.pipe_name=} pipe.")

    def disconnect(self) -> None:
        win32file.CloseHandle(self.pipe)

    def send(self, data_bytes) -> None:
        win32file.WriteFile(self.pipe, data_bytes)

    def receive(self) -> bytes:
        bytes_read: int; data: bytes
        bytes_read, data = win32file.ReadFile(self.pipe, READ_BUFFER_SIZE)
        return data
