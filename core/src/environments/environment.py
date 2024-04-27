import abc

import gymnasium as gym
import torch


class Environment(abc.ABC, gym.Env):
    @abc.abstractmethod
    def step(self, action: int) -> tuple[torch.Tensor, float, bool, bool, dict]:
        """
        :param action: Action to be performed in the environment.
        :return: tuple of:
        State of the environment after performing the action,
        Reward for performing the action,
        Whether the game ended or not
        """

    @abc.abstractmethod
    def reset(self, *, seed=None, options=None): ...

    @property
    @abc.abstractmethod
    def state(self) -> torch.Tensor: ...
