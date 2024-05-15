import os

import ray
import torch
from loguru import logger
from ray.rllib.algorithms import Algorithm
from ray.tune import register_env, Tuner

from core.src.environments.gymnasium_environment import GymnasiumServerEnvironment
from core.src.policies.pong_policy_network import PongPolicyNetwork
from core.src.settings import get_settings
from core.src.setup import configure_logging
from core.src.agents.pong_policy import CustomPolicy


class MyAlgo(Algorithm):
    def get_default_policy_class(self, config):
        return CustomPolicy


if __name__ == "__main__":
    os.environ["RAY_COLOR_PREFIX"] = "1"
    configure_logging()
    ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, configure_logging=False, num_gpus=1)

    DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    logger.info(f"Using {DEVICE=}")

    training_settings = get_settings().training

    env = GymnasiumServerEnvironment

    config = {
        "env": env,
        "framework": "torch",
        "num_workers": 5,
        "model": {
            "custom_model": PongPolicyNetwork,
        },
        "checkpoint_freq": 20,
    }

    tuner = Tuner(MyAlgo, param_space=config).fit()

