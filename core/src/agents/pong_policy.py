import os

import torch
from ray.rllib.policy.policy import Policy


class PongAgentPolicy(Policy):
    def __init__(self, observation_space, action_space, config):
        Policy.__init__(self, observation_space, action_space, config)
        self.policy_network = config["model"]["custom_model"](observation_space.shape[0], action_space.n)
        self.model_path = config["model_path"]
        if os.path.exists(self.model_path):
            self.policy_network.load_state_dict(torch.load(self.model_path))
        self.optimizer = torch.optim.Adam(self.policy_network.parameters(), lr=config["learning_rate"], maximize=True)
        self.gamma = config["gamma"]
        self.buffer = [None]
        self.loss = []

    def discount_rewards(self, rewards: torch.Tensor):
        """Decreasing rewards with respect to time."""
        discounted_rewards = torch.zeros_like(rewards)
        current_reward_sum = 0
        for k, reward in enumerate(reversed(rewards)):
            current_reward_sum = current_reward_sum * self.gamma + reward
            discounted_rewards[-k - 1] = current_reward_sum  # we start at the last reward

        return discounted_rewards

    def compute_actions(  # noqa: PLR0913
        self,
        obs_batch,
        state_batches=None,  # noqa: ARG002
        prev_action_batch=None,  # noqa: ARG002
        prev_reward_batch=None,  # noqa: ARG002
        info_batch=None,  # noqa: ARG002
        episodes=None,  # noqa: ARG002
        **kwargs,  # noqa: ARG002
    ):
        actions = []
        with torch.no_grad():
            for obs in obs_batch:
                obs_tensor = torch.tensor(obs, dtype=torch.float32)
                policy_logits = self.policy_network(obs_tensor.unsqueeze(0))

                # Tworzenie rozkładu prawdopodobieństwa
                distribution = torch.distributions.Categorical(logits=policy_logits)

                # Losowe próbkowanie akcji z rozkładu
                action = distribution.sample().item()
                actions.append(action)
        return actions, [], {}

    def learn_on_loaded_batch(self, offset: int = 0, buffer_index: int = 0):  # noqa: ARG002
        data = self.buffer[buffer_index]
        rewards = data.get("rewards", None)
        states = data.get("obs", None)
        actions = data.get("actions", None)

        policy_logits = self.policy_network(torch.tensor(states, dtype=torch.float32))
        distribution = torch.distributions.Categorical(logits=policy_logits)
        log_probs_tensor = distribution.log_prob(torch.tensor(actions))
        advantages = self.discount_rewards(torch.tensor(rewards, dtype=torch.float))
        # reconsider this, isn't sample size to small for normalization
        # advantages = (advantages - advantages.mean()) / advantages.std()
        policy_loss = torch.dot(log_probs_tensor, advantages) / len(advantages)

        self.optimizer.zero_grad()
        policy_loss.backward()
        self.optimizer.step()

        self.loss.append(policy_loss.item())
        return {"policy_loss": policy_loss.item()}

    def get_weights(self):
        return self.policy_network.state_dict()

    def set_weights(self, weights):
        os.makedirs("model", exist_ok=True)
        self.policy_network.load_state_dict(weights)
        torch.save(self.policy_network.state_dict(), self.model_path)

    def load_batch_into_buffer(self, batch, buffer_index: int = 0) -> int:
        num_devices = self.config["num_workers"]
        total_samples = buffer_index
        samples_per_device = total_samples // num_devices

        while len(self.buffer) < buffer_index:
            self.buffer.append(None)
        self.buffer[buffer_index] = batch

        return samples_per_device
