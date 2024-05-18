import os

from ray.rllib.algorithms import Algorithm
from ray.tune import Tuner

from core.src.agents.pong_policy import PongAgentPolicy
from core.src.environments.environment import Environment
from core.src.environments.gymnasium_environment import GymnasiumServerEnvironment
from core.src.policies.policy_network import PolicyNetwork
from core.src.policies.pong_policy_network import PongPolicyNetwork
from core.src.settings import get_settings


class MyAlgo(Algorithm):
    def get_default_policy_class(self, config):  # noqa: ARG002
        return PongAgentPolicy


def get_path() -> str | None:
    project_root = os.getcwd()
    while os.path.basename(project_root) != "src":
        project_root = os.path.dirname(project_root)
        if not project_root:
            break
    if project_root:
        import_path = os.path.join(project_root, "model", "model.pth")
        if os.path.exists(import_path):
            return import_path
    return None


class TrainingHandler:
    def __init__(  # noqa: PLR0913
        self,
        model_path: str | None = None,
        environment_cls: type[Environment] = GymnasiumServerEnvironment,
        policy_cls: type[PolicyNetwork] = PongPolicyNetwork,
        learning_rate: float = 1e-3,
        gamma: float = 0.99,
    ):
        self.model_path = model_path
        self.environment_cls = environment_cls
        self.policy_cls = policy_cls
        self.learning_rate = learning_rate
        self.gamma = gamma

    def train(self):
        training_settings = get_settings().training
        config = {
            "env": self.environment_cls,
            "framework": "torch",
            "num_workers": training_settings.number_of_workers,
            "model": {
                "custom_model": self.policy_cls,
            },
            "checkpoint_freq": training_settings.training_checkpoint_frequency,
            "train_batch_size": training_settings.training_batch_size,
            "learning_rate": self.learning_rate,
            "gamma": self.gamma,
        }
        if self.model_path:
            config["model_path"] = self.model_path
            Tuner(MyAlgo, param_space=config).fit()
        else:
            config["model_path"] = get_path()
            Tuner(MyAlgo, param_space=config).fit()  # Tuner params are saved by default at path ~/ray_results)
