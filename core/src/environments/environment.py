import random

import torch


class Environment:
    counter = 0

    def step(self, action) -> tuple[torch.Tensor, float, bool]:
        ...

    @property
    def state(self) -> torch.Tensor:
        return torch.rand(size=[6])
