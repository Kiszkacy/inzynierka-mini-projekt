import torch
from torch import nn


class PolicyNetwork(nn.Module):

    def forward(self, x) -> torch.Tensor: ...
