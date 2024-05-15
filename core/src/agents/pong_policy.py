from ray.rllib import SampleBatch
from ray.rllib.policy.policy import Policy
import torch

from core.src.policies.pong_policy_network import PongPolicyNetwork


class CustomPolicy(Policy):
    def __init__(self, observation_space, action_space, config):
        Policy.__init__(self, observation_space, action_space, config)
        self.policy_network = PongPolicyNetwork(observation_space.shape[0], action_space.n)
        self.lr = 0.001
        self.optimizer = torch.optim.Adam(self.policy_network.parameters(), lr=self.lr, maximize=True)
        # Zakładając istnienie optymalizatora
        self.gamma = 0.99
        self.patience = 100
        self.buffer = []
        self.loss = []

    def discount_rewards(self, rewards: torch.Tensor):
        """Decreasing rewards with respect to time."""
        discounted_rewards = torch.zeros_like(rewards)
        current_reward_sum = 0
        for k, reward in enumerate(reversed(rewards)):
            current_reward_sum = current_reward_sum * self.gamma + reward
            discounted_rewards[-k - 1] = current_reward_sum  # we start at the last reward

        return discounted_rewards

    def compute_actions(self, obs_batch, state_batches=None, prev_action_batch=None, prev_reward_batch=None,
                        info_batch=None, episodes=None, **kwargs):
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

    def learn_on_loaded_batch(self, offset: int = 0, buffer_index: int = 0):

        # Pobieranie wskaźników do nagród z batcha
        rewards = self.buffer[buffer_index].get("rewards", None)
        if rewards is None:
            raise ValueError
            return

        # Konwertowanie nagród na tensor PyTorch
        rewards_tensor = torch.tensor(rewards, dtype=torch.float32)

        discounted_rewards = self.discount_rewards(rewards_tensor)

        states = self.buffer[buffer_index].get("obs", None)
        if states is None:
            raise ValueError

        states_tensor = torch.tensor(states, dtype=torch.float32)

        policy_logits = self.policy_network(states_tensor)

        distribution = torch.distributions.Categorical(logits=policy_logits)

        actions = self.buffer[buffer_index].get("actions", None)
        if actions is None:
            return

        log_probs = distribution.log_prob(torch.tensor(actions))

        policy_loss = -(log_probs * discounted_rewards).mean()

        self.optimizer.zero_grad()
        policy_loss.backward()
        self.optimizer.step()

        # Zapisanie straty polityki dla celów monitorowania
        self.loss.append(policy_loss.item())
        return {"policy_loss": policy_loss.item()}

    def get_weights(self):
        """Funkcja pobierająca wagi polityki."""
        return self.policy_network.state_dict()

    def set_weights(self, weights):
        """Funkcja ustawiająca wagi polityki."""
        self.policy_network.load_state_dict(weights)

    def load_batch_into_buffer(self, batch, buffer_index: int = 0) -> int:
        if not SampleBatch == type(batch):
            raise TypeError
        while len(self.buffer) < buffer_index:
            self.buffer.append(None)
        self.buffer.append(batch)
        return 1
