import random

import torch


class Environment:
    counter = 0

    def step(self, action: int) -> tuple[torch.Tensor, float, bool]:
        """
        :param action: Action to be performed in the environment.
        :return: tuple of:
        State of the environment after performing the action,
        Reward for performing the action,
        Whether the game ended or not
        """
        self.counter += 1
        return self.state, action, self.counter % 40 == 0

    @property
    def state(self) -> torch.Tensor:
        return torch.rand(size=[6])
