import torch

from core.src.agents.pong_agent import PongAgent
from core.src.environments.server_environment import ServerEnvironment
from core.src.policies.pong_policy_network import PongPolicyNetwork

if __name__ == "__main__":
    DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    print(f"{DEVICE=}")

    env = ServerEnvironment()
    policy = PongPolicyNetwork(6, 2)
    agent = PongAgent(policy_network=policy, environment=env)

    agent.train()