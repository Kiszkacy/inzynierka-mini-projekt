
import platform
from typing import Optional
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
    def __init__(self, pipe_name: Optional[str] = None) -> None:
        if pipe_name is None:
            pipe_name = self._default_pipe_name()
        self.pipe: Optional[int] = None
        self.pipe_name: str = pipe_name

    def _default_pipe_name(self) -> str:
        return r"\\.\pipe\godot-python-pipe" if ON_WINDOWS else "/tmp/godot-python-pipe"

    def connect(self) -> None:
        if ON_WINDOWS:
            self.pipe = win32pipe.CreateNamedPipe(
                self.pipe_name,
                win32pipe.PIPE_ACCESS_DUPLEX,
                win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
                1, MAX_BUFFER_SIZE, MAX_BUFFER_SIZE,
                0,
                None
            )
            win32pipe.ConnectNamedPipe(self.pipe, None)
        else:
            os.mkfifo(self.pipe_name)
            self.pipe = open(self.pipe_name, 'r+')
        logger.info(f"Connected to the {self.pipe_name} pipe.")

    def disconnect(self) -> None:
        if ON_WINDOWS:
            win32file.CloseHandle(self.pipe)
        else:
            self.pipe.close()
        self.pipe = None

    def send(self, data_bytes) -> None:
        if ON_WINDOWS:
            win32file.WriteFile(self.pipe, data_bytes)
        else:
            self.pipe.write(data_bytes)
            self.pipe.flush()

    def receive(self) -> bytes:
        data: bytes
        if ON_WINDOWS:
            _, data = win32file.ReadFile(self.pipe, READ_BUFFER_SIZE)
        else:
            data = self.pipe.read(READ_BUFFER_SIZE)
            data = data.encode()
        return data
