import os

import ray
import torch
from loguru import logger
from ray.rllib.algorithms.ppo.ppo import PPOConfig

from core.src.environments.gymnasium_environment import GymnasiumServerEnvironment
from core.src.settings import configure_logging, reload_settings

if __name__ == "__main__":
    os.environ["RAY_COLOR_PREFIX"] = "1"
    configure_logging()
    reload_settings()
    ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, configure_logging=False, num_gpus=1)

    DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    logger.info(f"Using {DEVICE=}")

    env = GymnasiumServerEnvironment

    config = (
        PPOConfig()
        .environment(env)
        .rollouts(
            num_rollout_workers=15,
            create_env_on_local_worker=False,
            num_envs_per_worker=1,
        )
        .framework("torch")
        .training(model={"fcnet_hiddens": [64, 64]})
    )

    algo = config.build()

    for _ in range(5):
        logger.info(algo.train())
