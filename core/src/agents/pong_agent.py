import torch

from core.src.agents.agent import Agent
from core.src.environments.environment import Environment
from core.src.policies.policy_network import PolicyNetwork


class PongAgent(Agent):

    def __init__(self, policy_network: PolicyNetwork, environment: Environment, gamma: float = 0.99):
        super().__init__(policy_network, environment)
        self.gamma = gamma

    def discount_rewards(self, rewards: torch.Tensor):
        """Decreasing rewards with respect to time."""
        discounted_rewards = torch.zeros_like(rewards)
        current_reward_sum = 0
        for k, reward in enumerate(reversed(rewards)):
            current_reward_sum = current_reward_sum * self.gamma + reward
            discounted_rewards[-k - 1] = current_reward_sum  # we start at the last rewards

        return discounted_rewards

    def train(self, batch_size: int = 10, learning_rate: float = 1e-3):
        ...

