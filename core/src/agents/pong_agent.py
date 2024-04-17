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

    def train(self, learning_rate: float = 1e-3):
        optimizer = torch.optim.Adam(self.policy_network.parameters(), lr=learning_rate, maximize=True)
        state = self.environment.state
        rewards = []
        game_number = 0
        log_probs = []
        loss = []
        actions = []

        while True:
            policy_logits = self.policy_network(state)
            distribution = torch.distributions.Categorical(logits=policy_logits)
            action = distribution.sample()
            log_prob = distribution.log_prob(action)
            log_probs.append(log_prob)

            actions.append(action.item())
            state, reward, done = self.environment.step(action.item())
            rewards.append(reward)

            if done:
                game_number += 1
                advantages = self.discount_rewards(torch.tensor(rewards, dtype=torch.float))
                advantages = (advantages - advantages.mean()) / advantages.std()

                policy_gradient = []
                for log_prob, advantage in zip(log_probs, advantages):
                    policy_gradient.append(log_prob * advantage)

                optimizer.zero_grad()
                policy_loss = torch.stack(policy_gradient).sum()
                policy_loss.backward()
                optimizer.step()

                loss.append(sum(actions))
                log_probs = []
                rewards = []
                actions = []
                
            if game_number == 400:  # it shouldn't work like that, add proper stop criteria
                break

        self.visualize_loss(loss)

