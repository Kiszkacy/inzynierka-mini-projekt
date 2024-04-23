import functools
from pathlib import Path

from loguru import logger
from pydantic import FilePath, DirectoryPath, Field, ValidationError, field_validator
from pydantic_settings import BaseSettings, SettingsConfigDict


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
