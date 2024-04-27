import numpy as np
import torch
from core.src.environments.environment import Environment
from core.src.utils.godot_handler import GodotHandler
from gymnasium.spaces import Box, Discrete


class GymnasiumServerEnvironment(Environment):
    action_space = Discrete(2)
    # probably should be something more accurate
    observation_space = Box(low=-(2**60), high=2**60, shape=(6,), dtype=np.float32)

    def __init__(self, config: dict | None = None):
        self.godot_handler = GodotHandler()
        self._state: torch.Tensor | None = None
        self.initialized = False
        self.init()

    def step(self, action: np.integer) -> tuple[torch.Tensor, float, bool, bool, dict]:
        """
        :param action: Action to be performed in the environment.
        :return: tuple of:
        State of the environment after performing the action,
        Reward for performing the action,
        Whether the game ended or not,
        If truncated,
        Extra info
        """
        self.godot_handler.send(action.tobytes())
        return self.get_data()

    def get_data(self):
        data: dict = self.godot_handler.request_data()
        state = data["state"]
        reward: float = data["reward"]
        is_done: bool = data["is_done"]
        truncated = False
        info = {}
        return state, reward, is_done, truncated, info

    def reset(self, *, seed=None, options=None) -> tuple[torch.Tensor, dict]:
        """Resets the state of the environment and returns initial observations."""
        return self.default_state, {}

    @property
    def state(self) -> torch.Tensor:
        """Returns current state of the environment."""
        if self._state is None:
            self._state = self.get_data()[0]
        return self._state

    def init(self) -> None:
        self.godot_handler.launch_godot()
        self.initialized = True

    @property
    def default_state(self) -> np.random:
        return np.random.randn(6)
