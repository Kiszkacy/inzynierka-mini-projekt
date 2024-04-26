import json

import torch
from core.src.environments.environment import Environment
from core.src.SocketServerThread import SocketServerThread


class ServerEnvironment(Environment):
    counter = 0

    def __init__(self):
        self.server_thread = SocketServerThread("localhost", 12345)
        self.server_thread.start()
        self._state: torch.Tensor | None = None

    def step(self, action: int) -> tuple[torch.Tensor, float, bool]:
        """
        :param action: Action to be performed in the environment.
        :return: tuple of:
        State of the environment after performing the action,
        Reward for performing the action,
        Whether the game ended or not
        """
        self.server_thread.send(action.to_bytes())
        return self.request_data()

    def request_data(self):
        while not self.server_thread.received_request:
            ...

        data: bytes = self.server_thread.get_request()
        decoded_data = json.loads(data.decode())
        state = decoded_data["state"]
        reward = decoded_data["reward"]
        is_done = decoded_data["is_done"]

        return torch.tensor(state), reward, is_done

    @property
    def state(self) -> torch.Tensor:
        if self._state is None:
            self._state, _, _ = self.request_data()
        return self._state
