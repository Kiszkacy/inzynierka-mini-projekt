import functools
import inspect
import logging
import sys
from pathlib import Path

from loguru import logger
from pydantic import FilePath, DirectoryPath, Field, ValidationError, field_validator
from pydantic_settings import BaseSettings, SettingsConfigDict


def configure_logging():
    class InterceptHandler(logging.Handler):
        def emit(self, record: logging.LogRecord) -> None:
            # Get corresponding Loguru level if it exists.
            level: str | int
            try:
                level = logger.level(record.levelname).name
            except ValueError:
                level = record.levelno

            # Find caller from where originated the logged message.
            frame, depth = inspect.currentframe(), 0
            while frame and (depth == 0 or frame.f_code.co_filename == logging.__file__):
                frame = frame.f_back
                depth += 1

            logger.opt(depth=depth, exception=record.exc_info).log(level, record.getMessage())

    logger.remove()
    logger.add(sys.stderr, level="INFO")

    logging.basicConfig(handlers=[InterceptHandler()], level=0, force=True)
    logging.getLogger("ray").handlers = [InterceptHandler()]
    logging.getLogger("ray.tune").handlers = [InterceptHandler()]
    logging.getLogger("ray.rllib").handlers = [InterceptHandler()]
    logging.getLogger("ray.train").handlers = [InterceptHandler()]
    logging.getLogger("ray.serve").handlers = [InterceptHandler()]


class GodotSettings(BaseSettings):

    model_config = SettingsConfigDict(frozen=True)

    godot_executable: FilePath
    project_path: DirectoryPath

    # noinspection PyNestedDecorators
    @field_validator('godot_executable', mode='after')
    @classmethod
    def validate_godot_executable(cls, value: Path) -> Path:
        if value.suffix == '.exe':
            return value

        raise ValueError(f"Path should point to an .exe file but instead pointed to {value.suffix}")


class CoreSettings(BaseSettings):

    model_config = SettingsConfigDict(
        env_prefix="CORE_",
        env_file="../.env",
        env_nested_delimiter="__",
        env_file_encoding="utf-8",
        frozen=True,
    )

    godot: GodotSettings = Field(description="The godot settings")


@functools.lru_cache(maxsize=1)
def get_settings() -> CoreSettings:
    """Loads settings from .env file."""
    configure_logging()

    try:
        settings = CoreSettings()
        logger.success("Successfully loaded core settings.")
        return settings
    except ValidationError as e:
        logger.error(f"Error loading core settings: {e}")
        raise


def reload_settings() -> CoreSettings:
    """Reloads settings from disk."""
    get_settings.cache_clear()
    return get_settings()
