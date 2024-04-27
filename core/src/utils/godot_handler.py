import json
import random
import subprocess
import threading
from pathlib import Path

from core.src.settings import get_settings
from core.src.socket_server_thread import SocketServerThread
from loguru import logger


class GodotHandler:
    godot_executable: Path = get_settings().godot.godot_executable
    next_port: int = 20_000 + random.randint(0, 200)
    port_lock = threading.Lock()

    def __init__(self, project_path: str | None = None):
        self.project_path = get_settings().godot.project_path if project_path is None else project_path
        self.port = self._get_new_port_number()
        self.server_thread = SocketServerThread("localhost", self.port)
        self.godot_thread: threading.Thread | None = None

    @classmethod
    def _get_new_port_number(cls) -> int:
        with cls.port_lock:
            port = cls.next_port
            cls.next_port += 1
        return port

    def send(self, data: bytes) -> None:
        self.server_thread.send(data)

    def request_data(self) -> dict:
        while not self.server_thread.received_request:
            ...

        data: bytes = self.server_thread.get_request()
        decoded_data = json.loads(data.decode())
        return decoded_data

    def launch_godot(self):
        args = [
            f"--port={self.port}",
        ]
        command = [self.godot_executable, "--path", self.project_path] + args

        self._listen()  # starts server on the python's side, has to be done first

        try:
            # Run the command
            self.godot_thread = threading.Thread(target=subprocess.run, args=(command,))
            self.godot_thread.start()
        except subprocess.CalledProcessError as e:
            logger.error("Error running Godot project:", e)

    def _listen(self) -> None:
        self.server_thread.start()
