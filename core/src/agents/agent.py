from pathlib import Path

from core.src.environments.environment import Environment
from core.src.policies.policy_network import PolicyNetwork


class Agent:
    def __init__(self, policy_network: PolicyNetwork, environment: Environment):
        self.policy_network = policy_network
        self.environment = environment

    def load(self, path: Path): ...

    def act(self, state): ...

    def train(self): ...
