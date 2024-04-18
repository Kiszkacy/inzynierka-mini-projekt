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
            discounted_rewards[-k - 1] = current_reward_sum  # we start at the last reward

        return discounted_rewards

    def train(self, learning_rate: float = 1e-3, patience: int = 100):
        optimizer = torch.optim.Adam(self.policy_network.parameters(), lr=learning_rate, maximize=True)
        state = self.environment.state
        rewards = []
        game_number = 0
        log_probs = []
        loss = []
        best_reward = float('-inf')
        games_without_improvement = 0

        while True:
            policy_logits = self.policy_network(state)
            distribution = torch.distributions.Categorical(logits=policy_logits)
            action = distribution.sample()
            log_prob = distribution.log_prob(action)
            log_probs.append(log_prob)

            state, reward, done = self.environment.step(action.item())
            rewards.append(reward)

            if done:
                game_number += 1
                total_reward = sum(rewards)
                if total_reward > best_reward:
                    best_reward = total_reward
                    games_without_improvement = 0
                else:
                    games_without_improvement += 1
                    if games_without_improvement == patience:
                        break

                advantages = self.discount_rewards(torch.tensor(rewards, dtype=torch.float))
                # reconsider this, isn't sample size to small for normalization
                # advantages = (advantages - advantages.mean()) / advantages.std()

                log_probs_tensor = torch.stack(log_probs)
                policy_loss = torch.dot(log_probs_tensor, advantages) / len(advantages)

                optimizer.zero_grad()
                policy_loss.backward()
                optimizer.step()

                loss.append(policy_loss.item())
                log_probs = []
                rewards = []

        self.visualize_loss(loss)
