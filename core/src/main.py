import os

import ray
import torch
from core.src.environments.gymnasium_environment import GymnasiumServerEnvironment
from core.src.settings import configure_logging, reload_settings
from loguru import logger
from ray.rllib.algorithms.ppo.ppo import PPOConfig

if __name__ == "__main__":
    os.environ["RAY_COLOR_PREFIX"] = "1"
    configure_logging()
    ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, configure_logging=False)

    DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    logger.info(f"Using {DEVICE=}")
    reload_settings()

    env = GymnasiumServerEnvironment

    config = (  # 1. Configure the algorithm,
        PPOConfig()
        .environment(env)
        .rollouts(num_rollout_workers=4)
        .framework("torch")
        .training(model={"fcnet_hiddens": [64, 64]})
        .evaluation(evaluation_num_workers=1)
    )

    algo = config.build(
    )  # 2. build the algorithm,

    for _ in range(5):
        logger.info(algo.train())  # 3. train it,

    algo.evaluate()
