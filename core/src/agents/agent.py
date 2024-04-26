from pathlib import Path

import matplotlib.pyplot as plt
import seaborn as sns
from core.src.environments.environment import Environment
from core.src.policies.policy_network import PolicyNetwork


class Agent:
    def __init__(self, policy_network: PolicyNetwork, environment: Environment):
        self.policy_network = policy_network
        self.environment = environment

    def load(self, path: Path): ...

    def act(self, state): ...

    def train(self): ...

    def visualize_loss(self, loss):
        sns.lineplot(y=loss, x=range(len(loss)))
        plt.show()
